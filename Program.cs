using LiveStore.Data;
using LiveStore.Repositories;
using LiveStore.Repositories.Interfaces;
using LiveStore.Services;
using LiveStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// EF — SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositórios
builder.Services.AddScoped<ILiveRepository,    LiveRepository>();
builder.Services.AddScoped<IVendaRepository,   VendaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

// Serviços
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ILiveService,      LiveService>();
builder.Services.AddScoped<IVendaService,     VendaService>();
builder.Services.AddScoped<IProdutoService,   ProdutoService>();
builder.Services.AddScoped<IClienteService,   ClienteService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
