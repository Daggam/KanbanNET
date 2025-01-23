using tl2_proyecto_2024_Daggam.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDataProtection(o =>{
    o.ApplicationDiscriminator="";
}); //En produccion agregar donde se almacenarán las keys.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRepositorioUsuarios,RepositorioUsuarios>();
builder.Services.AddTransient<IRepositorioTableros,RepositorioTablero>();
builder.Services.AddTransient<IRepositorioTareas,RepositorioTareas>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name="AuthCookie";
    options.IdleTimeout = TimeSpan.FromHours(3);
    options.Cookie.HttpOnly=true;
    options.Cookie.IsEssential=true;
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

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
