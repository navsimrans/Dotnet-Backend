using Microsoft.EntityFrameworkCore;
using MyApi;                 // adjust to where BooksDbContext lives
using MyApi.Services;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ── Logging ───────────────────────────────────────────────────────────────
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ── JWT setup (DEMO: hardcoded key; prefer appsettings.json in real apps) ─
var keyBytes = Encoding.UTF8.GetBytes("u3jd83jsl9dk2039dkfms9f02kd9djsl394jf92msl93kd02js93kf9fsl20dkfj"); // ≥ 32 chars
// (Optional) sanity log: Console.WriteLine($"[JWT] Key length bytes: {keyBytes.Length}");

// ── AuthN/AuthZ ────────────────────────────────────────────────────────────
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
{
    options.IncludeErrorDetails = true; // show detailed reasons

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(keyBytes), // your bytes
        ValidateIssuer           = false,   // you can turn these on later
        ValidateAudience         = false,
        ValidateLifetime         = true,
        ClockSkew                = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = ctx =>
        {
            Console.WriteLine("[JWT] Auth failed: " + ctx.Exception.GetType().Name + " - " + ctx.Exception.Message);
            return Task.CompletedTask;
        },
        OnMessageReceived = ctx =>
        {
            Console.WriteLine("[JWT] Authorization header: " + ctx.Request.Headers["Authorization"]);
            return Task.CompletedTask;
        },
        OnTokenValidated = ctx =>
        {
            Console.WriteLine("[JWT] Token validated for: " + ctx.Principal?.Identity?.Name);
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddAuthorization();   // optional but recommended

// ── EF Core / DI / Swagger ────────────────────────────────────────────────
builder.Services.AddDbContext<BooksDbContext>(options =>
    options.UseSqlite("Data Source=books.db"));

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ── Pipeline order matters ────────────────────────────────────────────────
// 1) Global error handler FIRST
app.UseMiddleware<ErrorHandlerMiddleware>();

// 2) Swagger (dev only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3) AuthN then AuthZ
app.UseAuthentication();
app.UseAuthorization();

// 4) Endpoints last
app.MapControllers();

app.Run();
