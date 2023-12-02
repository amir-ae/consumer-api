using Consumer.Infrastructure;
using Consumer.API;
using Consumer.API.Extensions.ErrorHandling;
using Consumer.API.Endpoints.V1;
using Consumer.API.Extensions.Caching;
using Consumer.Application;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, config) 
    => config.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();
app.SeedCache();

app.AddCustomerEndpoints();
app.AddProductEndpoints();
app.AddErrorEndpoint();

app.Run();

public partial class Program { }