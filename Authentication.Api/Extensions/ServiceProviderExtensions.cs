using Authentication.Common;
using Authentication.Data;
using Authentication.Data.Abstracts;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Api.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task CreateDatabaseIfNotExists(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AuthenticationDbContext>();
                var userManager = services.GetService<UserManager<User>>();
                var roleManager = services.GetService<RoleManager<Role>>();
                var unitOfWork = services.GetRequiredService<IAuthenticationUnitOfWork>();

                await DataInitializer.Initialize(context, userManager, roleManager, unitOfWork);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
