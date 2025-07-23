using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using MerhabaDunyaApi.Data;
using MerhabaDunyaApi.Models;
using MerhabaDunyaApi.Services;
using MerhabaDunyaApi.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// 1) Swagger/OpenAPI ve Controller altyapısı
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
// EF Core: SQL Server bağlantısı
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Routing Motoru için HttpClient (OSRM örneği)
builder.Services.AddHttpClient("routing", c =>
{
    c.BaseAddress = new Uri("https://router.project-osrm.org/");
});

// Emisyon Hesaplayıcı servisi (DI)
builder.Services.AddScoped<IEmissionCalculator, EmissionCalculator>();
builder.Services.AddScoped<IBadgeService, BadgeService>();
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Geliştirme ortamında Swagger UI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// Auth servisini ekleyin


// Statik dosya servisi (wwwroot) ve default dosya bulunması:
app.UseDefaultFiles();   // ← burada index.html, default.html vs. aranır

// HTTP→HTTPS yönlendirmeyi kapatıyoruz (yalnızca HTTP kullanımı)
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// Controller rotalarını eşliyoruz
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
// NotificationHub'ı eşliyoruz
app.MapHub<NotificationHub>("/notificationHub");
// Demo amaçlı WeatherForecast endpoint’i
var summaries = new[]
{
    "Freezing","Bracing","Chilly","Cool","Mild",
    "Warm","Balmy","Hot","Sweltering","Scorching"
};
app.MapGet("/weatherforecast", () =>
{
    return Enumerable.Range(1, 5).Select(i =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        )
    ).ToArray();
})
.WithName("GetWeatherForecast");

// 9) Uygulama başında Emisyon Faktörlerini seed et
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.EmissionFactors.Any())
    {
        db.EmissionFactors.AddRange(
            new EmissionFactor { YakitTipi = "Diesel", KgCO2PerLitre = 0.265M },
            new EmissionFactor { YakitTipi = "Benzin", KgCO2PerLitre = 0.239M },
            new EmissionFactor { YakitTipi = "Elektrik", KgCO2PerLitre = 0.000M }
        );
        db.SaveChanges();
    }
}

app.Run();

// Küçük bir record tipi demo amaçlı
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
