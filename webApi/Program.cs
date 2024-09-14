
using Microsoft.EntityFrameworkCore;
using Domain_Core;
using Application;
using Infrastructure;
using webApi;
using System.Windows.Forms;
using Microsoft.AspNetCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


// Add services to the container.
builder.Services.AddSingleton<Iuser>(new userRepository(connectionString));
builder.Services.AddSingleton<IPostingService, PostingService>();
builder.Services.AddSingleton<IUserProfileService, UserProfileService>();
builder.Services.AddSingleton<IRepositoryService<Posting>, RepositoryService<Posting>>();
builder.Services.AddSingleton<IRepositoryService<images>, RepositoryService<images>>();
builder.Services.AddSingleton<IRepositoryService<postANDimage>, RepositoryService<postANDimage>>();
builder.Services.AddSingleton<IRepositoryService<userProfile>, RepositoryService<userProfile>>();

builder.Services.AddSingleton<IRepository<postANDimage>>(new GenericRepository<postANDimage>(connectionString));
builder.Services.AddSingleton<IRepository<images>>(new GenericRepository<images>(connectionString));
builder.Services.AddSingleton<IRepository<Posting>>(new GenericRepository<Posting>(connectionString));
builder.Services.AddSingleton<IRepository<userProfile>>(new GenericRepository<userProfile>(connectionString));
builder.Services.AddSingleton<IPosting>(provider =>
{
    var postingRepository = new PostingRepository(connectionString,
        provider.GetRequiredService<IRepository<Posting>>(),
        provider.GetRequiredService<IRepository<images>>());

    return postingRepository;
});
builder.Services.AddSingleton<IUserProfile>(prov =>
{
    var userProfileRepository = new userProfileRepository(connectionString,
         prov.GetRequiredService<IRepository<userProfile>>());
    return userProfileRepository;

});




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();


//For showing authorization button
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});



//JWT 




var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "https://localhost:7177",
        ValidAudience = "https://localhost:7177",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});




builder.Services.AddAuthorization();




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

app.Run();
