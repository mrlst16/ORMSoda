using System.Threading.Tasks;

namespace ORMSoda.Interfaces
{
    public interface ISprocCall<TRequest, TResponse>
    {
        Task<TResponse> Call(TRequest request);
    }
}