using Fibrasol_Delivery.AuthProvider.Includes;
using Fibrasol_Delivery.Config;
using Microsoft.AspNetCore.Identity;
using Tipi.Tools.Services.Config;

var builder = WebApplication.CreateBuilder(args);
var isDev = builder.Environment.IsDevelopment();

// Add services to the container.
builder.Services.AddControllersWithViews();

var razorBuilder = builder.Services.AddRazorPages();
if (isDev) //Enable Razor pages compilation if the Web App is in development
    razorBuilder.AddRazorRuntimeCompilation();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequiredLength = 4;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
})
                .AddDapperStores()
                .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/login";
    options.LogoutPath = $"/logout";
    options.AccessDeniedPath = $"/denied";
});

var config = builder.Configuration;
builder.Services.ConfigureDataAccessLayer(config);

builder.Services.ConfigureDoSpaces(config["S3Config:AccessKey"]!,
        config["S3Config:SecretKey"]!, config["S3Config:BucketName"]!,
        config["S3Config:Root"]!, config["S3Config:EndpointUrl"]!,
        config["S3Config:Region"]!, config["S3Config:UseCdn"]!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
