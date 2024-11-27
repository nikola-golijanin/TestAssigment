using TestAssigmentAPI.Middleware;
using TestAssigmentAPI.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddOptions<ClientsOptions>()
    .Bind(builder.Configuration)
    .ValidateDataAnnotations()
    .Validate(ClientsOptions.ValidateClients, "ApiKey or Name cannot be empty")
    .ValidateOnStart();

builder.Services.AddScoped<ApiKeyAttribute>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseMiddleware<ApiKeyMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();