# 🚚 Online Ordering & Delivery Management System

A complete Food Delivery Management System consisting of:

- 🖥️ Windows Forms Admin Application
- 📱 Flutter Android Customer Application
- 🛵 Flutter Android Rider Application
- 🔥 Firebase Authentication
- ☁️ Cloud Firestore Database

## 📌 Project Overview

Delivery Management System is designed to manage food ordering, delivery operations and resturent management efficiently.

The system consists of two platforms:

### 1. Admin Desktop Application (Windows Forms)

Used by administrators to manage and monitor the entire delivery process and resturent management options.

### 2. Mobile Application (Flutter)

A single mobile application with two user roles:

- Customer
- Delivery Rider

---

## 🖥️ Admin Features

### Authentication
- Admin Login

### Dashboard
- Total Orders
- Total Products
- Total Users
- Monthly Progress
- Salary Information
- Billing Information

### Order Management
- View all orders
- Update order status
- Delete orders
- Track delivery progress

### Product Management
- Add products
- Update products
- Delete products
- View product list

### Customer Management
- View customers
- Manage customer information

### Reports
- Generate delivery reports
- Monitor business performance

---

## 📱 Customer Features

### Account Management
- Register account
- Login account

### Product Browsing
- View available foods
- Search products

### Shopping Cart
- Add items to cart
- Checkout orders

### Order Tracking
Track order status through:

- Pending
- Preparing
- Delivering
- Delivered

### Order History
- View previous orders

---

## 🛵 Rider Features

### Authentication
- Rider Login

### Order Management
- View assigned orders
- View delivery address
- View customer order details

### Delivery Status Updates
Update order status:

- Pending → Preparing
- Preparing → Delivering
- Delivering → Delivered

### Earnings Tracking
- View completed deliveries
- Track earnings

---

## 🛠️ Technologies Used

### Frontend
- Flutter
- Dart
- Windows Forms (C#)

### Backend
- Firebase Authentication
- Cloud Firestore

### Development Tools
- Visual Studio
- Visual Studio Code
- Android Studio

---

## 📂 Project Structure

```text
Food-Delivery-System
│
├── AdminApp (Windows Forms)
│
├── MobileApp (Flutter)
│   ├── Customer Module
│   └── Rider Module
│
└── Firebase Backend
```

---

## 🔥 Database Collections

### Users

```json
{
  "name": "Kamal",
  "mobile": "0771234567",
  "role": "customer"
}
```

### Products

```json
{
  "name": "Burger",
  "price": 750,
  "image": "image_url"
}
```

### Orders

```json
{
  "customerId": "customer_id",
  "address": "Customer Address",
  "items": [],
  "total": 1500,
  "status": "Pending",
  "riderId": ""
}
```

---

## 🚀 Installation

### Flutter Application

```bash
flutter pub get
flutter run
```

### Windows Forms Application

1. Open solution in Visual Studio
2. Restore NuGet packages
3. Configure Firebase credentials
4. Build and Run

---

## 📸 WinForm Admin Application Screenshots

<img width="844" height="1169" alt="178006495350" src="https://github.com/user-attachments/assets/53dbca26-9738-4a43-bade-dcf8aa225933" />




<h2>📱 Mobile Application Screenshots</h2>

<p align="center">
  <img src="https://github.com/user-attachments/assets/36bb0a03-7268-4bd2-a64e-f01e3e6edc45" width="250"/>
  <img src="https://github.com/user-attachments/assets/1e8c05fb-80a5-4f5e-88eb-ad89a53871ef" width="250"/>
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/b3dd1273-d821-48c4-bbc8-334cc6cf7d97" width="250"/>
  <img src="https://github.com/user-attachments/assets/1bcf7a05-bdf8-4595-9729-4890c03c0c7c" width="250"/>
</p>

---

## 👨‍💻 Developer

**Undergraduates**
University of Sri Jayewardenepura

### Interests
- Machine Learning
- Automation
- Mobile App Development
- Software Engineering

---

## 📄 License

This project is developed for educational and learning purposes.
