using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Web_Turismo_Triunvirato.DataAccess;
using Web_Turismo_Triunvirato.Services; // Asegúrate de tener este using para IPromotionService y InMemoryPromotionService


var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// Configuración de la autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // La ruta a tu página de inicio de sesión
        options.LogoutPath = "/Account/Logout"; // La ruta a tu acción de cierre de sesión
        options.AccessDeniedPath = "/Account/AccessDenied"; // Ruta para cuando el acceso es denegado
    });

// Registro de tus servicios personalizados
// Registrar la interfaz y la implementación del servicio de correo
builder.Services.AddTransient<IEmailService, EmailService>();

// También necesitas la configuración para que el servicio pueda acceder a ella
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Registro del servicio de usuarios
builder.Services.AddScoped<IUserService, InMemoryUserService>();
// *** AGREGAR ESTA LÍNEA PARA EL SERVICIO DE PROMOCIONES ***
builder.Services.AddScoped<IPromotionService, InMemoryPromotionService>(); // <--- ¡Esta es la línea que faltaba!

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

// ¡Asegúrate de que UseAuthentication esté antes de UseAuthorization!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();