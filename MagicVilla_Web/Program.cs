using MagicVilla_Web;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(MappingConfig));


//service HttpCliente
builder.Services.AddHttpClient<IVillaService, VillaService>();
builder.Services.AddHttpClient<INumeroVillaService, NumeroVillaService>();
builder.Services.AddHttpClient<IUsuarioService, UsuarioService>();

//service Scoped
builder.Services.AddScoped<IVillaService, VillaService>();
builder.Services.AddScoped<INumeroVillaService, NumeroVillaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();


//para almacenar token tenemos q usar session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options=>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                                   .AddCookie(option =>
                                   {
                                       option.Cookie.HttpOnly = true;
                                       option.ExpireTimeSpan = TimeSpan.FromMinutes(100);
                                       option.LoginPath = "/Usuario/Login";
                                       option.AccessDeniedPath = "/Usuario/AccesoDenegado";
                                       option.SlidingExpiration = true;
                                   });


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
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
