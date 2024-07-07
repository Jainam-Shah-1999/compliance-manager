using KpaFinAdvisors.Common;
using KpaFinAdvisors.Common.Services.Interfaces;
using KpaFinAdvisors.Common.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Load base configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Load environment-specific configuration based on build configuration
#if DEBUG
builder.Configuration
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
#else
builder.Configuration
    .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);
#endif

// Add session and distributed cache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // Session timeout duration
});
builder.Services.AddDistributedMemoryCache();

// Add DbContext
builder.Services.AddDbContext<KpaFinAdvisorsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("calendar_main_connection")));

// Configure JWT authentication
var key = Encoding.ASCII.GetBytes("YourVeryLongSecureSecretKeyHere1234567890");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "JainamShah", // Replace with your issuer
            ValidAudience = "KPAFinAdvisors", // Replace with your audience
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                // Log claims here to check if they are correctly retrieved
                var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    foreach (var claim in claimsIdentity.Claims)
                    {
                        Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
                    }
                    return Task.CompletedTask;
                }
                else
                {
                    Console.WriteLine("No claims found");
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                }
            },
            OnAuthenticationFailed = context =>
            {
                // Log authentication failures if any
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnMessageReceived = async context =>
            {
                var token = context.Request.Cookies["AuthToken"];
                if (token != null)
                {
                    IDistributedCache cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
                    var isBlacklisted = await cache.GetStringAsync(token);
                    if (isBlacklisted != null)
                    {
                        context.Response.StatusCode = 401; // Unauthorized
                        context.Response.Headers.Remove("Authorization");
                        context.Response.Cookies.Delete("AuthToken");
                        return;
                    }
                }
                context.Token = token;
            }
        };

        //options.Events = new JwtBearerEvents
        //{
        //    OnMessageReceived = async context =>
        //    {
        //        var token = context.Request.Cookies["AuthToken"];
        //        if (token != null)
        //        {
        //            IDistributedCache cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
        //            var isBlacklisted = await cache.GetStringAsync(token);
        //            if (isBlacklisted != null)
        //            {
        //                context.Response.StatusCode = 401; // Unauthorized
        //                context.Request.Headers.Remove("Authorization");
        //                context.Request.Cookies.Keys.Remove("AuthToken");
        //                context.Response.Headers.Remove("Authorization");
        //                context.Response.Cookies.Delete("AuthToken");
        //                return;
        //            }
        //        }
        //        context.Token = token;
        //    }
        //};
    });

// Add other services to the container
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
