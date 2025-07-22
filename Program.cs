using System.Diagnostics;
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

            var connString = BuildConnStringFromEnv();
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
                    Console.WriteLine("Mode 3: Listing all employees");
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
                    Console.WriteLine("Mode 4: Bulk‐insert 1,000,100 employees…");
                    repo.BulkInsert(GenerateEmployees());
                    Console.WriteLine("Bulk insert complete.");
                    break;

                case "5":
                    Console.WriteLine("Mode 5: Query male employees with surnames starting 'F'…");
                    var sw = Stopwatch.StartNew();
                    var maleFs = repo.GetMaleF();
                    sw.Stop();

                    foreach (var e in maleFs)
                    {
                        var fio = e.MiddleName is null
                            ? $"{e.LastName} {e.FirstName}"
                            : $"{e.LastName} {e.FirstName} {e.MiddleName}";
                        Console.WriteLine(
                            $"{fio}\t{e.BirthDate:yyyy-MM-dd}\t{e.Gender}\tAge={e.Age}"
                        );
                    }

                    Console.WriteLine(
                        $"Found {maleFs.Count} records in {sw.Elapsed.TotalMilliseconds:N0} ms."
                    );
                    break;

                default:
                    Console.WriteLine($"Unknown mode: {mode}");
                    break;
            }
        }

        static IEnumerable<Employee> GenerateEmployees()
        {
            // small pool of names
            //(used Bogus library before that but there were issues with seeding too large of a dataset, using russian names, can instead load from small file or expand the pool)
            string[] surnames =
            {
                "Ivanov",
                "Petrov",
                "Sidorov",
                "Smirnov",
                "Kuznetsov",
                "Popov",
                "Sokolov",
                "Mikhailov",
                "Andreev",
                "Novikov",
                "Fedorov",
                "Frolov",
                "Filippov",
                "Lebedev",
                "Morozov",
            };
            string[] maleFirstNames = { "Ivan", "Petr", "Dmitry", "Alexey", "Nikolay" };
            string[] femaleFirstNames = { "Anna", "Olga", "Maria", "Elena", "Svetlana" };
            string[] patronymicRoots = maleFirstNames;

            var startDate = new DateTime(1950, 1, 1);
            int totalDays = (DateTime.Today - startDate).Days + 1;

            int maleIdx = 0,
                femaleIdx = 0;

            for (int i = 0; i < 1_000_000; i++)
            {
                bool isMale = (i % 2 == 0);
                var baseSur = surnames[i % surnames.Length];
                var lastName = isMale ? baseSur : baseSur + "a";

                string firstName = isMale
                    ? maleFirstNames[maleIdx++ % maleFirstNames.Length]
                    : femaleFirstNames[femaleIdx++ % femaleFirstNames.Length];

                int rootIdx = (i / 2) % patronymicRoots.Length;
                var root = patronymicRoots[rootIdx];
                var middle = isMale ? root + "ovich" : root + "ovna";

                var dob = startDate.AddDays(i % totalDays);

                yield return new Employee
                {
                    LastName = $"{lastName}{i:D7}", // Ivanov0000123 - this is done so LastName, FirstName, MiddleName, BirthDate combination is unique (Not relevant unless we have 1'000'000 entries using something like Bogus, in other case we can drop the unique index)
                    FirstName = firstName,
                    MiddleName = middle,
                    BirthDate = dob,
                    Gender = isMale ? 'M' : 'F',
                };
            }

            // 100 extras: all male, surnames starting with 'F'
            int extras = 0,
                j = 0;
            while (extras < 100)
            {
                var sur = surnames[j % surnames.Length];
                if (!sur.StartsWith("F"))
                {
                    j++;
                    continue;
                }

                var fn = maleFirstNames[j % maleFirstNames.Length];
                var pr = patronymicRoots[j % patronymicRoots.Length] + "ovich";
                var dob = startDate.AddDays(j % totalDays);

                yield return new Employee
                {
                    LastName = $"{sur}{j:D4}",
                    FirstName = fn,
                    MiddleName = pr,
                    BirthDate = dob,
                    Gender = 'M',
                };

                extras++;
                j++;
            }
        }

        static string BuildConnStringFromEnv()
        {
            var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            var user = Environment.GetEnvironmentVariable("DB_USER") ?? "";
            var pass = Environment.GetEnvironmentVariable("DB_PASS") ?? "";
            var db = Environment.GetEnvironmentVariable("DB_NAME") ?? "";
            return $"Host={host};Port={port};Username={user};Password={pass};Database={db}";
        }
    }
}
