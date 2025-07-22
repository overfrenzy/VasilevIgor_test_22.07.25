using DotNetEnv;

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
            var repo = new EmployeeRepository(connString);

            switch (mode)
            {
                case "1":
                    Console.WriteLine("Mode 1: Creating employees table");
                    repo.CreateEmployeeTable();
                    Console.WriteLine("End");
                    break;

                case "2":
                    if (args.Length != 4)
                    {
                        Console.WriteLine(
                            "Usage: dotnet run -- 2 \"Last First Middle\" YYYY-MM-DD [M|F]"
                        );
                        return;
                    }

                    var nameParts = args[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (nameParts.Length < 2)
                    {
                        Console.WriteLine("Please provide at least LastName and FirstName.");
                        return;
                    }

                    var employee = new Employee
                    {
                        LastName = nameParts[0],
                        FirstName = nameParts[1],
                        MiddleName = nameParts.Length > 2 ? nameParts[2] : null,
                        BirthDate = DateTime.Parse(args[2]),
                        Gender = char.ToUpper(args[3][0]),
                    };

                    repo.Insert(employee);
                    Console.WriteLine(
                        $"Inserted: {employee.LastName} {employee.FirstName} (Age: {employee.Age})"
                    );
                    break;

                case "3":
                    Console.WriteLine("Mode 3: Listing all employees");
                    var all = repo.GetAll();
                    foreach (var e in all)
                    {
                        var fio = e.MiddleName is null
                            ? $"{e.LastName} {e.FirstName}"
                            : $"{e.LastName} {e.FirstName} {e.MiddleName}";
                        Console.WriteLine(
                            $"{fio}\t{e.BirthDate:yyyy-MM-dd}\t{e.Gender}\tAge={e.Age}"
                        );
                    }
                    break;
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
