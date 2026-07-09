using Fibrasol_Delivery.AuthProvider.Includes;
using Fibrasol_Delivery.Config;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography.X509Certificates;
using Tipi.Tools.Services.Config;
using Tipicode.Orion.Logger;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOrionLogging(options =>
{
    builder.Configuration.GetSection("Orion").Bind(options);
    options.MinimumLevel = builder.Environment.IsProduction()
        ? Microsoft.Extensions.Logging.LogLevel.Warning
        : Microsoft.Extensions.Logging.LogLevel.Debug;
});
var isDev = builder.Environment.IsDevelopment();

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

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

var dataProtectionBuilder = builder.Services.AddDataProtection()
    .SetApplicationName("FibrasolDelivery");

if (!isDev)
{
    // /keys is only writable by the root user the production container runs as;
    // local dev keeps the ephemeral default so `dotnet run` doesn't need elevated permissions.
    dataProtectionBuilder.PersistKeysToFileSystem(new DirectoryInfo("/keys"));

    var dataProtectionCertBase64 = config["DataProtection:CertBase64"];
    if (!string.IsNullOrEmpty(dataProtectionCertBase64))
    {
        var dataProtectionCert = new X509Certificate2(Convert.FromBase64String(dataProtectionCertBase64),
            config["DataProtection:CertPassword"]);
        dataProtectionBuilder.ProtectKeysWithCertificate(dataProtectionCert);
    }
}

builder.Services.AddHttpsRedirection(options => options.HttpsPort = 443);

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

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    KnownNetworks = { },
    KnownProxies = { }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
