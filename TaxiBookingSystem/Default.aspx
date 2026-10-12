
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TaxiBookingSystem._Default" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Taxi Booking System</title>

    <style>
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: "Segoe UI", Arial, sans-serif;
            background: #f4f6fa;
            color: #172033;
            min-height: 100vh;
        }

        .navbar {
            height: 76px;
            background: #111827;
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 0 8%;
        }

        .brand {
            color: #ffffff;
            font-size: 25px;
            font-weight: 800;
            letter-spacing: -1px;
        }

        .brand span {
            color: #fbbf24;
        }

        .nav-label {
            color: #d1d5db;
            font-size: 18px;
        }

        .hero {
            max-width: 1100px;
            margin: 0 auto;
            padding: 65px 24px 30px;
            text-align: center;
        }

        .tagline {
            display: inline-block;
            background: #fef3c7;
            color: #92400e;
            padding: 8px 15px;
            border-radius: 30px;
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 20px;
        }

        .hero h1 {
            font-size: clamp(34px, 5vw, 52px);
            line-height: 1.15;
            margin-bottom: 18px;
            letter-spacing: -1.5px;
        }

        .hero h1 span {
            color: #d99b00;
        }

        .hero p {
            color: #64748b;
            font-size: 16px;
            line-height: 1.8;
            max-width: 620px;
            margin: 0 auto;
        }

        .cards {
            max-width: 850px;
            margin: 35px auto 60px;
            padding: 0 24px;
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 24px;
        }

        .login-card {
            background: #ffffff;
            border: 1px solid #e5e7eb;
            border-radius: 18px;
            padding: 32px;
            box-shadow: 0 12px 35px rgba(15, 23, 42, 0.06);
            text-align: left;
            transition: transform 0.2s, box-shadow 0.2s;
        }

        .login-card:hover {
            transform: translateY(-4px);
            box-shadow: 0 18px 40px rgba(15, 23, 42, 0.10);
        }

        .icon {
            width: 58px;
            height: 58px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 15px;
            font-size: 27px;
            margin-bottom: 22px;
        }

        .customer-icon {
            background: #fef3c7;
        }

        .admin-icon {
            background: #e0e7ff;
        }

        .login-card h2 {
            font-size: 23px;
            margin-bottom: 12px;
        }

        .login-card p {
            color: #64748b;
            font-size: 14px;
            line-height: 1.7;
            min-height: 48px;
            margin-bottom: 25px;
        }

        .action {
            display: block;
            width: 100%;
            padding: 13px 18px;
            border-radius: 9px;
            text-align: center;
            text-decoration: none;
            font-weight: 700;
            font-size: 15px;
            transition: opacity 0.2s;
        }

        .action:hover {
            opacity: 0.88;
        }

        .customer-button {
            background: #fbbf24;
            color: #111827;
        }

        .admin-button {
            background: #1e293b;
            color: #ffffff;
        }

        .register {
            text-align: center;
            color: #64748b;
            font-size: 14px;
            margin-top: -30px;
            margin-bottom: 50px;
        }

        .register a {
            color: #a87500;
            font-weight: 700;
            text-decoration: none;
        }

        .register a:hover {
            text-decoration: underline;
        }

        footer {
            background: #111827;
            color: #9ca3af;
            padding: 22px;
            text-align: center;
            font-size: 13px;
        }

        @media (max-width: 600px) {
            .navbar {
                padding: 0 20px;
                height: 68px;
            }

            .brand {
                font-size: 22px;
            }

            .nav-label {
                font-size: 18px;
            }

            .hero {
                padding-top: 45px;
            }

            .cards {
                grid-template-columns: 1fr;
                margin-top: 28px;
            }

            .login-card {
                padding: 26px;
            }

            .register {
                margin-top: 0;
                padding: 0 20px;
            }
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <header class="navbar">
            <div class="nav-label">Taxi Booking Management System</div>
        </header>

        <main>
            <section class="cards">

                <article class="login-card">
                    <div class="icon customer-icon">&#128100;</div>

                    <h2>Customer Portal</h2>

                    <p>
                        Book a taxi, choose your pickup and drop locations,
                        and view your booking history.
                    </p>

                    <a class="action customer-button"
                       href="Login.aspx">
                        Customer Login &rarr;
                    </a>
                </article>

                <article class="login-card">
                    <div class="icon admin-icon">&#128274;</div>

                    <h2>Administrator</h2>

                    <p>
                        Access the administration dashboard to manage
                        customers, taxis and bookings.
                    </p>

                    <a class="action admin-button"
                       href="AdminLogin.aspx">
                        Admin Login &rarr;
                    </a>
                </article>

            </section>

            <div class="register">
                New?
                <a href="Register.aspx">Create a customer account</a>
            </div>
        </main>

        <footer>
            &copy; 2026 Taxi Booking System
            <br />
            Simple. Reliable. Convenient.
        </footer>

    </form>
</body>
</html>
