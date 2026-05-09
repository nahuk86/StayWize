**StayWize**

Sistema de gestión de ingresos y egresos para propiedades de alquiler temporario. Permite administrar propiedades, clientes, host locales, reservas y códigos de acceso físico, con registro completo de bitácora de eventos.

---

**Stack tecnológico**

- **Backend:** .NET 8 — ASP.NET Core Web API
- **Arquitectura:** Clean Architecture + CQRS con MediatR
- **Base de datos:** SQL Server 2022 en Docker
- **ORM:** Entity Framework Core 8
- **Contenedores:** Docker + Docker Compose
- **Documentación API:** Swagger / OpenAPI

---

**Estructura de la solución**

```
StayWize.sln
├── StayWize.API            → Controladores REST, middleware, configuración
├── StayWize.Application    → Use cases, DTOs, interfaces, CQRS handlers
├── StayWize.Domain         → Entidades, value objects, enums, excepciones
├── StayWize.Infrastructure → EF Core, repositorios, persistencia
└── StayWize.Services       → Servicios base transversales (auth, logs, i18n)
```

---

**Módulos implementados**

- **Propiedades** — ABM completo con soft delete y auditoría
- **Clientes** — ABM con validación de unicidad por email y documento
- **Host Locales** — ABM con gestión de disponibilidad por zona
- **Reservas** — Gestión de estados con control de concurrencia (SemaphoreSlim + Optimistic Concurrency)
- **Códigos de acceso** — Generación, validación y bitácora de ingresos/egresos

---

**Decisiones de diseño**

- **UUIDv7** como identificador de entidades: globalmente único, ordenado cronológicamente, sin fragmentación de índices
- **Soft delete global** implementado via `HasQueryFilter` en EF Core sobre todas las entidades
- **Auditoría completa** en todas las entidades: `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`, `DeletedAt`, `DeletedBy`
- **Optimistic Concurrency** con `RowVersion` para detectar conflictos a nivel de base de datos
- **SemaphoreSlim por recurso** para prevenir race conditions en creación de reservas y asignación de host locales

---

**Requisitos**

- Docker Desktop
- .NET 8 SDK
- Visual Studio 2022

---

**Levantar el proyecto**

```bash
git clone https://github.com/tu-usuario/staywize.git
cd staywize
docker-compose up --build
```

La API queda disponible en `http://localhost:5000/swagger`.

Para aplicar las migraciones localmente:

```bash
dotnet ef database update --project StayWize.Infrastructure --startup-project StayWize.API
```

---

**Roadmap**

| Milestone | Estado |
|-----------|--------|
| v0.1.0 — Infraestructura base | ✅ Completo |
| v0.2.0 — Módulos de negocio | ✅ Completo |
| v0.3.0 — Servicios base (auth, logs, i18n) | ✅ Completo |
| v0.4.0 — Gestión de ingresos y egresos | 🔄 En progreso |
| v0.5.0 — Frontend | ⏳ Pendiente |

---

**Contexto académico**

Proyecto final de la carrera Ingeniería en Sistemas Informáticos. El sistema implementa los requerimientos funcionales y no funcionales definidos en el plan de negocios StayWize, desarrollado en el marco de la materia Trabajo Final de Ingeniería.
