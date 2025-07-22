using Npgsql;
using NpgsqlTypes;

namespace EmployeeDirectory
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateEmployeeTable()
        {
            var sql =
                @"
                CREATE TABLE IF NOT EXISTS employees (
                    id           SERIAL PRIMARY KEY,
                    last_name    TEXT    NOT NULL,
                    first_name   TEXT    NOT NULL,
                    middle_name  TEXT,
                    birth_date   DATE    NOT NULL,
                    gender       CHAR(1) NOT NULL
                );
            ";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        public void Insert(Employee e)
        {
            const string sql =
                @"
                INSERT INTO employees
                (""LastName"", ""FirstName"", ""MiddleName"", ""BirthDate"", ""Gender"")
                VALUES
                (@ln, @fn, @mn, @bd, @g);
                ";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("ln", e.LastName);
            cmd.Parameters.AddWithValue("fn", e.FirstName);
            cmd.Parameters.AddWithValue("mn", (object?)e.MiddleName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("bd", e.BirthDate);
            cmd.Parameters.AddWithValue("g", e.Gender.ToString());

            cmd.ExecuteNonQuery();
        }

        public List<Employee> GetAll()
        {
            const string sql =
                @"
                SELECT 
                  ""LastName"", ""FirstName"", ""MiddleName"", ""BirthDate"", ""Gender""
                FROM ""employees""
                ORDER BY ""LastName"", ""FirstName"", ""MiddleName"", ""BirthDate"";
                ";
            var list = new List<Employee>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(
                    new Employee
                    {
                        LastName = reader.GetString(0),
                        FirstName = reader.GetString(1),
                        MiddleName = reader.IsDBNull(2) ? null : reader.GetString(2),
                        BirthDate = reader.GetDateTime(3),
                        Gender = reader.GetString(4)[0],
                    }
                );
            }
            return list;
        }

        public void BulkInsert(IEnumerable<Employee> employees)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            // using Npgsql’s binary COPY instead of individual INSERT
            using var importer = conn.BeginBinaryImport(
                @"
                COPY ""employees"" (""LastName"", ""FirstName"", ""MiddleName"", ""BirthDate"", ""Gender"")
                FROM STDIN (FORMAT BINARY)
                "
            );

            foreach (var e in employees)
            {
                importer.StartRow();
                importer.Write(e.LastName, NpgsqlDbType.Text);
                importer.Write(e.FirstName, NpgsqlDbType.Text);
                if (e.MiddleName is null)
                    importer.WriteNull();
                else
                    importer.Write(e.MiddleName, NpgsqlDbType.Text);
                importer.Write(e.BirthDate, NpgsqlDbType.Date);
                importer.Write(e.Gender.ToString(), NpgsqlDbType.Char);
            }

            importer.Complete();
        }
    }
}
