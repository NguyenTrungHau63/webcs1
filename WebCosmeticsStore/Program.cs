using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebCosmeticsStore.Models;
using WebCosmeticsStore.Repositories;
using WebCosmeticsStore.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<User, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();


builder.Services.AddSingleton<ShoppingCartService>();
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<IProductImageRepository, EFProductImageRepository>();
builder.Services.AddScoped<IShoppingCart, EFShoppingCart>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<IVnPayService, VnPayService>();
builder.Services.AddRazorPages();
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
app.UseSession();
app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.UseEndpoints(endpoints =>
{
	_ = endpoints.MapControllerRoute(
			name: "admin",
			pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
		  );
	_ = endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.MapRazorPages();
app.Run();
