using System;
using DotNetEnv;
using Npgsql;

namespace EmployeeDirectory
{
    class Program
    {
        static void Main(string[] args)
        {
            Env.Load();

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: dotnet run <mode> [parameters...]");
                return;
            }

            var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            var user = Environment.GetEnvironmentVariable("DB_USER") ?? "";
            var pass = Environment.GetEnvironmentVariable("DB_PASS") ?? "";
            var db = Environment.GetEnvironmentVariable("DB_NAME") ?? "";

            var connString =
                $"Host={host};Port={port};Username={user};Password={pass};Database={db}";

            var mode = args[0];
            var dbm = new EmployeeRepository(connString);

            switch (mode)
            {
                case "1":
                    Console.WriteLine("Mode 1: Creating employees table");
                    dbm.CreateEmployeeTable();
                    Console.WriteLine("End");
                    break;

                case "2":
                case "3":
                case "4":
                case "5":
                    Console.WriteLine($"Mode {mode} not implemented yet.");
                    break;

                default:
                    Console.WriteLine($"Unknown mode: {mode}");
                    break;
            }
        }
    }
}
