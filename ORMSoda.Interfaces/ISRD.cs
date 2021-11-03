using System.Collections.Generic;
using System.Threading.Tasks;

namespace ORMSoda.Interfaces
{
    public interface ISRD<T>
        where T : class
    {
        Task Save(T request);
        Task Save(IEnumerable<T> items);
        Task<T> Read(int id);
        Task Delete(int id);
        Task Delete(IEnumerable<T> input);
    }
}