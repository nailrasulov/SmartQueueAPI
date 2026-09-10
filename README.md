# Smart Queue API

Smart Queue API xidmət mərkəzində müştərilərin növbəsini idarə etmək üçün .NET 8 və N-Tier Architecture prinsipilə hazırlanmış səliqəli,sürətli RESTful API layihəsidir.

---

## İstifadə Olunan Texnologiyalar

- Framework: .NET 8 (C#)
- Database: MS SQL Server
- ORM: Entity Framework Core 8
- Architecture: N-Tier (Domain, Infrastructure, Application, API)
- Design Patterns: Result Pattern, BaseResponse, Repository Pattern
- Documentation: Swagger / OpenAPI

---

## Layihə Strukturu (N-Tier)

- `SmartQueue.Domain`: Entity-lər, Enum-lar və ümumi `Result` pattern tipləri.
- `SmartQueue.Infrastructure`: DbContext, Migrations və Database Repository implementasiyaları.
- `SmartQueue.Application`: DTO-lar, Service-lər və Biznes məntiqi.
- `SmartQueue.API`: Controller-lər, `BaseResponse` cavab strukturu və konfiqurasiyalar.

---

## Verilənlər Bazası (Database)

Layihədə **MS SQL Server** və **EF Core (Code-First)** yanaşmasından istifadə olunur.

### Tables (Cədvəllər)
`Customers` cədvəlinin strukturu:

| Sütun | Tip | Təsvir |
|---|---|---|
| `Id` | `int` (Primary Key, Identity) | Müştərinin unikal ID-si |
| `Name` | `nvarchar(100)` | Müştərinin adı |
| `CreatedAt` | `datetime` | Növbəyə daxilolma tarixi |
| `CompletedAt` | `datetime` (Nullable) | Xidmətin başa çatma tarixi |
| `Status` | `nvarchar(20)` / `int` | `Waiting`, `Serving`, `Completed` |

---

## API Endpoint-lər

| HTTP Method | Endpoint | Təsvir |
|---|---|---|
| `POST` | `/api/queue/create` | Yeni müştərini növbəyə əlavə edir |
| `GET` | `/api/queue/all-waiting` | Gözləyən bütün müştərilərin siyahısını qaytarır |
| `GET` | `/api/queue/get-by-id/{id}` | Müştəri məlumatını və növbədə neçənci olduğunu göstərir |
| `POST` | `/api/queue/call-next` | Növbədəki növbəti müştərini çağırır (`Serving` statusuna keçirir) |
| `POST` | `/api/queue/complete/{id}` | Müəyyən müştərini xidmətdən sonra statusunu `Completed` edir |
| `DELETE` | `/api/queue/delete/{id}` | Müştərini növbədən/sistemdən silir |

---

## `/call-next` Endpoint-i üçün Concurrency Həlli

### Problem
Sistemə eyni anda çoxlu operator müştəri çağırmaq üçün `/api/queue/call-next` request-i göndərdikdə, **Race Condition** yaranması və eyni müştərinin iki müxtəlif operatora təyin edilməsi riski var.

### Həll Üsulu (Pessimistic Locking)
Verilənlər bazası səviyyəsində tranzaksiya daxilində **Row-level Lock** təmin edilmişdir:

```csharp
using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

var nextCustomer = await _context.Customers
    .FromSql($"Select Top 1 * from Customers with (UPDLOCK, READPAST) where Status = {CustomerStatus.Waiting.ToString()} Order By CreatedAt ASC")
    .FirstOrDefaultAsync(cancellationToken);
