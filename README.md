# UI Crafter - Mono Repository

## Overview

UI Crafter is a low-code development tool designed to help users create simple mobile app views and integrate them with basic API functionality. Inspired by the growing popularity of low-code platforms, this project allows users, even those with limited programming knowledge, to quickly design and deploy functional app interfaces.

The main goal of UI Crafter is to simplify app development by providing an easy-to-use drag-and-drop interface for building views. Users can create and test app views without needing to go through the complexities of deployment or backend setup.

## Running the project
UI Crafter server project uses`EF Core` and has a dependency on `PostgrSQL`. The project is setup with a docker-compose file that will spin up a default instance that will work out of the box.

Start up a docker container with postgres
```sh
docker-compose up -d
```

Change directory to server project
```sh
cd src/UICrafter/
```

Run database update based on project migrations
```sh
dotnet ef database update
```
The project should now be runnable as normal

## Project Structure

### Backend
- **Framework**: ASP.NET Core
  - The backend handles data management, user authentication, and API integration. It is built using ASP.NET Core for its robustness and scalability.
  
- **Database**: PostgreSQL
  - PostgreSQL is used for storing user data and app view configurations. It is a reliable relational database system with strong integration through EF Core.

- **Data Format**: Protocol Buffers
  - We use Protocol Buffers for efficient data serialization, ensuring that app views are stored and retrieved in a compact, high-performance format.

### Web Application
- **Framework**: Blazor
  - The web interface is developed with Blazor, enabling users to easily design app views using a drag-and-drop interface. This allows users to create views that are then sent to the backend for storage.

### Mobile Application
- **Framework**: Blazor Hybrid
  - Blazor Hybrid allows the mobile app to display views created by users on the web. The mobile app can load these views, enabling users to interact with their custom app designs on their smartphones.

### Key Features
- **Low-Code View Creation**: Users can design app interfaces with a simple drag-and-drop tool, making it accessible even for those without extensive programming knowledge.
- **API Integration**: The created app views can connect to APIs, allowing users to interact with external data. Buttons within the app can trigger API calls and display responses directly in the view.
- **Cross-Platform Support**: The mobile app, built using Blazor Hybrid and .NET MAUI, supports both iOS and Android, providing a seamless experience across devices.

## Project Goals

The primary goal of UI Crafter is to offer users an intuitive platform for building mobile app views without the complexities of traditional development. Users can quickly design views, integrate basic API functionality, and use their designs on a mobile app—all without needing to understand deployment processes or backend systems.