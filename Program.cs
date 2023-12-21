using Microsoft.EntityFrameworkCore;
using Selflink_api.Db;
using Selflink_api.Services;
using Selflink_api.Services.Hosted;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY") ?? throw new Exception("Stripe secret key not found in env variable !");
StripeConfiguration.MaxNetworkRetries = 3;


// Add services to the container.
builder.Services.AddDbContext<SelflinkDbContext>(options => {
    
    string? connStr =  Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
    string? dbName =  Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");

    
    if ( connStr == null ) {
        Console.WriteLine("MongoDB connection string not found in env variable !");
    }

    if ( dbName == null ) {
        Console.WriteLine("MongoDB database name not found in env variable !");
    }

    options.UseMongoDB(connStr!, dbName!);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "SelfLinkPolicy",
                      policy  =>
                      {
                          policy.WithOrigins("http://localhost:4200",
                                              "https://selflink.fr");
                      });
});

builder.Services.AddScoped<ILinkService, LinkService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// background service for stripe webhook & consumer
builder.Services.AddHostedService<StripeWebHookHostedService>();
builder.Services.AddScoped<IScopedProcessingService, ScopedProcessingService>();


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
