# E Auction: Live Bidding Platform

A full featured online auction system built with ASP.NET MVC 4. Sellers list products, admins moderate and publish listings, and buyers bid in real time with countdown timers. The platform supports category based auction alerts, email notifications, and a dedicated admin dashboard for system management.

## Features

### Live Auction and Bidding
- Browse all active auction listings with product images, descriptions, and base prices
- Filter products by category (Home Decor, Office, Kitchen, Government Product, Art, Collectibles, Electronics, Fashion)
- Place bids on live items with a real time countdown timer
- Automatic auction resolution: items marked as sold (if bids exist) or unsold when the timer expires

### Seller Workflow
- Register an account and submit products for auction (including image upload, base price, and bid duration)
- Products go through admin approval before going live
- Track your live listings, sold items, and total earnings from the seller dashboard

### Buyer Experience
- Browse and search live auctions across multiple categories
- Place competitive bids during the countdown window
- View purchase history and account dashboard
- Subscribe to category based auction alerts to get notified about new listings

### Admin Panel
- Dedicated admin login with a localhost only initial setup endpoint
- Dashboard displaying total bid requests, users, live products, sold items, and alert subscriptions
- Approve or reject seller bid requests and publish products to the live auction
- Manage products, sold records, users, and alert subscriptions
- Manually add or repost products

### Email Notifications
- Sellers notified when their product is approved and goes live
- Subscribers alerted when a new product is listed in their chosen category
- Buyer and seller both notified when an auction closes with a winning bid

### Static Pages
- FAQs, Terms of Use, Privacy Policy, Refund Policy, Company Info, Contact Us, and more

## Tech Stack

| Layer        | Technology                                      |
|--------------|------------------------------------------------|
| Framework    | ASP.NET MVC 4 (.NET Framework 4.5)             |
| ORM          | Entity Framework 5                              |
| Database     | SQL Server LocalDB                              |
| View Engine  | Razor                                           |
| Frontend     | jQuery 1.7, jQuery UI 1.8, Modernizr 2.5       |
| Validation   | jQuery Validation, Unobtrusive Validation       |
| Data Binding | KnockoutJS 2.1                                  |
| Bundling     | ASP.NET Web Optimization                        |
| Email        | System.Net.Mail with configurable SMTP settings |
| Auth         | Custom session based authentication             |

## Project Structure

```
MvcApplication1.sln
MvcApplication1/
    Controllers/
        HomeController.cs      # Public facing auction, auth, user features
        AdminController.cs     # Admin panel, moderation, product management
    Models/
        Bidding.cs             # Entity models (Product, Regi, Sold, BidRequest, AuctionAlert, Admin)
        actionDbContext.cs     # Entity Framework DbContext
    Views/
        Home/                  # 40+ views for auction, auth, account, static pages
        Admin/                 # Admin dashboard, moderation, and management views
        Shared/                # Layouts and partial views
    Services/
        AppEmailService.cs     # Centralized SMTP email service
    App_Start/
        RouteConfig.cs         # URL routing
        BundleConfig.cs        # Script and style bundling
        DummyDataSeeder.cs     # Sample product seeder (runs on startup)
    Content/                   # CSS, images, and static assets
    Scripts/                   # JavaScript libraries
    App_Data/                  # Local database files (excluded from version control)
```

## Getting Started

### Prerequisites
- Windows with .NET Framework 4.5 or later
- SQL Server LocalDB (included with Visual Studio)
- Visual Studio 2012 or later (recommended) or VS Code with C# support

### Running with Visual Studio
1. Open `MvcApplication1.sln` in Visual Studio
2. Restore NuGet packages (right click the solution and select Restore NuGet Packages)
3. Press F5 to build and run. The app will launch in your default browser

### Running with VS Code
Use the preconfigured tasks:
1. Run the **Run Full Stack (Restore + Build + IIS)** task to restore packages, build, and start IIS Express on port 8080
2. Open `http://localhost:8080` in your browser

### First Time Setup
- The application seeds 20 sample products on first run via `DummyDataSeeder`
- To create the initial admin account, navigate to `http://localhost:<port>/Admin/SetupAdmin` (works only from localhost)
- Register a user account through the home page to start bidding or selling

## User Roles

| Role   | Access                                                                 |
|--------|------------------------------------------------------------------------|
| Buyer  | Browse auctions, place bids, subscribe to alerts, view purchase history|
| Seller | Submit products for auction, track listings and earnings               |
| Admin  | Moderate bid requests, manage products and users, view dashboard stats |

## Configuration

SMTP email settings are configured in `Web.config` under `appSettings`:

```xml
<add key="SmtpHost" value="smtp.gmail.com" />
<add key="SmtpPort" value="587" />
<add key="SmtpUser" value="" />
<add key="SmtpPass" value="" />
<add key="SmtpFrom" value="no-reply@example.com" />
```

Fill in `SmtpUser` and `SmtpPass` with valid credentials to enable email notifications. If left empty, the application will skip sending emails gracefully.

## Screenshots

| Home Page | Login |
|-----------|-------|
| ![Home](docs/screenshots/01-home.png) | ![Login](docs/screenshots/02-login.png) |

| Admin Login | Admin Dashboard |
|-------------|-----------------|
| ![Admin Login](docs/screenshots/03-admin-login.png) | ![Admin Dashboard](docs/screenshots/04-admin-dashboard.png) |

## Known Limitations

- This is a legacy academic project intended for learning and demonstration purposes
- Passwords are stored without hashing (not suitable for production use)
- Authentication is session based without framework level security features
- The bidding engine supports a fixed number of concurrent auction channels
- Images are stored as binary data in the database rather than in external storage

## License

This project is provided as is for educational and portfolio purposes.
