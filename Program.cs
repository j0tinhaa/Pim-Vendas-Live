using LiveStore.Data;
using LiveStore.Repositories;
using LiveStore.Repositories.Interfaces;
using LiveStore.Services;
using LiveStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// EF — SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Cookie Authentication (sem Identity) ─────────────────────────────────────
builder.Services.AddAuthentication("LiveStoreCookie")
    .AddCookie("LiveStoreCookie", options =>
    {
        options.LoginPath         = "/Login";
        options.LogoutPath        = "/Login/Logout";
        options.AccessDeniedPath  = "/Login";
        options.ExpireTimeSpan    = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name       = "LiveStore.Auth";
    });

// ── Repositórios ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<ILiveRepository,    LiveRepository>();
builder.Services.AddScoped<IVendaRepository,   VendaRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IGastoRepository,   GastoRepository>();

// ── Serviços ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ILiveService,      LiveService>();
builder.Services.AddScoped<IVendaService,     VendaService>();
builder.Services.AddScoped<IClienteService,   ClienteService>();
builder.Services.AddScoped<IGastoService,     GastoService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();
builder.Services.AddScoped<IWhatsAppService,  MockWhatsAppService>();

var app = builder.Build();

// ── Mostra o erro detalhado mesmo em produção temporariamente para debug ──
app.UseDeveloperExceptionPage();

// ── Seed de Dados Automático ──
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        LiveStore.Data.DbInitializer.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Um erro ocorreu ao recriar e popular o banco de dados.");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ORDEM OBRIGATÓRIA: Authentication ANTES de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
