using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenTriviaDbWebService.Infrastructure;
using OpenTriviaDbWebService.Options;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");


    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog();

    // Should come from vault when building release.
    const string symmetricKey = "06F30BA2-CE84-4532-929E-0129B5532B65";

    // JWT Configuration for session tokens. We don't need to validate users.
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(symmetricKey)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

    // Add services to the container.

    builder.Services.AddControllers();

    builder.Services.Configure<OpenTriviaDbOptions>(builder.Configuration.GetSection(OpenTriviaDbOptions.OptionKey));
    builder.Services.AddSingleton<OpenTriviaDbConnector>();

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

    app.UseAuthorization();

    app.MapControllers();

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