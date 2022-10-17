using Authentication.Api.Configurations;
using Authentication.Api.Extensions;
using Authentication.Common;
using Authentication.Data;
using Authentication.Data.Abstracts;
using Authentication.Data.Stores;
using Authentication.Services;
using Authentication.Services.Abstracts;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;


services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auth.API", 
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "SintoGenroku",
            Email = "borozda.a.s@gmail.com",
            Url = new Uri("https://github.com/SintoGenroku")
        }
    });
                
    var filePath = Path.Combine(AppContext.BaseDirectory, "Authentication.Api.xml");

    options.IncludeXmlComments(filePath);
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

services.AddDbContext<AuthenticationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

services.AddScoped<IAuthenticationUnitOfWork, AuthenticationUnitOfWork>();
services.AddScoped<IAuthenticationService, AuthenticationService>();
services.AddScoped<IUserService, UserService>();


services.AddAutoMapper(configuration => { configuration.AddMaps(typeof(Program).Assembly); });

services.AddIdentityCore<User>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddPasswordValidator<PasswordValidator<User>>()
    .AddSignInManager()
    .AddUserStore<UserStore>()
    .AddRoles<Role>()
    .AddRoleStore<RoleStore>()
    .AddRoleManager<RoleManager<Role>>()
    .AddUserManager<UserManager<User>>();

    services.AddIdentityServer()
    .AddInMemoryApiResources(Configuration.ApiResources)
    .AddInMemoryIdentityResources(Configuration.IdentityResources)
    .AddInMemoryApiScopes(Configuration.ApiScopes)
    .AddInMemoryClients(Configuration.Clients)
    .AddDeveloperSigningCredential()
    .AddAspNetIdentity<User>();

builder.Services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(options =>
    {
        options.Authority = "https://authentication-api";
    });


var app = builder.Build();

app.UseCustomExceptionHandler();
await app.Services.CreateDatabaseIfNotExists();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Authentication.API v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.UseIdentityServer();

app.MapControllers();

app.Run();