using System.Data;

namespace Domain.Interfaces
{
    public interface IDbConnectionProvider
    {
        IDbConnection GetDbConnection();
    }
}
