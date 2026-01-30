using Services;
using ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepositoryContracts;
using Repositories;
using Serilog;
using ContactManager.Filters.ActionFilters;
using ContactManager;

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
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider builder, LoggerConfiguration loggerConfiguration) => { 
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(builder);
});

// Configure builder
builder.Services.ConfigureServices(builder.Configuration);


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