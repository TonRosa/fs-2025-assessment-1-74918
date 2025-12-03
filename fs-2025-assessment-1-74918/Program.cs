using fs_2025_a_api_demo_002.BackgroundServices;
using fs_2025_a_api_demo_002.Services;
using fs_2025_a_api_demo_002.Startup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<JsonDataService>();
builder.Services.AddSingleton<CosmosDataService>();

builder.Services.AddMemoryCache();
builder.Services.AddHostedService<BikeStationUpdater>();

builder.AddDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
   
}


app.UseHttpsRedirection();

app.MapControllers();


app.Run();

public partial class Program { }

