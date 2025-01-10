d# Fruitkha E-commerce Platform

## Project Overview
This project implements the **Fruitkha Theme** using **ASP.NET MVC** and an **N-Tier Architecture**. It is a dynamic e-commerce platform with an admin panel for managing content, products, and news. The platform includes advanced features like a shopping cart, Stripe integration, and user authentication.

---

## Features

### **Frontend Features**
- **Home Page**: Displays products, news, and team sections as configured via the admin panel.
- **Products**: Filterable list of products.
- **News**: News articles with a comment section powered by CKEditor.
- **Team Section**: Information about the team, dynamically managed via the admin panel.
- **Contact Segment**: Contact messages with a "read" checkbox for the admin.
- **Cart Section**: Add/remove items from the cart and apply optional coupons.

### **Backend Features**
- **Admin Panel**:
  - Manage Products: Add, edit, delete, and filter products.
  - Manage News: Add, edit, delete, and decide whether to show articles on the home page.
  - Team Management: Update team information.
  - Contact Management: Mark contact messages as read/unread.
- **Authentication & Authorization**:
  - Implement ASP.NET Identity for user management.
  - Role-based access control for admin and customers.
- **Coupon Logic (Optional)**: Apply discounts using coupons.
- **Stripe Integration**: Handle payments securely.

---

## Technology Stack
- **Framework**: ASP.NET MVC
- **Architecture**: N-Tier
  - **Presentation Layer**: Handles the UI.
  - **Business Logic Layer**: Contains business rules and application logic.
  - **Data Access Layer**: Manages database operations.
- **Database**: MSSQL
- **Frontend Theme**: Fruitkha (HTML, CSS, JavaScript)
- **Rich Text Editor**: CKEditor
- **Payment Gateway**: Stripe

---

## Project Structure
```
FruitkhaProject/
|-- PresentationLayer/       # MVC Controllers, Views, and Frontend Logic
|-- BusinessLogicLayer/      # Core Business Rules
|-- DataAccessLayer/         # Database Context and Repositories
|-- Models/                  # Shared Models
|-- wwwroot/                 # Static Assets (CSS, JS, Images)
```

---

## Installation

1. **Clone the Repository**:
   ```bash
   git clone <repository-url>
   cd FruitkhaProject
   ```

2. **Setup the Database**:
   - Update the connection string in `appsettings.json`.
   - Run migrations:
     ```bash
     dotnet ef database update
     ```

3. **Install Dependencies**:
   ```bash
   npm install
   ```

4. **Build and Run**:
   ```bash
   dotnet run
   ```

5. **Admin Access**:
   - Navigate to `/admin`.
   - Default admin credentials:
     - **Email**: `admin@example.com`
     - **Password**: `Admin@123`

---

## Usage

### Admin Panel
- Navigate to `/admin` to manage products, news, team, and contact messages.

### Coupons (Optional)
- Admin can create and manage coupons.
- Users can apply coupons at checkout for discounts.

### Stripe Integration
- Ensure Stripe API keys are added in `appsettings.json`.

---

## Future Enhancements
- Multi-language support.
- Advanced analytics dashboard for admin.
- Subscription-based product options.

---

## Contribution
We welcome contributions! Please follow these steps:
1. Fork the repository.
2. Create a feature branch:
   ```bash
   git checkout -b feature-name
   ```
3. Commit your changes:
   ```bash
   git commit -m "Add feature-name"
   ```
4. Push to your branch:
   ```bash
   git push origin feature-name
   ```
5. Open a pull request.

---

## License
This project is licensed under the MIT License.

---

## Contact
For questions or support, please contact the development team:
- Email: support@fruitkha.com

# Fruitkha E-commerce Platform

## Project Overview
This project implements the **Fruitkha Theme** using **ASP.NET MVC** and an **N-Tier Architecture**. It is a dynamic e-commerce platform with an admin panel for managing content, products, and news. The platform includes advanced features like a shopping cart, Stripe integration, and user authentication.

---

## Features

### **Frontend Features**
- **Home Page**: Displays products, news, and team sections as configured via the admin panel.
- **Products**: Filterable list of products.
- **News**: News articles with a comment section powered by CKEditor.
- **Team Section**: Information about the team, dynamically managed via the admin panel.
- **Contact Segment**: Contact messages with a "read" checkbox for the admin.
- **Cart Section**: Add/remove items from the cart and apply optional coupons.

### **Backend Features**
- **Admin Panel**:
  - Manage Products: Add, edit, delete, and filter products.
  - Manage News: Add, edit, delete, and decide whether to show articles on the home page.
  - Team Management: Update team information.
  - Contact Management: Mark contact messages as read/unread.
- **Authentication & Authorization**:
  - Implement ASP.NET Identity for user management.
  - Role-based access control for admin and customers.
- **Coupon Logic (Optional)**: Apply discounts using coupons.
- **Stripe Integration**: Handle payments securely.

---

## Technology Stack
- **Framework**: ASP.NET MVC
- **Architecture**: N-Tier
  - **Presentation Layer**: Handles the UI.
  - **Business Logic Layer**: Contains business rules and application logic.
  - **Data Access Layer**: Manages database operations.
- **Database**: MSSQL
- **Frontend Theme**: Fruitkha (HTML, CSS, JavaScript)
- **Rich Text Editor**: CKEditor
- **Payment Gateway**: Stripe

---

## Project Structure
```
FruitkhaProject/
|-- PresentationLayer/       # MVC Controllers, Views, and Frontend Logic
|-- BusinessLogicLayer/      # Core Business Rules
|-- DataAccessLayer/         # Database Context and Repositories
|-- Models/                  # Shared Models
|-- wwwroot/                 # Static Assets (CSS, JS, Images)
```

---

## Installation

1. **Clone the Repository**:
   ```bash
   git clone <repository-url>
   cd FruitkhaProject
   ```

2. **Setup the Database**:
   - Update the connection string in `appsettings.json`.
   - Run migrations:
     ```bash
     dotnet ef database update
     ```

3. **Install Dependencies**:
   ```bash
   npm install
   ```

4. **Build and Run**:
   ```bash
   dotnet run
   ```

5. **Admin Access**:
   - Navigate to `/admin`.
   - Default admin credentials:
     - **Email**: `admin@example.com`
     - **Password**: `Admin@123`

---

## Usage

### Admin Panel
- Navigate to `/admin` to manage products, news, team, and contact messages.

### Coupons (Optional)
- Admin can create and manage coupons.
- Users can apply coupons at checkout for discounts.

### Stripe Integration
- Ensure Stripe API keys are added in `appsettings.json`.

---

## Future Enhancements
- Multi-language support.
- Advanced analytics dashboard for admin.
- Subscription-based product options.

---

## Contribution
We welcome contributions! Please follow these steps:
1. Fork the repository.
2. Create a feature branch:
   ```bash
   git checkout -b feature-name
   ```
3. Commit your changes:
   ```bash
   git commit -m "Add feature-name"
   ```
4. Push to your branch:
   ```bash
   git push origin feature-name
   ```
5. Open a pull request.

---

## License
This project is licensed under the MIT License.

---

# Fruitkha E-commerce Platform

## Project Overview
This project implements the **Fruitkha Theme** using **ASP.NET MVC** and an **N-Tier Architecture**. It is a dynamic e-commerce platform with an admin panel for managing content, products, and news. The platform includes advanced features like a shopping cart, Stripe integration, and user authentication.

---

## Features

### **Frontend Features**
- **Home Page**: Displays products, news, and team sections as configured via the admin panel.
- **Products**: Filterable list of products.
- **News**: News articles with a comment section powered by CKEditor.
- **Team Section**: Information about the team, dynamically managed via the admin panel.
- **Contact Segment**: Contact messages with a "read" checkbox for the admin.
- **Cart Section**: Add/remove items from the cart and apply optional coupons.

### **Backend Features**
- **Admin Panel**:
  - Manage Products: Add, edit, delete, and filter products.
  - Manage News: Add, edit, delete, and decide whether to show articles on the home page.
  - Team Management: Update team information.
  - Contact Management: Mark contact messages as read/unread.
- **Authentication & Authorization**:
  - Implement ASP.NET Identity for user management.
  - Role-based access control for admin and customers.
- **Coupon Logic (Optional)**: Apply discounts using coupons.
- **Stripe Integration**: Handle payments securely.

---

## Technology Stack
- **Framework**: ASP.NET MVC
- **Architecture**: N-Tier
  - **Presentation Layer**: Handles the UI.
  - **Business Logic Layer**: Contains business rules and application logic.
  - **Data Access Layer**: Manages database operations.
- **Database**: MSSQL
- **Frontend Theme**: Fruitkha (HTML, CSS, JavaScript)
- **Rich Text Editor**: CKEditor
- **Payment Gateway**: Stripe

---

## Project Structure
---

## Installation

1. **Clone the Repository**:
2. **Setup the Database**:
   - Update the connection string in `appsettings.json`.
   - Run migrations:
3. **Install Dependencies**:
4. **Build and Run**:
5. **Admin Access**:
   - Navigate to `/admin`.
   - Default admin credentials:
     - **Email**: `admin@example.com`
     - **Password**: `Admin@123`

---

## Usage

### Admin Panel
- Navigate to `/admin` to manage products, news, team, and contact messages.

### Coupons (Optional)
- Admin can create and manage coupons.
- Users can apply coupons at checkout for discounts.

### Stripe Integration
- Ensure Stripe API keys are added in `appsettings.json`.

---

## Future Enhancements
- Multi-language support.
- Advanced analytics dashboard for admin.
- Subscription-based product options.

---

## Contribution
We welcome contributions! Please follow these steps:
1. Fork the repository.
2. Create a feature branch:
3. Commit your changes:
4. Push to your branch:
5. Open a pull request.

---

## License
This project is licensed under the MIT License.

---

## Contact
For questions or support, please contact the development team:
- Email: support@fruitkha.com
