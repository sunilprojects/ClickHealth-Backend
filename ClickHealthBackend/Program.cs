using ClickHealthBackend.Data;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Implementations;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Implementation;
using ClickHealthBackend.Services.Implementations;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------------------
// 1️⃣ Web Host URLs
// --------------------------------------------------------------------
builder.WebHost.UseUrls("https://localhost:7286", "http://localhost:5074");

// --------------------------------------------------------------------
// 2️⃣ Core Services
// --------------------------------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------------------------------------------------------
// 3️⃣ MongoDB Setup
// --------------------------------------------------------------------
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddSingleton<MongoDbContext>();

// --------------------------------------------------------------------
// 4️⃣ SMTP / Email
// --------------------------------------------------------------------
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// --------------------------------------------------------------------
// 5️⃣ Repositories & Services
// --------------------------------------------------------------------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IContentRepository, ContentRepository>();
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IContentRepository, ContentRepository>();
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();
builder.Services.AddScoped<ICampaignMetricsService, CampaignMetricsService>();
builder.Services.AddScoped<IHCPRepository, HCPRepository>();
builder.Services.AddScoped<IContentMetricsService, ContentMetricsService>();
builder.Services.AddScoped<IMRActivityRepository, MRActivityRepository>();
builder.Services.AddScoped<IMRService, MRService>();
builder.Services.AddScoped<IPatientInviteRepository, PatientInviteRepository>();
builder.Services.AddScoped<IHCPActivityRepository, HCPActivityRepository>();
builder.Services.AddScoped<ICampaignDashboardRepository, CampaignDashboardRepository>();
builder.Services.AddScoped<ICampaignDashboardService, CampaignDashboardService>();
builder.Services.AddScoped<IHCPService, HCPService>();
// --------------------------------------------------------------------
// 6️⃣ CORS (Frontend allowed origins)
// --------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7071"

            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// --------------------------------------------------------------------
// 7️⃣ File Upload Config
// --------------------------------------------------------------------
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 600_000_000; // 600 MB
});

// --------------------------------------------------------------------
// 8️⃣ Session (needed for OAuth correlation cookies)
// --------------------------------------------------------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// --------------------------------------------------------------------
// 9️⃣ JWT + Google Authentication
// --------------------------------------------------------------------
var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSection);

var jwtSettings = jwtSection.Get<JwtSettings>();
if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
    throw new InvalidOperationException("Missing JwtSettings configuration in appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})

.AddCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.CallbackPath = "/api/auth/google-callback"; // Must match Google Console
    googleOptions.Scope.Add("profile");
    googleOptions.Scope.Add("email");
    googleOptions.SaveTokens = true;

    // Fix “state missing or invalid”
    googleOptions.CorrelationCookie.SameSite = SameSiteMode.None;
    googleOptions.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

    googleOptions.Events.OnRedirectToAuthorizationEndpoint = context =>
    {
        Console.WriteLine("➡️ Redirecting to Google: " + context.RedirectUri);
        return Task.CompletedTask;
    };

    googleOptions.Events.OnRemoteFailure = context =>
    {
        Console.WriteLine("❌ Google OAuth Error: " + context.Failure?.Message);
        context.Response.Redirect("/api/auth/error?reason=" +
            Uri.EscapeDataString(context.Failure?.Message ?? "Unknown error"));
        context.HandleResponse();
        return Task.CompletedTask;
    };
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

builder.Services.AddAuthorization();

// --------------------------------------------------------------------
// 🔟 Build App
// --------------------------------------------------------------------
var app = builder.Build();

// --------------------------------------------------------------------
// 1️⃣1️⃣ Middleware Order (critical for OAuth)
// --------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
});

app.UseStaticFiles();

app.UseRouting();

// ✅ MUST COME BEFORE Authentication (so correlation cookie persists)
app.UseCookiePolicy();
app.UseSession();

// ✅ Now apply CORS and Auth
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --------------------------------------------------------------------
// 1️⃣2️⃣ Seed Database
// --------------------------------------------------------------------
await SeedAdminUserAsync(app);
CreateCollectionsIfNotExists(app);

app.Run();

// --------------------------------------------------------------------
// 1️⃣3️⃣ Helper Methods
// --------------------------------------------------------------------
static async Task SeedAdminUserAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    var users = context.Users;

    var existingAdmin = await users.Find(u => u.Email == "sunilofficial781@gmail.com" && u.Role == UserRole.Admin)
                                  .FirstOrDefaultAsync();

    if (existingAdmin == null)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword("admin@123");
        var adminUser = new User
        {
            Email = "sunilofficial781@gmail.com",
            Name = "Admin",
            Role = UserRole.Admin,
            Phone = "9008284717",
            Specialty = "Admin",
            Territory = "Global",
            IsActive = true,
            IsApproved = true,
            Status = UserStatus.Approved,
            PreferredLanguage = "English",
            CreatedAt = DateTime.UtcNow,
            Password = hashedPassword,
            MustResetPassword = false
        };

        await users.InsertOneAsync(adminUser);
        Console.WriteLine("✅ Default Admin created: sunilofficial781@gmail.com / admin@123");
    }
}


static void CreateCollectionsIfNotExists(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MongoDbContext>().Database;

    string[] collections = { "Users", "Campaigns", "Contents", "AuditLog" };
    foreach (var name in collections)
    {
        if (!db.ListCollectionNames().ToList().Contains(name))
        {
            db.CreateCollection(name);
            Console.WriteLine($"✅ Created collection: {name}");
        }
    }
}
