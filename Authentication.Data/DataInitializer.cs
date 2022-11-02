using Authentication.Common;
using Authentication.Data.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data;

public class DataInitializer
{
    public static async Task Initialize(AuthenticationDbContext context, UserManager<User> userManager, 
                                            RoleManager<Role> roleManager, IAuthenticationUnitOfWork unitOfWork)
        {
            await context.Database.MigrateAsync();

            var users = new[]
            {
                new InitUser()
                {
                    Password = "1mBoss",
                    RoleNames = new[] {RoleNames.Admin},
                    Username = "Admin",
                    IsActive = true,
                },
                new InitUser()
                {
                    Password = "zxc1V1",
                    RoleNames = new[] {RoleNames.Client},
                    Username = "User",
                    IsActive = true,    
                }
            };

            await AddOrUpdateRolesAsync(users, roleManager);
            await AddOrUpdateUsersAsync(users, userManager);
        }

        private static async Task AddOrUpdateUsersAsync(IEnumerable<InitUser> users,
                                UserManager<User> userManager)
        {
            foreach (var user in users)
            {
                var registeredUser = await userManager.FindByNameAsync(user.Username);
                if (registeredUser == null)
                {
                    var newUser = new User()
                    {
                        Username = user.Username,
                        Roles = new List<Role>(),
                        IsActive = user.IsActive,

                    };
                    await userManager.CreateAsync(newUser, user.Password);

                    await userManager.AddToRolesAsync(newUser, user.RoleNames);
                }
                else
                {
                    var removedRoles = registeredUser.Roles
                        .Select(role => role.Name).Except(user.RoleNames);

                    await userManager.RemoveFromRolesAsync(registeredUser, removedRoles);

                    var addedRoles = user.RoleNames.Except(registeredUser.Roles.Select(role => role.Name));

                    await userManager.AddToRolesAsync(registeredUser, addedRoles);

                    await userManager.UpdateAsync(registeredUser);
                }
            }
        }

        private static async Task AddOrUpdateRolesAsync(IEnumerable<InitUser> users, RoleManager<Role> roleManager)
        {
            var rolesNames = users
                .SelectMany(user => user.RoleNames)
                .Distinct()
                .Select(role => role.ToUpper()).ToList();

            foreach (var roleName in rolesNames)
            {
                var existedRole = await roleManager.FindByNameAsync(roleName);
                if (existedRole == null)
                {
                    await roleManager.CreateAsync(new Role() { Name = roleName });
                }
                else
                {
                    await roleManager.UpdateAsync(existedRole);
                }
            }
        }
        
        public class InitUser
        {
            public string Username { get; set; }
    
            public string Password { get; init; }

            public string[] RoleNames { get; init; }

            public bool IsActive { get; set; }
        }
}