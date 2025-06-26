# 📆 Reservation Notification System

An event-driven microservices-based system built with ASP.NET Core Web API that manages reservations and sends confirmation emails using RabbitMQ and SMTP.

---

## 🧰 Tech Stack

- ✅ ASP.NET Core Web API (.NET 7 / .NET 8)
- 📬 RabbitMQ (event messaging)
- 📧 SMTP with MailKit for email notifications
- 🛢️ SQL Server via Entity Framework Core
- 🔍 MediatR + CQRS Pattern
- 🧪 FluentValidation for request validation
- 🧪 Swagger (Swashbuckle) for API documentation
- 🐋 Docker for RabbitMQ & SQL Server

---

## 🏗️ Project Structure

This solution includes two services:

```
ReservationSolution/
├── ReservationService/      → Handles reservation CRUD
├── NotificationService/     → Listens to events and sends emails
└── README.md                → You're here!
```

---

## 🚀 Features

### 📌 Reservation Service
- Create, Read, Update, Delete (CRUD) reservations
- Publishes `ReservationCreated` & `ReservationDeleted` events to RabbitMQ
- Uses SQL Server with EF Core for persistence
- Validates all inputs using FluentValidation
- RESTful API with full Swagger documentation

### 📬 Notification Service
- Listens to RabbitMQ events
- Sends emails on new or deleted reservations
- Uses MailKit for SMTP integration
- Lightweight, no internal persistence (stateless)
- Input validations for incoming messages

---

## 🐳 Docker Setup

Before starting, ensure Docker is installed and running.

### 🔄 Start SQL Server
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Password" \
-p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest
```

### 🐇 Start RabbitMQ
```bash
docker run -d --hostname my-rabbit --name rabbitmq \
-p 5672:5672 -p 15672:15672 rabbitmq:3-management
```
RabbitMQ UI: [http://localhost:15672](http://localhost:15672)  
Login: `guest` / `guest`

---

## ⚙️ Running the Services

### ⬇️ 1. Clone the Repository
```bash
git clone https://github.com/your-username/reservation-system.git
cd reservation-system
```

### 🛠️ 2. Update Configurations

Each service has `appsettings.json`. Update the following:
- **SQL Server connection string** in `ReservationService`
- **RabbitMQ host/user/pass** in both services
- **SMTP settings** in `NotificationService`

Use [Ethereal Email](https://ethereal.email/) for testing SMTP if needed.

### 🧱 3. Apply Migrations

From the root directory:
```bash
cd ReservationService
dotnet ef database update
cd ../NotificationService
dotnet ef database update
```

### 🚀 4. Run Services

In two terminals:
```bash
dotnet run --project ReservationService
dotnet run --project NotificationService
```

---

## 🔬 Testing

### 🧪 Try ReservationService
Swagger UI available at:  
`http://localhost:5000/swagger` *(or port you configured)*

Test endpoints:
- `POST /api/reservations` ➜ Creates reservation
- `GET /api/reservations/{id}` ➜ Fetch by ID
- `PUT /api/reservations/{id}` ➜ Update
- `DELETE /api/reservations/{id}` ➜ Delete

🎯 A `POST` or `DELETE` triggers RabbitMQ event → NotificationService sends email.

### 📫 View Sent Emails
If using Ethereal:
1. Go to [https://ethereal.email](https://ethereal.email)
2. Login with the test account you created
3. See incoming emails (no actual delivery)

---

## ✅ Validation

Both services validate incoming data using `FluentValidation`.  
If validation fails, a `400 Bad Request` with error details will be returned.

Example invalid response:
```json
{
  "errors": {
    "email": ["Email is required"],
    "name": ["Name must be at least 3 characters"]
  }
}
```

---

## 🧠 Architecture Overview

```
User ➜ ReservationService ➜ RabbitMQ ➜ NotificationService ➜ Email Sent
```

- Loose coupling via RabbitMQ events
- No direct HTTP calls between services
- Easy to scale NotificationService independently

---

## ✉️ SMTP Example (Ethereal)

In `NotificationService/appsettings.json`:
```json
"Email": {
  "Host": "smtp.ethereal.email",
  "Port": 587,
  "User": "your-ethereal-user",
  "Pass": "your-ethereal-pass"
}
```

Create a free SMTP test account at: [https://ethereal.email](https://ethereal.email)

---

## 🔒 Security Notes
- Do not commit secrets to Git!
- Use [dotnet user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) or environment variables
- Make sure SMTP credentials are protected

---

## 📄 License

This project is open-sourced under the MIT License.  
See the [LICENSE](LICENSE) file for more information.

---

## 🤝 Contributing

Contributions are welcome!  
Feel free to open issues or submit pull requests.

---

## 📞 Contact

Maintained by **your-team-name**  
For support or bugs: [open an issue](https://github.com/your-username/reservation-system/issues)

---

🧡 Happy Coding!