using JwtAuthenticationTemplateNet9.Helper;
using JwtAuthenticationTemplateNet9.Infrastructure;
using JwtAuthenticationTemplateNet9.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container, migrate, update db, add user roles: admin, user - Schritt 0
string? connString = builder.Configuration.GetConnectionString("ToDosMySqlConnection");
builder.Services.AddDbContextPool<ToDosDbContext>(options =>
    options.UseMySql(connString, ServerVersion.AutoDetect(connString)));

builder.Services.AddScoped<ITokenService, JWTService>();    // Schritt 3d

#region JWT Configuration

// Add Identity services to the container                   - Schritt 1a
builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;         // to add
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ToDosDbContext>();

// Add JWT Authentication services to the container         - Schritt 1b
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtOptions =>
    {
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(
                    builder.Configuration.GetSection("JwtSettings")["Secret"]!
                )),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

#endregion

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();       // bei Swagger zu ersetzen mit folgendem Block
builder.Services.AddOpenApi(options =>  // OpenAPI + Transformer registrieren - Schitt 1c
{
    options.AddDocumentTransformer<BearerSecurityTransformer>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //app.MapScalarApiReference(); // variant a: install Scalar.AspNetCore NuGet package

    app.UseSwaggerUI(o =>          // variant b: install Swashbuckle.AspNetCore NuGet package
    {
        o.SwaggerEndpoint("/openapi/v1.json", "v1"); // bindet das .NET-9-OpenAPI an Swagger UI
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();        // Schritt 1d
app.UseAuthorization();         

app.MapControllers();

app.Run();
