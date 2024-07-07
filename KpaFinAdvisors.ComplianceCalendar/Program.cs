using KpaFinAdvisors.Common;
using KpaFinAdvisors.Common.Services;
using KpaFinAdvisors.Common.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddDbContext<KpaFinAdvisorsDbContext>(item => item.UseSqlServer(builder.Configuration.GetConnectionString("calendar_main_connection"), b => b.MigrationsAssembly("KpaFinAdvisors.ComplianceCalendar")));
var key = Encoding.UTF8.GetBytes("YourVeryLongSecureSecretKeyHere1234567890");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
                    context.Request.Headers.Remove("Authorization");
                    context.Request.Cookies.Keys.Remove("AuthToken");
                    context.Response.Headers.Remove("Authorization");
                    context.Response.Cookies.Delete("AuthToken");
                    return;
                }
            }
            context.Token = token;
            return;
        }
    };
});
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
