using System;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDirectory
{
    public class EmployeeContext : DbContext
    {
        private readonly string _connString;

        public EmployeeContext()
            : this(BuildConnectionStringFromEnv()) { }

        public EmployeeContext(string connString)
        {
            _connString = connString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseNpgsql(_connString);

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Employee>(entity =>
            {
                entity.ToTable("employees");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.LastName).IsRequired().HasColumnType("TEXT");

                entity.Property(e => e.FirstName).IsRequired().HasColumnType("TEXT");

                entity.Property(e => e.MiddleName).HasColumnType("TEXT");

                entity.Property(e => e.BirthDate).IsRequired().HasColumnType("DATE");

                entity.Property(e => e.Gender).IsRequired().HasColumnType("CHAR(1)");

                entity
                    .HasIndex(e => new
                    {
                        e.LastName,
                        e.FirstName,
                        e.MiddleName,
                        e.BirthDate,
                    })
                    .IsUnique();

                //entity.HasIndex(e => new { e.Gender, e.LastName });
            });
        }

        private static string BuildConnectionStringFromEnv()
        {
            Env.Load();

            var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            var user = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
            var pass = Environment.GetEnvironmentVariable("DB_PASS") ?? "postgres";
            var db = Environment.GetEnvironmentVariable("DB_NAME") ?? "db_test";

            return $"Host={host};Port={port};Username={user};Password={pass};Database={db}";
        }
    }
}
