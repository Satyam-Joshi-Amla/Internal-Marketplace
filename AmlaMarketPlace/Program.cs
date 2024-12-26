using AmlaMarketPlace.BAL.Agent.Agents.Account;
using AmlaMarketPlace.BAL.Agent.Agents.Admin;
using AmlaMarketPlace.BAL.Agent.Agents.Product;
using AmlaMarketPlace.BAL.Agent.Agents.Profile;
using AmlaMarketPlace.BAL.Agent.IAgents.IAccount;
using AmlaMarketPlace.BAL.Agent.IAgents.IAdmin;
using AmlaMarketPlace.BAL.Agent.IAgents.IProduct;
using AmlaMarketPlace.BAL.Agent.IAgents.IProfile;
using AmlaMarketPlace.DAL.Data;
using AmlaMarketPlace.DAL.Service.IServices.IAccount;
using AmlaMarketPlace.DAL.Service.IServices.IAdmin;
using AmlaMarketPlace.DAL.Service.IServices.IProduct;
using AmlaMarketPlace.DAL.Service.IServices.IProfile;
using AmlaMarketPlace.DAL.Service.Services.Account;
using AmlaMarketPlace.DAL.Service.Services.Admin;
using AmlaMarketPlace.DAL.Service.Services.Product;
using AmlaMarketPlace.DAL.Service.Services.Profile;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registering the DBContext with the connection string
builder.Services.AddDbContext<AmlaMarketPlaceDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registering for DI
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountAgent, AccountAgent>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductAgent, ProductAgent>();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAdminAgent, AdminAgent>();

builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IProfileAgent, ProfileAgent>();


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

// For Detailed Error Page
// app.UseDeveloperExceptionPage();

// For Production Environment
// app.UseExceptionHandler("/Home/Error"); // Redirects to the "Error" action in the "Home" controller
// app.UseStatusCodePagesWithRedirects("/Home/Error?code={0}"); // Redirects based on status codes


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=ProductListing}/{id?}");

app.Run();
