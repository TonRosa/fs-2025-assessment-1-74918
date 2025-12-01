using fs_2025_a_api_demo_002.BackgroundServices;
using fs_2025_a_api_demo_002.Endpoints;
using fs_2025_a_api_demo_002.Services;
using fs_2025_a_api_demo_002.Startup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API V1", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "API V2", Version = "v2" });

    // Resolve conflicts by selecting the first action
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
}); builder.Services.AddApiVersioning(opt =>
{
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.ReportApiVersions = true;
});

builder.Services.AddControllers();

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
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "API V2");
    });
}

app.UseHttpsRedirection();

app.MapControllers();



//app.AddRootEndPoints();
app.AddBikeEndPoints();

app.Run();


