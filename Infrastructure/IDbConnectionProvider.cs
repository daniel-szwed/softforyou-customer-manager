using System.Data;

namespace Infrastructure
{
    public interface IDbConnectionProvider
    {
        IDbConnection GetDbConnection();
    }
}
