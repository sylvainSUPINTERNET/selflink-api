using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Selflink_api.Db;
using Selflink_api.Services;
using Selflink_api.Services.Hosted;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY") ?? throw new Exception("Stripe secret key not found in env variable !");
StripeConfiguration.MaxNetworkRetries = 3;


// Add services to the container.
// Conf for EF Core + MongoDB ( but is too early to use it for the moment ! wait for 2024)
// builder.Services.AddDbContext<SelflinkDbContext>(options => {
    
//     string? connStr =  Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
//     string? dbName =  Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");

    
//     if ( connStr == null ) {
//         Console.WriteLine("MongoDB connection string not found in env variable !");
//     }

//     if ( dbName == null ) {
//         Console.WriteLine("MongoDB database name not found in env variable !");
//     }

//     options.UseMongoDB(connStr!, dbName!);
// });

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "SelfLinkPolicy",
                      policy  =>
                      {
                          policy.WithOrigins("http://localhost:4200",
                                              "https://selflink.fr").AllowAnyMethod().AllowAnyHeader();
                      });
});

builder.Services.AddSingleton<IMongoDbClientSingleton, MongoDbClientSingleton>();
builder.Services.AddScoped<ILinkService, LinkService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// background service for stripe webhook & consumer
// builder.Services.AddHostedService<StripeWebHookHostedService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Google", options =>
    {
        options.Authority = "https://accounts.google.com";
        options.Audience = "114480973126-npvvh928tpq60jhngerft26ebvbsksbi.apps.googleusercontent.com"; 
    });
    // If want to add another provider support
    //.AddJwtBearer("XXX", options => ...)

builder.Services.AddAuthorization(options =>
{   
    //Override default authorization since we have multiple provider !
        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes("Google") // If want to add another provider support .AddAuthenticationSchemes("Google", "XXX")
            .Build();

        // add granular policies
        //   options.AddPolicy("GoogleAdministrators", new AuthorizationPolicyBuilder()
        // .RequireAuthenticatedUser()
        // .AddAuthenticationSchemes("Google")
        // .RequireClaim("role", "admin")
        // .Build());

        //   options.AddPolicy("XXXUser", new AuthorizationPolicyBuilder()
        // .RequireAuthenticatedUser()
        // .AddAuthenticationSchemes("Google")
        // .RequireClaim("role", "user")
        // .Build());
});


if ( Environment.GetEnvironmentVariable("LOAD_DATABASE") != null && Environment.GetEnvironmentVariable("LOAD_DATABASE") == "true") {
        // Load database 
    Console.WriteLine($"LOAD_DATABASE has been set, loading database data ...");
    builder.Services.AddHostedService<StartupTaskHostService>();
}


builder.Services.AddScoped<IScopedProcessingService, ScopedProcessingService>();


var app = builder.Build();

app.UseCors("SelfLinkPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
