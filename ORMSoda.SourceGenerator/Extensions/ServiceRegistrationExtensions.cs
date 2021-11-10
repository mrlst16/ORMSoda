using Microsoft.Extensions.DependencyInjection;

namespace ORMSoda.SourceGenerator.Extensions
{
    public static partial class ServiceRegistrationExtensions
    {
        static partial void RegisterORMSodaDependencies(this IServiceCollection services);
        public static void RegisterORMSoda(this IServiceCollection services)
            => RegisterORMSodaDependencies(services);
    }
}