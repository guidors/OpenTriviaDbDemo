using Microsoft.Extensions.Options;
using OpenTriviaDbWebService.Infrastructure;
using OpenTriviaDbWebService.Models;
using OpenTriviaDbWebService.Options;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");


    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog();

    // Add services to the container.

    builder.Services.AddControllers();

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
    var openTriviaDbOptions = app.Services.GetRequiredService<IOptions<OpenTriviaDbOptions>>().Value;
    QuizRequest.Init(openTriviaDbOptions.CategoryUrl);

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