# MiniDashboard

A **Mini Dashboard** project demonstrating a full-stack .NET solution using:

* **ASP.NET Core Web API** (backend)
* **WPF MVVM** desktop client (frontend)
* **Unit and integration tests**
* **UI automation tests** (FlaUI)

The app allows users to **view, add, edit, delete, search, and sort items** with persistence in a JSON file and offline caching.

---

## Table of Contents

* [Project Structure](#project-structure)
* [Features](#features)
* [Setup and Running](#setup-and-running)
* [Architecture](#architecture)
* [API Endpoints](#api-endpoints)
* [Frontend UI](#frontend-ui)
* [Testing](#testing)
* [Bonus Features](#bonus-features)

---

## Project Structure

```
MiniDashboard-[YourName]/
├─ src/
│  ├─ MiniDashboard.Api/          # ASP.NET Core Web API project
│  ├─ MiniDashboard.App/          # WPF MVVM frontend project
├─ tests/
│  ├─ MiniDashboard.Tests/        # Unit tests for API and services
│  ├─ MiniDashboard.IntegrationTests/  # Integration tests for API endpoints
│  ├─ MiniDashboard.UITests/      # Optional UI automation tests
├─ README.md
```

---

## Features

* **CRUD** operations for `Item` objects
* **Search and sort** functionality
* **Loading indicators** and **error messages**
* **Persistence** using a JSON file
* **Offline caching** 
* **Unit, integration, and UI tests**

---

## Setup and Running

### Prerequisites

* .NET 9 SDK or later
* Visual Studio 2022 or later (with WPF and ASP.NET workload)
* The API runs on HTTPS by default (`https://localhost:7264`). On a new machine, you may need to trust the .NET development certificate:

```powershell
dotnet dev-certs https --trust
```

### Run the API

1. Navigate to the API project folder:

```powershell
cd src\MiniDashboard.Api
```

2. Run the API:

```powershell
dotnet run
```

3. Open Swagger at `https://localhost:7264/swagger` to test endpoints.
(or http://localhost:5145/swagger)

---

### Run the WPF App

1. Navigate to the WPF project folder:

```powershell
cd src\MiniDashboard.App
```

2. Run the app:

```powershell
dotnet run
```

3. The app connects to the local API
4. Use the UI to manage items 
---

## Frontend UI

* **Top panel**: Search textbox and search button
* **DataGrid**: Shows all items, sortable columns
* **Bottom panel**: Textboxes for Name, Description, Price, and **Add** button
* **CRUD buttons**: Add, Update, Delete
* **Error messages**: Red TextBlock
* **Loading indicator**: Indeterminate ProgressBar

---

## Architecture

### Backend (API)

* **Controllers → Services → Repository**
* **Dependency Injection** used throughout
* **Persistence**: JSON file (`items.json`) stored in `Data` folder
* **Unit tests** cover services and controllers
* **Integration tests** cover all endpoints: GET, POST, PUT, DELETE

### Frontend (WPF MVVM)

* **MVVM pattern** (View, ViewModel, Model)
* **ObservableCollection** for dynamic UI updates
* **INotifyPropertyChanged** for binding
* **Commands** (`AddItemCommand`, `UpdateItemCommand`, `DeleteItemCommand`, `SearchCommand`)
* **Async API calls** via `HttpClient`
* **Loading indicators** (ProgressBar) and error messages (TextBlock)
* **UI caching** to local `cache` folder for offline mode

---

## API Endpoints

| Method | Endpoint                   | Description               |
| ------ | -------------------------- | ------------------------- |
| GET    | `/api/items`               | Get all items             |
| GET    | `/api/items/{id}`          | Get an item by ID         |
| GET    | `/api/items/search?query=` | Search items by name/desc |
| POST   | `/api/items`               | Add a new item            |
| PUT    | `/api/items/{id}`          | Update an existing item   |
| DELETE | `/api/items/{id}`          | Delete an item            |

---

## Testing

### Unit Tests

* API controller and service logic
* WPF ViewModel logic
* Uses **MSTest** and **Moq/NSubstitute** for mocking

### Integration Tests

* Tests API endpoints: GET, POST, PUT, DELETE
* Uses **WebApplicationFactory** and `HttpClient`

### UI Automation Tests (Optional)

* Uses **FlaUI** to simulate user actions
* Adds an item, verifies it appears in the DataGrid, then deletes it
* Can leave app open if a test fails for inspection
* Ensures test data is cleaned up

---

## Additional Features

* **Local caching**: Stores API responses in `cache` folder for offline mode
* **Error handling**: Gracefully displays API failures in the UI
* **UI automation**: Basic Add verification

---

## Notes

* Ensure the **API is running** before starting the WPF app
* JSON file (`items.json`) is recreated with sample data if missing
* UI tests use relative paths to locate the WPF executable (`MiniDashboard.App.exe`)
* Supports .NET 9.0+ on Windows

