using ClickHealthBackend.Data;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Implementations;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Implementations;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using OfficeOpenXml;
using System.Text;
using System.Text.Json.Serialization;

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var builder = WebApplication.CreateBuilder(args);

// Backend must ONLY run on localhost:7286
builder.WebHost.UseUrls("https://localhost:7286");

// ------------------ Controllers ------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------ MongoDB ------------------
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});
builder.Services.AddSingleton<MongoDbContext>();

// ------------------ Services ------------------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IContentRepository, ContentRepository>();
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
builder.Services.AddScoped<ICampaignMetricsService, CampaignMetricsService>();
builder.Services.AddScoped<IContentMetricsService, ContentMetricsService>();
builder.Services.AddScoped<ICampaignDashboardRepository, CampaignDashboardRepository>();
builder.Services.AddScoped<ICampaignDashboardService, CampaignDashboardService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IPatientInviteRepository, PatientInviteRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IHCPRepository, HCPRepository>();
builder.Services.AddScoped<IHCPService, HCPService>();
builder.Services.AddScoped<IPatientInviteService, PatientInviteService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

// ------------------ CORS ------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7071")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ------------------ File Upload ------------------
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 600_000_000; // ~600MB
});

// ------------------ Session ------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ------------------ Authentication ------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Default for cookies
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Needed for Google OAuth
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Redirect to Google on challenge
})
.AddCookie() // Cookie storage for Google OAuth
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// ------------------ Build App ------------------
var app = builder.Build();

// ------------------ Middleware ------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCors("AllowFrontend");
app.UseStaticFiles();
app.UseRouting();

app.UseCookiePolicy();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ------------------ DB Setup ------------------
await SeedAdminUserAsync(app);
await CreateCollectionsIfNotExistsAsync(app);

app.Run();

// ------------------ Helpers ------------------
static async Task SeedAdminUserAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    var users = context.Users;

    var existing = await users.Find(u => u.Email == "sunilofficial781@gmail.com" && u.Role == UserRole.Admin)
                              .FirstOrDefaultAsync();

    if (existing == null)
    {
        await users.InsertOneAsync(new User
        {
            Email = "sunilofficial781@gmail.com",
            Name = "Admin",
            Role = UserRole.Admin,
            IsApproved = true,
            IsActive = true,
            Password = BCrypt.Net.BCrypt.HashPassword("admin@123"),
            CreatedAt = DateTime.UtcNow
        });

        Console.WriteLine("✔ Default admin created");
    }
}

static async Task CreateCollectionsIfNotExistsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MongoDbContext>().Database;

    string[] collections = { "Users", "Campaigns", "Contents", "Patients", "PatientInvites", "AuditLog" };
    var existing = await db.ListCollectionNames().ToListAsync();

    foreach (var name in collections)
    {
        if (!existing.Contains(name))
        {
            await db.CreateCollectionAsync(name);
            Console.WriteLine($"✔ Collection created: {name}");
        }
    }
}
