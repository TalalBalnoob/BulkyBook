# BulkyBook - E-Commerce Web Application

Welcome to **BulkyBook**! This is a full-featured e-commerce web application built using ASP.NET Core MVC. This project was developed to learn and implement modern web development practices, including the Repository Pattern, Unit of Work, Identity for authentication, and Stripe for payments.

## 🚀 Features

*   **Product Management**: Full CRUD operations for Categories, Products, and Companies.
*   **Shopping Cart**: Users can add products to their cart, adjust quantities, and manage their orders.
*   **Authentication & Authorization**: Secure login and registration using ASP.NET Core Identity.
*   **Role-Based Access**: Different views and permissions for Admin and Customer users.
*   **Payment Integration**: Secure checkout process using **Stripe API**.
*   **Order Management**: Admins can manage order statuses (Processing, Shipped, etc.), while customers can track their history.
*   **Clean Architecture**: Separation of concerns using multiple projects (Web, DataAccess, Models, and Utility).

## 🛠️ Technologies Used

*   **.NET 9.0** (C# 13.0)
*   **ASP.NET Core MVC**
*   **Entity Framework Core**
*   **SQL Server**
*   **ASP.NET Core Identity**
*   **Stripe API** (for payments)
*   **Bootstrap 5** (for styling)
*   **TinyMCE** (for rich text editing in the admin panel)
*   **SweetAlert2 & Toastr** (for notifications)

## 📋 Project Structure

The solution is divided into several layers:

*   **BulkyBookWeb**: The main MVC project containing Controllers, Views, and Areas (Admin/Customer).
*   **BulkyBook.DataAccess**: Handles database context, migrations, and repository implementations.
*   **BulkyBook.Models**: Contains all domain models and view models.
*   **BulkyBook.Utility**: Helper classes, static strings (like roles), and external services (like Email Sender).

## ⚙️ Getting Started

### Prerequisites

*   [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
*   [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or LocalDB)
*   [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [JetBrains Rider](https://www.jetbrains.com/rider/)

### Installation & Setup

1.  **Clone the repository**:
    ```bash
    git clone https://github.com/your-username/BulkyBook.git
    cd BulkyBook
    ```

2.  **Configure the Connection String**:
    Open `BulkyBookWeb/appsettings.json` and update the `DefaultConnection` to point to your local SQL Server instance.

3.  **Setup Stripe**:
    Add your Stripe Secret and Publishable keys to `appsettings.json` or use Secret Manager:
    ```json
    "Stripe": {
      "SecretKey": "your_secret_key",
      "PublishableKey": "your_publishable_key"
    }
    ```

4.  **Apply Migrations**:
    Open the Package Manager Console in Visual Studio and run:
    ```powershell
    Update-Database
    ```
    Or via terminal:
    ```bash
    dotnet ef database update --project BulkyBook.DataAccess --startup-project BulkyBookWeb
    ```

5.  **Run the Project**:
    Press `F5` in your IDE or use the terminal:
    ```bash
    dotnet run --project BulkyBookWeb
    ```

## 📝 Acknowledgments

This project was built as part of a learning journey to master ASP.NET Core. Special thanks to the community and various tutorials (like those from DotNetMastery) that helped in understanding these concepts!

---
*Created by a passionate Junior Developer 💻*
