/*
  TaxiBookingDb_Setup.sql
  Creates a local development database for TaxiBookingSystem.

  Intended for a new local database. Back up any existing database before
  changing it. Sample taxi records and coordinates are illustrative only.
*/

IF DB_ID(N'TaxiBookingDb') IS NULL
BEGIN
    CREATE DATABASE TaxiBookingDb;
END;
GO

USE TaxiBookingDb;
GO

/* Customers */
IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Customers
    (
        CustomerID   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Customers PRIMARY KEY,
        FullName     NVARCHAR(100) NOT NULL,
        Email        NVARCHAR(254) NOT NULL,
        Phone        NVARCHAR(20) NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        CreatedAt    DATETIME2(0) NOT NULL CONSTRAINT DF_Customers_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT UQ_Customers_Email UNIQUE (Email),
        CONSTRAINT UQ_Customers_Phone UNIQUE (Phone)
    );
END;
GO

/* Admins: no default admin or plain-text password is inserted. */
IF OBJECT_ID(N'dbo.Admins', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Admins
    (
        AdminID      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Admins PRIMARY KEY,
        Username     NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        IsActive     BIT NOT NULL CONSTRAINT DF_Admins_IsActive DEFAULT (1),
        CONSTRAINT UQ_Admins_Username UNIQUE (Username)
    );
END;
GO

/* Locations */
IF OBJECT_ID(N'dbo.Locations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Locations
    (
        LocationID   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Locations PRIMARY KEY,
        LocationName NVARCHAR(150) NOT NULL,
        Latitude     FLOAT NOT NULL,
        Longitude    FLOAT NOT NULL,
        IsActive     BIT NOT NULL CONSTRAINT DF_Locations_IsActive DEFAULT (1),
        CONSTRAINT UQ_Locations_LocationName UNIQUE (LocationName),
        CONSTRAINT CK_Locations_Latitude CHECK (Latitude BETWEEN -90 AND 90),
        CONSTRAINT CK_Locations_Longitude CHECK (Longitude BETWEEN -180 AND 180)
    );
END;
GO

/* Taxis */
IF OBJECT_ID(N'dbo.Taxis', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Taxis
    (
        TaxiID          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Taxis PRIMARY KEY,
        CurrentLocation NVARCHAR(150) NULL,
        AvailableFrom   DATETIME2(0) NOT NULL CONSTRAINT DF_Taxis_AvailableFrom DEFAULT (SYSDATETIME()),
        TotalEarnings   DECIMAL(12,2) NOT NULL CONSTRAINT DF_Taxis_TotalEarnings DEFAULT (0),
        IsActive        BIT NOT NULL CONSTRAINT DF_Taxis_IsActive DEFAULT (1),
        CurrentLatitude FLOAT NULL,
        CurrentLongitude FLOAT NULL,
        CONSTRAINT CK_Taxis_TotalEarnings CHECK (TotalEarnings >= 0),
        CONSTRAINT CK_Taxis_CurrentLatitude CHECK (CurrentLatitude IS NULL OR CurrentLatitude BETWEEN -90 AND 90),
        CONSTRAINT CK_Taxis_CurrentLongitude CHECK (CurrentLongitude IS NULL OR CurrentLongitude BETWEEN -180 AND 180)
    );
END;
GO

/* Bookings. Location IDs are nullable for compatibility with older rows/code. */
IF OBJECT_ID(N'dbo.Bookings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Bookings
    (
        BookingID        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Bookings PRIMARY KEY,
        CustomerID       INT NOT NULL,
        TaxiID           INT NOT NULL,
        PickupLocation   NVARCHAR(150) NOT NULL,
        DropLocation     NVARCHAR(150) NOT NULL,
        PickupTime       DATETIME2(0) NOT NULL,
        DropTime         DATETIME2(0) NOT NULL,
        DistanceKm       DECIMAL(10,2) NOT NULL,
        Fare             DECIMAL(12,2) NOT NULL,
        BookingStatus    NVARCHAR(30) NOT NULL CONSTRAINT DF_Bookings_BookingStatus DEFAULT (N'Booked'),
        CreatedAt        DATETIME2(0) NOT NULL CONSTRAINT DF_Bookings_CreatedAt DEFAULT (SYSDATETIME()),
        PickupLocationID INT NULL,
        DropLocationID   INT NULL,
        CONSTRAINT FK_Bookings_Customers FOREIGN KEY (CustomerID) REFERENCES dbo.Customers(CustomerID),
        CONSTRAINT FK_Bookings_Taxis FOREIGN KEY (TaxiID) REFERENCES dbo.Taxis(TaxiID),
        CONSTRAINT FK_Bookings_PickupLocation FOREIGN KEY (PickupLocationID) REFERENCES dbo.Locations(LocationID),
        CONSTRAINT FK_Bookings_DropLocation FOREIGN KEY (DropLocationID) REFERENCES dbo.Locations(LocationID),
        CONSTRAINT CK_Bookings_DifferentLocations CHECK
            (PickupLocationID IS NULL OR DropLocationID IS NULL OR PickupLocationID <> DropLocationID),
        CONSTRAINT CK_Bookings_Distance CHECK (DistanceKm >= 0),
        CONSTRAINT CK_Bookings_Fare CHECK (Fare >= 0),
        CONSTRAINT CK_Bookings_TripTimes CHECK (DropTime >= PickupTime)
    );
END;
GO

/* Indexes for common booking queries */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Bookings_Customer_PickupTime' AND object_id = OBJECT_ID(N'dbo.Bookings'))
BEGIN
    CREATE INDEX IX_Bookings_Customer_PickupTime ON dbo.Bookings(CustomerID, PickupTime);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Bookings_Taxi_PickupTime_DropTime' AND object_id = OBJECT_ID(N'dbo.Bookings'))
BEGIN
    CREATE INDEX IX_Bookings_Taxi_PickupTime_DropTime ON dbo.Bookings(TaxiID, PickupTime, DropTime);
END;
GO

/* Demo locations: inserted only if each name is not already present. */
IF NOT EXISTS (SELECT 1 FROM dbo.Locations WHERE LocationName = N'Mepco College')
    INSERT dbo.Locations (LocationName, Latitude, Longitude, IsActive) VALUES (N'Mepco College', 9.4550, 77.7970, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.Locations WHERE LocationName = N'Sivakasi Bus Stand')
    INSERT dbo.Locations (LocationName, Latitude, Longitude, IsActive) VALUES (N'Sivakasi Bus Stand', 9.4500, 77.7970, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.Locations WHERE LocationName = N'Sivakasi Railway Station')
    INSERT dbo.Locations (LocationName, Latitude, Longitude, IsActive) VALUES (N'Sivakasi Railway Station', 9.4530, 77.8130, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.Locations WHERE LocationName = N'Sivakasi Main Market')
    INSERT dbo.Locations (LocationName, Latitude, Longitude, IsActive) VALUES (N'Sivakasi Main Market', 9.4505, 77.8020, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.Locations WHERE LocationName = N'Sivakasi Government Hospital')
    INSERT dbo.Locations (LocationName, Latitude, Longitude, IsActive) VALUES (N'Sivakasi Government Hospital', 9.4560, 77.8060, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.Locations WHERE LocationName = N'Sivakasi Fire Station')
    INSERT dbo.Locations (LocationName, Latitude, Longitude, IsActive) VALUES (N'Sivakasi Fire Station', 9.4470, 77.8040, 1);
GO

/* Demo taxis: add these only if no taxi records exist. */
IF NOT EXISTS (SELECT 1 FROM dbo.Taxis)
BEGIN
    INSERT dbo.Taxis (CurrentLocation, AvailableFrom, TotalEarnings, IsActive, CurrentLatitude, CurrentLongitude)
    VALUES
        (N'Mepco College', SYSDATETIME(), 0, 1, 9.4550, 77.7970),
        (N'Sivakasi Bus Stand', SYSDATETIME(), 0, 1, 9.4500, 77.7970),
        (N'Sivakasi Railway Station', SYSDATETIME(), 0, 1, 9.4530, 77.8130),
        (N'Sivakasi Main Market', SYSDATETIME(), 0, 1, 9.4505, 77.8020);
END;
GO

PRINT N'TaxiBookingDb setup completed.';
GO
