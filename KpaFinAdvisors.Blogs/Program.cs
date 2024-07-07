using KpaFinAdvisors.Common;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var sharedConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "KpaFinAdvisors.Common");

builder.Configuration
    .SetBasePath(sharedConfigPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Load environment-specific configuration based on build configuration
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

builder.Services.AddDbContext<KpaFinAdvisorsDbContext>(item => item.UseSqlServer(builder.Configuration.GetConnectionString("calendar_main_connection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Blogs}/{action=Index}/{id?}");

app.Run();