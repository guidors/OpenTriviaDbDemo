using OpenTriviaDbWebService.Infrastructure;
using OpenTriviaDbWebService.Models;
using OpenTriviaDbWebService.Options;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");


    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog();

    // Add services to the container.

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });

    builder.Services.Configure<OpenTriviaDbOptions>(builder.Configuration.GetSection(OpenTriviaDbOptions.OptionKey));
    builder.Services.AddSingleton<OpenTriviaDbConnector>();

    // Add CORS for Vue frontend
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Use CORS before other middleware
    app.UseCors();

    app.MapControllers();

    // Initialize QuizRequest with categoryUrl from options
    var openTriviaDbConnector = app.Services.GetRequiredService<OpenTriviaDbConnector>();
    QuizRequest.Init(openTriviaDbConnector.GetCategoriesAsync);

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}