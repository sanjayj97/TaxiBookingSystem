
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TaxiBookingSystem.Services
{
    public class BookingResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int BookingID { get; set; }
        public int TaxiID { get; set; }
        public decimal DistanceKm { get; set; }
        public decimal Fare { get; set; }
        public DateTime DropTime { get; set; }
    }

    public class BookingService
    {
        private const double AverageSpeedKmph = 30.0;
        private const decimal BaseFare = 50m;
        private const decimal FarePerKm = 15m;

        private class LocationInfo
        {
            public int ID;
            public string Name;
            public double Latitude;
            public double Longitude;
        }

        private class TaxiInfo
        {
            public int ID;
            public string Location;
            public double Latitude;
            public double Longitude;
            public DateTime AvailableFrom;
            public decimal Earnings;
        }

        public BookingResult BookTaxi(
            int customerID,
            int pickupLocationID,
            int dropLocationID,
            DateTime pickupTime)
        {
            if (customerID <= 0)
                return Failure("Please log in before booking a taxi.");

            if (pickupLocationID <= 0 || dropLocationID <= 0)
                return Failure("Please select valid pickup and drop locations.");

            if (pickupLocationID == dropLocationID)
                return Failure("Pickup and drop locations must differ.");

            if (pickupTime <= DateTime.Now.AddMinutes(1))
                return Failure("Choose a pickup time at least two minutes in the future.");

            string connectionString =
                ConfigurationManager.ConnectionStrings["TaxiBookingDb"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                using (SqlTransaction tx =
                    con.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        // 1. Retrieve and validate the selected locations.
                        LocationInfo pickup =
                            GetLocation(con, tx, pickupLocationID);

                        LocationInfo drop =
                            GetLocation(con, tx, dropLocationID);

                        if (pickup == null || drop == null)
                        {
                            tx.Rollback();
                            return Failure(
                                "Select valid active pickup and drop locations.");
                        }

                        // 2. Calculate trip distance using geographic coordinates.
                        double rawTripDistance = DistanceKm(
                            pickup.Latitude,
                            pickup.Longitude,
                            drop.Latitude,
                            drop.Longitude);

                        decimal tripDistance = Math.Round(
                            (decimal)Math.Max(0.1, rawTripDistance),
                            2,
                            MidpointRounding.AwayFromZero);

                        // 3. Estimate trip duration and drop-off time.
                        int tripMinutes = Math.Max(
                            1,
                            (int)Math.Ceiling(
                                (double)tripDistance / AverageSpeedKmph * 60.0));

                        DateTime dropTime = pickupTime.AddMinutes(tripMinutes);

                        // 4. Calculate the fare.
                        decimal fare = Math.Round(
                            BaseFare + FarePerKm * tripDistance,
                            2,
                            MidpointRounding.AwayFromZero);

                        // 5. Load active taxis while holding database locks.
                        // Close the reader before executing any other query.
                        List<TaxiInfo> taxis = new List<TaxiInfo>();

                        using (SqlCommand cmd = new SqlCommand(@"
                            SELECT TaxiID,
                                   CurrentLocation,
                                   CurrentLatitude,
                                   CurrentLongitude,
                                   AvailableFrom,
                                   TotalEarnings
                            FROM dbo.Taxis WITH (UPDLOCK, HOLDLOCK)
                            WHERE IsActive = 1
                              AND CurrentLatitude IS NOT NULL
                              AND CurrentLongitude IS NOT NULL
                            ORDER BY TaxiID;",
                            con, tx))
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    TaxiInfo taxi = new TaxiInfo();

                                    taxi.ID = Convert.ToInt32(
                                        reader["TaxiID"]);

                                    taxi.Location = Convert.ToString(
                                        reader["CurrentLocation"]);

                                    taxi.Latitude = Convert.ToDouble(
                                        reader["CurrentLatitude"]);

                                    taxi.Longitude = Convert.ToDouble(
                                        reader["CurrentLongitude"]);

                                    taxi.AvailableFrom = Convert.ToDateTime(
                                        reader["AvailableFrom"]);

                                    taxi.Earnings = Convert.ToDecimal(
                                        reader["TotalEarnings"]);

                                    taxis.Add(taxi);
                                }
                            }
                        }

                        // 6. Select the nearest taxi that can arrive on time.
                        TaxiInfo selectedTaxi = null;
                        double selectedPickupDistance = double.MaxValue;

                        foreach (TaxiInfo taxi in taxis)
                        {
                            DateTime now = DateTime.Now;

                            DateTime approachStart =
                                taxi.AvailableFrom > now
                                    ? taxi.AvailableFrom
                                    : now;

                            double approachDistance = DistanceKm(
                                taxi.Latitude,
                                taxi.Longitude,
                                pickup.Latitude,
                                pickup.Longitude);

                            int approachMinutes = (int)Math.Ceiling(
                                approachDistance / AverageSpeedKmph * 60.0);

                            DateTime estimatedArrival =
                                approachStart.AddMinutes(approachMinutes);

                            // Reject taxis that cannot reach the pickup on time.
                            if (estimatedArrival > pickupTime)
                                continue;

                            // Reject taxis already assigned during this interval.
                            if (HasOverlappingBooking(
                                con,
                                tx,
                                taxi.ID,
                                approachStart,
                                dropTime))
                            {
                                continue;
                            }

                            bool isCloser =
                                approachDistance <
                                selectedPickupDistance - 0.000001;

                            bool sameDistanceButLowerEarnings =
                                Math.Abs(
                                    approachDistance - selectedPickupDistance)
                                    <= 0.000001
                                && selectedTaxi != null
                                && taxi.Earnings < selectedTaxi.Earnings;

                            if (selectedTaxi == null ||
                                isCloser ||
                                sameDistanceButLowerEarnings)
                            {
                                selectedTaxi = taxi;
                                selectedPickupDistance = approachDistance;
                            }
                        }

                        if (selectedTaxi == null)
                        {
                            tx.Rollback();

                            return Failure(
                                "No free taxi can reach this pickup location by the requested time.");
                        }

                        // 7. Insert the booking and retrieve its generated ID.
                        int bookingID;

                        using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO dbo.Bookings
                            (
                                CustomerID,
                                TaxiID,
                                PickupLocation,
                                DropLocation,
                                PickupLocationID,
                                DropLocationID,
                                PickupTime,
                                DropTime,
                                DistanceKm,
                                Fare,
                                BookingStatus
                            )
                            OUTPUT INSERTED.BookingID
                            VALUES
                            (
                                @CustomerID,
                                @TaxiID,
                                @PickupLocation,
                                @DropLocation,
                                @PickupLocationID,
                                @DropLocationID,
                                @PickupTime,
                                @DropTime,
                                @DistanceKm,
                                @Fare,
                                'Booked'
                            );",
                            con, tx))
                        {
                            cmd.Parameters.Add(
                                "@CustomerID", SqlDbType.Int).Value = customerID;

                            cmd.Parameters.Add(
                                "@TaxiID", SqlDbType.Int).Value = selectedTaxi.ID;

                            cmd.Parameters.Add(
                                "@PickupLocation", SqlDbType.NVarChar, 150)
                                .Value = pickup.Name;

                            cmd.Parameters.Add(
                                "@DropLocation", SqlDbType.NVarChar, 150)
                                .Value = drop.Name;

                            cmd.Parameters.Add(
                                "@PickupLocationID", SqlDbType.Int)
                                .Value = pickup.ID;

                            cmd.Parameters.Add(
                                "@DropLocationID", SqlDbType.Int)
                                .Value = drop.ID;

                            cmd.Parameters.Add(
                                "@PickupTime", SqlDbType.DateTime2)
                                .Value = pickupTime;

                            cmd.Parameters.Add(
                                "@DropTime", SqlDbType.DateTime2)
                                .Value = dropTime;

                            SqlParameter distanceParameter =
                                cmd.Parameters.Add(
                                    "@DistanceKm", SqlDbType.Decimal);

                            distanceParameter.Precision = 8;
                            distanceParameter.Scale = 2;
                            distanceParameter.Value = tripDistance;

                            SqlParameter fareParameter =
                                cmd.Parameters.Add(
                                    "@Fare", SqlDbType.Decimal);

                            fareParameter.Precision = 10;
                            fareParameter.Scale = 2;
                            fareParameter.Value = fare;

                            bookingID = Convert.ToInt32(
                                cmd.ExecuteScalar());
                        }

                        // 8. Update taxi location, availability and earnings.
                        using (SqlCommand cmd = new SqlCommand(@"
                            UPDATE dbo.Taxis
                            SET CurrentLocation = @Location,
                                CurrentLatitude = @Latitude,
                                CurrentLongitude = @Longitude,
                                AvailableFrom = @AvailableFrom,
                                TotalEarnings = TotalEarnings + @Fare
                            WHERE TaxiID = @TaxiID
                              AND IsActive = 1;",
                            con, tx))
                        {
                            cmd.Parameters.Add(
                                "@Location", SqlDbType.NVarChar, 150)
                                .Value = drop.Name;

                            SqlParameter latitudeParameter =
                                cmd.Parameters.Add(
                                    "@Latitude", SqlDbType.Decimal);

                            latitudeParameter.Precision = 9;
                            latitudeParameter.Scale = 6;
                            latitudeParameter.Value =
                                (decimal)drop.Latitude;

                            SqlParameter longitudeParameter =
                                cmd.Parameters.Add(
                                    "@Longitude", SqlDbType.Decimal);

                            longitudeParameter.Precision = 9;
                            longitudeParameter.Scale = 6;
                            longitudeParameter.Value =
                                (decimal)drop.Longitude;

                            cmd.Parameters.Add(
                                "@AvailableFrom", SqlDbType.DateTime2)
                                .Value = dropTime;

                            SqlParameter updateFareParameter =
                                cmd.Parameters.Add(
                                    "@Fare", SqlDbType.Decimal);

                            updateFareParameter.Precision = 10;
                            updateFareParameter.Scale = 2;
                            updateFareParameter.Value = fare;

                            cmd.Parameters.Add(
                                "@TaxiID", SqlDbType.Int)
                                .Value = selectedTaxi.ID;

                            int rowsUpdated = cmd.ExecuteNonQuery();

                            if (rowsUpdated != 1)
                            {
                                throw new InvalidOperationException(
                                    "The selected taxi could not be updated.");
                            }
                        }

                        // Both the booking and taxi update succeed together.
                        tx.Commit();

                        return new BookingResult
                        {
                            Success = true,
                            Message = "Taxi booked successfully.",
                            BookingID = bookingID,
                            TaxiID = selectedTaxi.ID,
                            DistanceKm = tripDistance,
                            Fare = fare,
                            DropTime = dropTime
                        };
                    }
                    catch
                    {
                        try
                        {
                            tx.Rollback();
                        }
                        catch
                        {
                            // Preserve the original exception.
                        }

                        throw;
                    }
                }
            }
        }

        // Retrieves one active location using its database ID.
        private static LocationInfo GetLocation(
            SqlConnection con,
            SqlTransaction tx,
            int locationID)
        {
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT LocationID,
                       LocationName,
                       Latitude,
                       Longitude
                FROM dbo.Locations
                WHERE LocationID = @LocationID
                  AND IsActive = 1;",
                con, tx))
            {
                cmd.Parameters.Add(
                    "@LocationID", SqlDbType.Int).Value = locationID;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    LocationInfo location = new LocationInfo();

                    location.ID = Convert.ToInt32(
                        reader["LocationID"]);

                    location.Name = Convert.ToString(
                        reader["LocationName"]);

                    location.Latitude = Convert.ToDouble(
                        reader["Latitude"]);

                    location.Longitude = Convert.ToDouble(
                        reader["Longitude"]);

                    return location;
                }
            }
        }

        // Returns true if this taxi already has a booking that overlaps
        // the proposed time interval.
        private static bool HasOverlappingBooking(
            SqlConnection con,
            SqlTransaction tx,
            int taxiID,
            DateTime proposedStart,
            DateTime proposedEnd)
        {
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT COUNT(*)
                FROM dbo.Bookings WITH (UPDLOCK, HOLDLOCK)
                WHERE TaxiID = @TaxiID
                  AND BookingStatus = 'Booked'
                  AND PickupTime < @ProposedEnd
                  AND DropTime > @ProposedStart;",
                con, tx))
            {
                cmd.Parameters.Add(
                    "@TaxiID", SqlDbType.Int).Value = taxiID;

                cmd.Parameters.Add(
                    "@ProposedStart", SqlDbType.DateTime2)
                    .Value = proposedStart;

                cmd.Parameters.Add(
                    "@ProposedEnd", SqlDbType.DateTime2)
                    .Value = proposedEnd;

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                return count > 0;
            }
        }

        // Calculates the straight-line distance between two coordinates.
        private static double DistanceKm(
            double lat1,
            double lon1,
            double lat2,
            double lon2)
        {
            const double EarthRadiusKm = 6371.0;

            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            double sinLat = Math.Sin(dLat / 2.0);
            double sinLon = Math.Sin(dLon / 2.0);

            double a =
                sinLat * sinLat +
                Math.Cos(DegreesToRadians(lat1)) *
                Math.Cos(DegreesToRadians(lat2)) *
                sinLon * sinLon;

            // Protect against floating-point rounding errors.
            a = Math.Max(0.0, Math.Min(1.0, a));

            return EarthRadiusKm *
                   2.0 *
                   Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private static BookingResult Failure(string message)
        {
            return new BookingResult
            {
                Success = false,
                Message = message
            };
        }
    }
}
