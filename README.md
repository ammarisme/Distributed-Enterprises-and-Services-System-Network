# Distributed Enterprises and Services System Network

## Project Summary

This project focuses on developing a network of integrated enterprise information systems and services to cater to the needs of wholesale and retail enterprises in the domestic trade sector of Sri Lanka. The solution includes two main types of enterprise systems (Retail Enterprise Systems and Wholesale Enterprise Systems) and two types of trading portals (Retail Trading Portal and Wholesale Trading Portal). All systems are integrated through an integration system.

---

## Features

### 1. Retail Enterprise Systems
   - **Product Management**: Add, view, and edit products; set retail prices.
   - **Stock Management**: Manage stock levels, add received stock, and handle stock wastage.
   - **Sales Management**: Record retail sales and manage sales returns.
   - **Quotation Management**: Manage received quotations and sent quotation requests.
   - **Purchase Orders Management**: Create and manage purchase orders.
   - **Account Management**: User authentication, profile management, and password changes.
   - **Customer Management**: Manage customer information.
   - **Reports Generation**: Generate various reports for management and analysis.

### 2. Wholesale Enterprise Systems
   - **Product Management**: Add, view, and edit products; set retail and wholesale prices.
   - **Stock Management**: Manage stock levels, add received stock, and handle stock wastage.
   - **Sales Management**: Manage both wholesale and retail sales and returns.
   - **Quotation Management**: Manage sent quotations and received quotation requests.
   - **Customer Management**: Manage both retail and wholesale customer information.
   - **Reports Generation**: Generate various reports for management and analysis.

### 3. Retail Trading Portal
   - **Product Browsing**: Search and view retail products online.
   - **Shop Browsing**: Search and view retail shops online.
   - **Order Management**: Place and manage retail orders.

### 4. Wholesale Trading Portal
   - **Product Browsing**: Search and view wholesale products online.
   - **Shop Browsing**: Search and view wholesale shops online.
   - **Order Management**: Place and manage wholesale orders.
   - **Quotation Requests**: Add products to quotation requests and manage them.

### 5. Integration System
   - **Enterprise Integration Management**: Integrate, disintegrate, and manage enterprise systems in the network.
   - **Service Integration Management**: Manage integration and connectivity of services.
   - **Account Integration Management**: Manage user accounts and their association with enterprises and services.
   - **Automated Updates**: Automatically update inventory levels and stock information across systems.

---

## Implementation Details

### Development Methodology
The project followed the Rational Unified Process (RUP) to ensure a structured approach to analysis, design, and implementation. 

### Technologies Used
- **Backend**: ASP.NET, C#
- **Frontend**: HTML, CSS, JavaScript, AJAX
- **Database**: SQL Server with Entity Framework
- **Libraries and Frameworks**: LINQ, NewtonJsoft, jQuery, DataTable.js, Ripples.js, Bootstrap, Materialize CSS

### Deployment Environment
- **Server**: IIS Server 11.0
- **Development Tools**: Microsoft Visual Studio 2013, Google Chrome Developer Tools
- **Operating System**: Microsoft Windows 10 Pro

---

## Usage Instructions

1. **Clone the Repository**: 
   ```bash
   git clone <repository-url>

### Install Dependencies:

Ensure all necessary dependencies are installed for both frontend and backend development.
### Configure the Application:

Update configuration files (Web.config and App.config) with appropriate settings for your environment.
### Run the Backend Server:

Open the solution file in Visual Studio and run the project.
### Access the Frontend Application:
Serve the frontend files using a web server and navigate to the application in your browser.

```
Project 1: Integration System

|-- IntegrationSystem
    |-- App_Start
    |-- Areas
    |-- Controllers
    |-- DAL
    |-- Migrations
    |-- Models
    |-- Views
    |-- Content
    |-- Scripts
    |-- Properties
    |-- Global.asax
    |-- Startup.cs
    |-- IntegrationSystem.csproj
    |-- Web.config

Project 2: Retail Enterprise System

|-- RetailEnterpriseSystem
    |-- App_Start
    |-- Controllers
    |-- DAL
    |-- Migrations
    |-- Models
    |-- Views
    |-- Content
    |-- Scripts
    |-- Properties
    |-- Global.asax
    |-- Startup.cs
    |-- RetailEnterpriseSystem.csproj
    |-- Web.config

Project 3: Wholesale Enterprise System

|-- WholesaleEnterpriseSystem
    |-- App_Start
    |-- Controllers
    |-- DAL
    |-- Migrations
    |-- Models
    |-- Views
    |-- Content
    |-- Scripts
    |-- Properties
    |-- Global.asax
    |-- Startup.cs
    |-- WholesaleEnterpriseSystem.csproj
    |-- Web.config

Project 4: Retail Trading Portal

|-- RetailTradingPortal
    |-- App_Start
    |-- Controllers
    |-- DAL
    |-- Migrations
    |-- Models
    |-- Views
    |-- Content
    |-- Scripts
    |-- Properties
    |-- Global.asax
    |-- Startup.cs
    |-- RetailTradingPortal.csproj
    |-- Web.config

Project 5: Wholesale Trading Portal

|-- WholesaleTradingPortal
    |-- App_Start
    |-- Controllers
    |-- DAL
    |-- Migrations
    |-- Models
    |-- Views
    |-- Content
    |-- Scripts
    |-- Properties
    |-- Global.asax
    |-- Startup.cs
    |-- WholesaleTradingPortal.csproj
    |-- Web.config
```plaintext

### Conclusion
This project successfully provides a robust and scalable solution for automating sales processes and integrating various enterprise systems in the domestic trade sector. Future enhancements include implementing the systems using PHP for extendability and developing a cloud-based solution for broader accessibility.
