using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Azure.Storage.Blobs;
using KeepTheApex;
using KeepTheApex.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using KeepTheApex.Hubs;
using Microsoft.Azure.SignalR;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSignalR().AddAzureSignalR();

// Add Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = configuration["Jwt:Authority"];
        options.Audience = configuration["Jwt:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = false,
            ValidateLifetime = false,
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role,
            SignatureValidator = (token, parameters) => 
                new JwtSecurityToken(token)
        };
    });

// Register DI for services (implementations to be added)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IFeedService, FeedService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<INotificationService, NotificationService>();


// Register Cosmos DB client
builder.Services.AddSingleton(s =>
{
    var config = builder.Configuration.GetSection("CosmosDb");
    return new CosmosClient(config["Account"], config["Key"]);
});
// Register Blob Storage client
builder.Services.AddSingleton(s =>
{
    var config = builder.Configuration.GetSection("BlobStorage");
    return new BlobServiceClient(config["ConnectionString"]);
});

builder.Services.AddScoped<IPostService, PostService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "KeepTheApex API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name            = "Authorization",
        Type            = SecuritySchemeType.ApiKey,
        Scheme          = "Bearer",
        BearerFormat    = "JWT",
        In              = ParameterLocation.Header,
        Description     = "Enter ‘Bearer {your JWT}’"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    
    options.OperationFilter<SwaggerFileUploadOperationFilter>();
});


var app = builder.Build();

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

app.UseAzureSignalR(routes =>
{
    routes.MapHub<NotificationHub>("/hubs/notifications");
});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
