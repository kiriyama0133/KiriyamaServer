using KiriyamaServer.Application.Services;
using KiriyamaServer.Infrastructure.HealthChecks;
using KiriyamaServer.Infrastructure.Security;
using KiriyamaServer.WebApi.ApiClient;
using KiriyamaServer.WebApi.Extensions;
using KiriyamaServer.WebApi.Extensions.FeatureFlags;
using KiriyamaServer.WebApi.Extensions.PostgreSQL;

using Genocs.Core.Builders;
using Genocs.Logging;
using Genocs.Telemetry;
using Genocs.WebApi;
using Genocs.WebApi.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Refit;
using Serilog;
using System.Text;

StaticLogger.EnsureInitialized();

var builder = WebApplication.CreateBuilder(args);

// 显式监听所有网卡（0.0.0.0），端口从配置读取、默认 5000。
// 默认的 Kestrel 只绑定 http://localhost:5000（回环），部署到 Linux 后
// 客户端通过 ZeroTier 虚拟 IP / 局域网 IP 访问会被拒之门外（表现为连接被重置）。
builder.WebHost.ConfigureKestrel(options =>
{
    int port = builder.Configuration.GetValue<int>("Kestrel:Port", 5000);
    options.ListenAnyIP(port);
});

builder.Host
        .UseLogging();

// Use Genocs Core Builders to register services and build the container
IGenocsBuilder genocsBuilder = builder
    .AddGenocs()
    .AddTelemetry()
    .AddWebApi()
    .AddOpenApiDocs();

var services = builder.Services;

// services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, _) =>
// {
//     module.IncludeDiagnosticSourceActivities.Add("MassTransit");
// });

services.AddControllers().AddControllersAsServices();

services.AddBusinessExceptionFilter();
services.AddFeatureFlags(builder.Configuration);
services.AddVersioning();

services.AddCustomHealthChecks(builder.Configuration);

services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(2);
    options.Predicate = check => check.Tags.Contains("ready");
});

// Setup Cors
services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins(
                            "https://localhost:5001",
                            "http://localhost:5000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
    });
});

// Setup Database
services.AddPostgreSQLPersistence(builder.Configuration);

// Setup the enterprise service bus library
services.AddRebusServiceBus(builder.Configuration);

services.AddUseCases();

services.AddPresentersV1();
services.AddPresentersV2();

// Setup the room lobby and ZeroTier Controller services.
services.AddRooms(builder.Configuration);

// Setup authentication (register/login/PKCE token exchange).
services.AddAuth(builder.Configuration);

// Setup JWT Bearer authentication (validate access tokens issued by the token endpoint).
var jwtOptions = builder.Configuration.GetSection(JwtOptions.Position).Get<JwtOptions>() ?? new JwtOptions();
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
services.AddAuthorization();

// refit apis
services.AddRefitClient<IOrderApi>()

  // .AddHttpMessageHandler<AuthorizationMessageHandler>()
  .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["ExternalWebServices:Order"]));

var app = builder.Build();

// Setup Database
app.Services.UsePostgreSQLPersistence();

genocsBuilder.Build(app.Services);

app.UseGenocs()
    .UseOpenApiDocs();

app.UseHttpsRedirection();

app.UseCookiePolicy();

// var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
// app.UseVersionedSwagger(provider);

app.MapControllers();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Ensure the ZeroTier network has room-isolation Tag definition and Flow Rules provisioned.
var zeroTierController = app.Services.GetRequiredService<IZeroTierController>();
await zeroTierController.EnsureNetworkAsync();

await app.RunAsync();

await Log.CloseAndFlushAsync();

// Make the implicit Program class public so test projects can access it
public partial class Program;
