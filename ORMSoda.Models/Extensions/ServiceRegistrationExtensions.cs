using Microsoft.Extensions.DependencyInjection;

namespace ORMSoda.Models.Extensions
{
    public static partial class ServiceRegistrationExtensions
    {
        static partial void RegisterORMSodaDependencies(IServiceCollection services);
        public static void RegisterORMSoda(this IServiceCollection services)
        {
            RegisterORMSodaDependencies(services);
        }
    }
}