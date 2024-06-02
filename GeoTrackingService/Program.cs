using GeoTrackingService.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register FirebaseService with DI container
var firebaseConfig = builder.Configuration.GetSection("Firebase");
string firebaseBaseUrl = firebaseConfig["BaseUrl"];

if (string.IsNullOrEmpty(firebaseBaseUrl))
{
    throw new InvalidOperationException("Firebase base URL is not configured.");
}

builder.Services.AddScoped<IFirebaseService, FirebaseService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<FirebaseService>>();
    return new FirebaseService(firebaseBaseUrl, logger);
});


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
