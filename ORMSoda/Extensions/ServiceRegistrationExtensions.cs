using Microsoft.Extensions.DependencyInjection;

namespace ORMSoda.Extensions
{
    public static partial class ServiceRegistrationExtensions
    {
        static partial void RegisterORMSodaDependencies(this IServiceCollection services);
    }
}