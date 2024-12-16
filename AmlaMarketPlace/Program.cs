using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Options;
using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.BAL.Agent.Agents.Account;
using AmlaMarketPlace.BAL.Agent.Agents.Product;
using AmlaMarketPlace.DAL.Service.Services.Account;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using AmlaMarketPlace.DAL.Service.Services.Product;
using AmlaMarketPlace.DAL.Service.Services.Admin;
using AmlaMarketPlace.BAL.Agent.Agents.Admin;
using AmlaMarketPlace.BAL.Agent.Agents.Profile;
using AmlaMarketPlace.DAL.Service.Services.Profile;

var builder = WebApplication.CreateBuilder(args);

// Registering the DBContext with the connection string
builder.Services.AddDbContext<AmlaMarketPlaceDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registering for DI
builder.Services.AddScoped<AccountAgent>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<ProductAgent>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<AdminAgent>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<ProfileAgent>();
builder.Services.AddScoped<ProfileService>();

// Adding cookie authentication services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Account/SignIn";
    options.LogoutPath = "/Account/SignOut";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "AmlaMarketPlaceAuth";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
});

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true; // Enable client-side validation
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

// Added authentication and authorization middle ware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=ProductListing}/{id?}");

app.Run();
