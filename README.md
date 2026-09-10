# Smart Queue API

Smart Queue API xidmət mərkəzində müştərilərin növbəsini idarə etmək üçün **.NET 8** və **N-Tier Architecture** prinsipilə hazırlanmış səliqəli, sürətli və production-ready RESTful API layihəsidir.

---

## 🛠 İstifadə Olunan Texnologiyalar

- **Framework:** .NET 8 (C#)
- **Database:** MS SQL Server
- **ORM:** Entity Framework Core 8
- **Architecture:** N-Tier (Domain, Infrastructure, Application, API)
- **Design Patterns:** Result Pattern, BaseResponse Wrapper, Repository Pattern
- **Documentation:** Swagger / OpenAPI

---

## 🏗 Layihə Strukturu (N-Tier)

- **`SmartQueue.Domain`**: Entity-lər, Enum-lar və ümumi `Result` pattern tipləri.
- **`SmartQueue.Infrastructure`**: DbContext, Migrations və Database Repository implementasiyaları.
- **`SmartQueue.Application`**: DTO-lar, Service-lər və Biznes məntiqi.
- **`SmartQueue.API`**: Controller-lər, `BaseResponse` cavab strukturu və konfiqurasiyalar.

---

## 🚀 API Endpoint-lər

| HTTP Method | Endpoint | Təsvir |
|---|---|---|
| `POST` | `/api/queue` | Yeni müştərini növbəyə əlavə edir |
| `GET` | `/api/queue` | Gözləyən bütün müştərilərin siyahısını qaytarır |
| `GET` | `/api/queue/{id}` | Müştəri məlumatını və növbədə neçənci olduğunu göstərir |
| `POST` | `/api/queue/next` | Növbədəki növbəti müştərini çağırır (`Serving` statusuna keçirir) |
| `DELETE` | `/api/queue/{id}` | Müştərini növbədən/sistemdən silir |

---

## 🔒 `/next` Endpoint-i üçün Concurrency Həlli

### Problem
Sistemə eyni anda çoxlu operator müştəri çağırmaq üçün `/api/queue/next` request-i göndərdikdə, **Race Condition** yaranması və eyni müştərinin iki müxtəlif operatora təyin edilməsi riski var.

### Həll Üsulu (Pessimistic Locking)
Verilənlər bazası səviyyəsində tranzaksiya daxilində **Row-level Lock** təmin edilmişdir:

```csharp
using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

var nextCustomer = await _context.Customers
    .FromSqlRaw("SELECT TOP 1 * FROM Customers WITH (UPDLOCK, READPAST) WHERE Status = {0} ORDER BY CreatedAt ASC", (int)CustomerStatus.Waiting)
    .FirstOrDefaultAsync(cancellationToken);
