using SafeVault.Service;

var builder = WebApplication.CreateBuilder(args);

// dependancy injections
builder.Services.AddSingleton<DynamoDbService>();
builder.Services.AddTransient<AesEncryptionService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors();

app.MapControllers();
app.Run();