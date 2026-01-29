using Services;
using ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepositoryContracts;
using Repositories;
using System.Runtime.InteropServices;
using Serilog;
using ContactManager.Filters.ActionFilters;

var builder = WebApplication.CreateBuilder(args);

// Logging
//builder.Host.ConfigureLogging(loggingProvider =>
//{
//    loggingProvider.ClearProviders();
//    loggingProvider.AddConsole();
//});
//builder.Logging.ClearProviders().AddConsole().AddDebug().AddEventLog();

// Logging
//builder.Host.ConfigureLogging(loggingProvider =>
//{
//    loggingProvider.ClearProviders();
//    loggingProvider.AddConsole();
//});

//builder.Logging.ClearProviders().AddConsole().AddDebug();
//if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
//{
//    builder.Logging.AddEventLog();
//}


// Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => { 
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services);
});

builder.Services.AddControllersWithViews(options =>
{
    // Global filter registrations can be done here
    //options.Filters.Add<ResponseHeaderActionFilter>(); // Default constructor will be used

    var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

    options.Filters.Add(new ResponseHeaderActionFilter(logger, "X-Global-Header", "Global-Value", 2)); // Parameterized constructor will be used
});

// Registering CountriesService as a scoped service
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();

// DB Context service registration
builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

// Register logging service
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
});

var app = builder.Build();

if(builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

if(builder.Environment.IsEnvironment("Test") == false)
{
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}

app.UseSerilogRequestLogging();
app.UseHttpLogging();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();


public partial class Program { } // Makes the auto-generated Program class accessible programmatically