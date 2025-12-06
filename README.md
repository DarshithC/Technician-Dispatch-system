# Technician Dispatch System
A complete desktop-based application for managing customers, technicians, service requests, dispatching, and revenue tracking — built using **C# (.NET)** and **Windows Forms** with JSON-based data persistence.

---

## 🚀 Problem Statement

Service companies often struggle with:

- Managing customer records
- Tracking technician skills and availability
- Scheduling and dispatching service requests
- Preventing double-booking
- Recording job completion details
- Calculating revenue

This system provides an **end-to-end solution** to these challenges.

---

## ⭐ Features

### **1. Dashboard**
- Real-time statistics
- Total customers, technicians, requests
- Revenue tracking

### **2. Customer Management**
- Add/manage customers
- Searchable list
- JSON data persistence

### **3. Technician Management**
- Multiple skills per technician
- Hourly rate setup
- Availability tracking

### **4. Service Request Management**
- Create service requests
- Automatic technician assignment based on:
  - Skills
  - Availability
- Manual reassignment
- Job completion with hours + parts tracking

### **5. Revenue Reports**
- Completed jobs listing
- Total revenue calculation
- Cost breakdown by technician/customer

---

## 🏗 Technical Architecture

### **Object-Oriented Classes**
- `Customer`
- `Technician`
- `ServiceRequest`
- `DispatchManager`
- `IdGenerator`
- `MainForm`
- `AssignTechForm`
- `CompleteJobForm`

### **Enums**
- `Status` → New, Dispatched, Completed, Cancelled

---

## 💾 File Handling (JSON)

All data is stored automatically in:

| File | Purpose |
|------|---------|
| `customers.json` | Customer data |
| `technicians.json` | Technician data |
| `requests.json` | Service request data |

Supports:
- Auto-save after each operation  
- Safe file I/O  
- JSON serialization/deserialization  

---

## 🛡 Exception & Input Handling

Includes:
- Input validation  
- User-friendly error messages  
- File handling error protection  
- Technician double-booking prevention  

---

## 🖥 User Interface (Windows Forms)

- Tab-based navigation  
- DataGridViews for listing  
- Modal dialogs for assignments & job completion  
- Responsive resizing  

---


MainForm.cs
AssignTechForm.cs
CompleteJobForm.cs
Program.cs
