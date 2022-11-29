using Authentication.Api.Configurations;
using Authentication.Api.Consumers;
using Authentication.Api.Extensions;
using Authentication.Api.Validators;
using Authentication.Common;
using Authentication.Data;
using Authentication.Data.Abstracts;
using Authentication.Data.Stores;
using Authentication.Services;
using Authentication.Services.Abstracts;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RentRide.AuthenticationApi.Models;
using Serilog;
using Serilog.Sinks.Logz.Io;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;


services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddControllersWithViews();

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

services.AddMassTransit(c =>
{
    c.AddConsumer<UserConsumer>();
    
    c.UsingRabbitMq((context, config) =>
    {
        config.ReceiveEndpoint("user-auth-queue", e =>
        {
            e.Bind<UserQueue>();
            e.ConfigureConsumer<UserConsumer>(context);
        });
    });
});

services.AddIdentityCore<User>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddPasswordValidator<PasswordValidator<User>>()
    .AddSignInManager()
    .AddUserStore<UserStore>()
    .AddRoles<Role>()
    .AddRoleStore<RoleStore>()
    .AddRoleManager<RoleManager<Role>>()
    .AddUserManager<UserManager<User>>();

services.AddIdentityServer(options =>
    {
        options.EmitStaticAudienceClaim = true;
    })
    .AddAspNetIdentity<User>()
    .AddInMemoryApiResources(Configuration.ApiResources)
    .AddInMemoryIdentityResources(Configuration.IdentityResources)
    .AddInMemoryApiScopes(Configuration.ApiScopes)
    .AddInMemoryClients(Configuration.Clients)
    .AddDeveloperSigningCredential()
    .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();

services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Authentication/Login";
    options.LogoutPath = "/Authentication/Logout";
});

services.AddAuthConfiguration();

var logger = new LoggerConfiguration()
    .WriteTo.LogzIoDurableHttp(
        "https://listener.logz.io:8071/?type=<string>&token=NLKhSzkrJgYczQDwPtnEFSOrpfFVKRRn",
        logzioTextFormatterOptions: new LogzioTextFormatterOptions
        {
            BoostProperties = true,
            LowercaseLevel = true,
            IncludeMessageTemplate = true,
            FieldNaming = LogzIoTextFormatterFieldNaming.CamelCase,
            EventSizeLimitBytes = 261120,
        })
    .MinimumLevel.Verbose()
    .CreateLogger();

var app = builder.Build();

app.UseCustomExceptionHandler();
await app.Services.CreateDatabaseIfNotExists();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "Authentication.API v1"));
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseIdentityServer();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Login}");

app.Run();