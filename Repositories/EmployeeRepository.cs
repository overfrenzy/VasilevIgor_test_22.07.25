using Npgsql;

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
    }
}
