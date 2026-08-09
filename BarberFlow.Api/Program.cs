using BarberFlow.Api.Extensions;
using BarberFlow.Application.Features.Users.CreateUser;
using BarberFlow.Infrastructure;
using BarberFlow.Infrastructure.Common.Configurations;
using BarberFlow.Infrastructure.Persistence;
using BarberFlow.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2 * 1024 * 1024;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerDocumentation();

builder.Services
    .AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
    .ValidateDataAnnotations() // opcional pero recomendado
    .ValidateOnStart(); // 🔥 falla al iniciar si está mal

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("OtpPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "global",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5, // Máximo 5 solicitudes
                Window = TimeSpan.FromMinutes(1), // Por minuto
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            }));
});

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly);
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BarberFlowDbContext>();

    await context.Database.MigrateAsync();

    await DatabaseSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseGlobalExceptionHandling();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
