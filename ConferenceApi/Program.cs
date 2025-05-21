using System.Security.Claims;
using System.Text;
using ConferenceApi.Database;
using ConferenceApi.Database.Repositories;
using ConferenceApi.Models;
using ConferenceApi.Models.Database;
using ConferenceApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "M Bank API", 
        Version = "v1",
        Description = "API для конференциии",
        TermsOfService = new Uri("http://localhost:8080/terms"),
        Contact = new OpenApiContact
        {
            Name = "API поддержки",
            Url = new Uri("http://localhost:8080/support")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("http://opensource.org/licenses/MIT")
        },
        
    });
    
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationContext>(x =>
{
    var dbSettings =  builder.Configuration.GetSection("DatabaseSettings");
    var hostname = dbSettings["Hostname"] ?? "localhost";
    var port = dbSettings["Port"] ?? "5432";
    var name = dbSettings["Name"] ?? "postgres";
    var username = dbSettings["Username"] ?? "postgres";
    var password = dbSettings["Password"] ?? "postgres";
    x.UseNpgsql($"Server={hostname}:{port};Database={name};Uid={username};Pwd={password};");
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
})
.AddEntityFrameworkStores<ApplicationContext>()
.AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"]
    };
});

builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddScoped<UnitOfWork>(sp => new UnitOfWork(sp.GetRequiredService<ApplicationContext>()));

builder.Services.AddScoped<IJWTService,JWTService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ReadPolicy", policy => 
        policy.RequireClaim("Permission", "Read"));
        
    options.AddPolicy("WritePolicy", policy => 
        policy.RequireClaim("Permission", "Write"));
        
    options.AddPolicy("AdminPolicy", policy => 
        policy.RequireRole("Admin"));
        
    options.AddPolicy("DeletePolicy", policy => 
        policy.RequireClaim("Permission", "Delete"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers().RequireAuthorization();

/*
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger") && 
        !context.User.Identity.IsAuthenticated)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }

    await next.Invoke();
});

app.MapWhen(
    ctx => ctx.Request.Path.StartsWithSegments("/swagger"),
    appBuilder =>
    {
        appBuilder.UseRouting();
        appBuilder.UseAuthentication();
        appBuilder.UseAuthorization();
        appBuilder.UseSwagger();
        appBuilder.UseSwaggerUI();
    });
*/

app.UseRouting();
app.UseCors("ProductionPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
