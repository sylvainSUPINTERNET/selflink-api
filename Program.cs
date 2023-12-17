using Microsoft.EntityFrameworkCore;
using Selflink_api.Db;
using Selflink_api.Services;

var builder = WebApplication.CreateBuilder(args);

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


builder.Services.AddScoped<ILinkService, LinkService>();

builder.Services.AddControllers();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
