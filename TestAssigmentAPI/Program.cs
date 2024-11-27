using Serilog;
using TestAssigmentAPI.Extensions;
using TestAssigmentAPI.Filters;
using TestAssigmentAPI.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<RequestLoggingFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.RegisterSwaggerUI();

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
                            loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
builder.Services
    .AddOptions<ClientsOptions>()
    .Bind(builder.Configuration)
    .ValidateDataAnnotations()
    .Validate(ClientsOptions.ValidateClients, "ApiKey or Name cannot be empty")
    .ValidateOnStart();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();