using System.Data;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISqlExecutor
    {
        Task<int> ExecuteAsync(
            string sql,
            object param,
            IDbTransaction transaction);
    }
}
