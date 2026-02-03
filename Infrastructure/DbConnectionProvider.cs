using System.Data;
using System.Data.SqlClient;

namespace Infrastructure
{
    public class DbConnectionProvider : IDbConnectionProvider
    {
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CustomerManagerDb;Integrated Security=True;";

        public IDbConnection GetDbConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
