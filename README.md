# EV Rent – Electric Vehicle Rental Platform

## Overview

EV Rent is a web-based electric vehicle rental platform developed as a diploma project.

The application allows users to explore electric vehicles, make rental reservations, calculate estimated driving range, locate nearby charging stations, and complete payments online.

The system also provides administrative functionality for managing vehicles, rentals, charging stations, and payments.

## Features

### User Features
- User registration and authentication
- Browse available electric vehicles
- View vehicle details
- Make rental reservations
- Manage personal rentals
- Complete payments through Stripe Checkout
- Calculate estimated driving range based on driving conditions and external factors
- Locate nearby charging stations using OpenChargeMap
- Email notifications

### Admin Features
- Administrative dashboard
- Manage electric vehicles
- Manage rental reservations
- Manage charging stations
- Manage payments
- View system statistics

## Technologies Used

- **C#**
- **.NET 10**
- **ASP.NET Core MVC**
- **Entity Framework Core**
- **Microsoft SQL Server**
- **ASP.NET Core Identity**
- **Razor Views**
- **HTML5 / CSS3 / JavaScript**
- **Bootstrap**
- **MailKit** – Email functionality
- **Stripe** – Online payment integration
- **OpenChargeMap API** – Charging station information

## Project Structure

The solution follows the ASP.NET Core MVC architecture:

- `Controllers` – Handle application requests and business logic.
- `Models` – Define the application's data models.
- `Views` – Contain the user interface using Razor.
- `Data` – Database context and Entity Framework migrations.
- `Services` – Application services, including email, charging station integration, and range calculation.
- `Areas/Identity` – Authentication and account management pages.
- `wwwroot` – Static files such as CSS, JavaScript, and images.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Visual Studio 2022 or later
- Microsoft SQL Server or SQL Server LocalDB
- A Stripe account for payment integration
- An OpenChargeMap API key
- Gmail SMTP credentials for email functionality

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/IordanP1/Ev-Rent-Diploma.git
   ```

2. Open the solution file:

   `Ev Rent-Diploma.slnx`

3. Restore NuGet packages:

   ```bash
   dotnet restore
   ```

4. Configure the required application settings using .NET User Secrets.

### Configuration

The application uses User Secrets to store sensitive configuration values during development.

Set the following configuration values:

- `ConnectionStrings:DefaultConnection`
- `EmailSettings:SmtpServer`
- `EmailSettings:Port`
- `EmailSettings:SenderName`
- `EmailSettings:SenderEmail`
- `EmailSettings:Username`
- `EmailSettings:Password`
- `OpenChargeMap:ApiKey`
- `Stripe:SecretKey`

Example:

```bash
dotnet user-secrets set "Stripe:SecretKey" "YOUR_STRIPE_SECRET_KEY" --project "UserAdmin/Ev Rent.csproj"
```

Repeat the command for the other required settings using your own credentials and API keys.

**Important:** Never commit real passwords, API keys, or other sensitive information to GitHub.

### Database Setup

Apply the existing Entity Framework Core migrations:

```bash
dotnet ef database update --project "UserAdmin/Ev Rent.csproj"
```

Make sure your SQL Server connection string is correctly configured before running the command.

### Run the Application

Start the application:

```bash
dotnet run --project "UserAdmin/Ev Rent.csproj"
```

The application will be available at the local address displayed in the terminal.

## External Integrations

- **Stripe:** Handles online payment processing through Stripe Checkout.
- **OpenChargeMap:** Provides information about electric vehicle charging stations.
- **MailKit:** Supports email communication and notifications.

## Purpose

This project was developed as part of my diploma project:

**"Design and Development of an Intelligent Web-Based Electric Vehicle Rental System."**

It demonstrates practical experience with ASP.NET Core MVC, database management, authentication, external API integrations, and payment processing.