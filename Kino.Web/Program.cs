using Kino.Application;
using Kino.Infrastructure;
using Kino.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// MVC + локалізація інтерфейсу (JSON-переклади, Razor-в'юхи, валідація)
builder.Services.AddApplicationLocalization();

// ---- Composition Root: єдине місце, де Web знає про Infrastructure ----
var webRootPath = builder.Environment.WebRootPath
    ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, webRootPath);
// -----------------------------------------------------------------------

var app = builder.Build();

app.Services.InitializeDatabase();
app.UseApplicationLocalization();
app.UseApplicationErrorHandling();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

app.Run();
