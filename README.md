# Usage Guide

This application can connect to a **local** PostgreSQL instance or spin up a **Docker** container for the database. Choose whichever fits your environment.

---

## Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/download)
* (Optional) Docker & Docker Compose (for the Docker path)

> **Tip:** If you already have a local PostgreSQL instance listening on port 5432, pick a different host port (e.g. 5433) and set `DB_PORT` accordingly in your `.env`. You’ll also need to update the `ports:` mapping under **Docker-Hosted Database**.

You should have a `.env` file in the project root with these variables:

```ini
DB_HOST=localhost
DB_PORT=5432
DB_USER=user
DB_PASS=pass
DB_NAME=db_test
```

Copy from the provided `.env.example` and adjust values as needed.

---

### A) Local PostgreSQL Setup

1. **Ensure PostgreSQL is running** on `localhost:5432` and you have a role with `CREATEDB` rights.

2. **Create database & role (once)**:

   ```bash
   psql -d postgres #enter postresql

   -- replace user/pass with your values:
   CREATE ROLE user LOGIN PASSWORD 'pass' CREATEDB;
   CREATE DATABASE db_test OWNER user ENCODING 'UTF8' TEMPLATE template0;
   \q
   ```

3. **Update `.env`** to match your local setup:

   ```ini
   DB_HOST=localhost
   DB_PORT=5432
   DB_USER=user
   DB_PASS=pass
   DB_NAME=db_test
   ```

4. **Restore & apply migrations**:

   ```bash
   dotnet restore
   dotnet tool install --global dotnet-ef   # if needed
   dotnet ef database update                # creates schema
   ```

5. **Run Mode 1** (verify table creation):

   ```bash
   dotnet run -- 1
   ```

---

### B) Docker-Hosted Database (no app container)

1. **Start the Postgres service**:

   ```bash
   docker-compose up -d db
   ```

   Wait until `db` is healthy:

   ```bash
   docker-compose ps
   ```

2. **Run migrations and the app locally against the Docker Postgres**:

   ```bash
   dotnet restore
   dotnet tool install --global dotnet-ef   # if needed
   dotnet ef database update                # applies migrations to the Docker DB
   dotnet run -- 1                          # runs Mode 1 locally against Docker DB
   ```

3. **Inspect the schema (optional)**:

   ```bash
   psql -h localhost -p 5433 -U my_user -d db_test -c "\d employees"
   ```

4. **Run other modes**:

   ```bash
   dotnet run -- 2 "Ivanov Petr Sergeevich" 2009-07-12 M
   dotnet run -- 3
   dotnet run -- 4
   ```

---

## Task 5 – Execution Time Report

### Sample Command

```bash
dotnet run -- 5
```

### Sample Output

```
Mode 5: Query male employees with surnames starting 'F'…
Found 100 employees in 11 ms.
Fedorov0000010 Ivan Ivanovich	1950-01-11	M	Age=75
Filippov0214752 Petr Petrovich	2009-01-24	M	Age=16
...
Found 100099 records in 686 ms.
```

### Observed Execution Time

**686 ms** (on local machine: MacBook Air M2 8GB / PostgreSQL 14.15)

---
