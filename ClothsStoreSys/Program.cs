using ClothsStoreSys.Data;
using ClothsStoreSys.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Configure DbContext (SQL Server Express). Connection string is in appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? @"Server=.\sqlexpress;Database=ClothsStoreSysDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Authorization
builder.Services.AddAuthorizationCore();

// Authentication state and auth service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());

// Application services
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IReturnService, ReturnService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IUserService, UserService>();

// Current invoice state per circuit/session
builder.Services.AddScoped<CurrentInvoiceService>();

var app = builder.Build();

// Configure supported cultures and default culture (ar-EG)
var supportedCultures = new[] { new CultureInfo("ar-EG"), new CultureInfo("en-US") };
var requestLocalizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ar-EG"),
    SupportedCultures = supportedCultures.ToList(),
    SupportedUICultures = supportedCultures.ToList()
};
app.UseRequestLocalization(requestLocalizationOptions);

// Initialize database (apply migrations/seed) - best-effort; if DB not available it will not crash startup.
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // Ensure database created. In production use migrations.
        db.Database.EnsureCreated();
        DbInitializer.Seed(db);
    }
    catch
    {
        // Swallow exceptions to avoid startup failure in development without SQL Server
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
