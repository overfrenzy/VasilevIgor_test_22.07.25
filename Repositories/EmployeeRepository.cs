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
    }
}
