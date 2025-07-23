using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniETicaret.Domain.Entities;
using MiniETicaret.Persistence.Contexts;
using MiniETicaret.Persistence;
using FluentValidation.AspNetCore;
using FluentValidation;
using MiniETicaret.Application.Validations.ProductValidtions;
using MiniETicaret.Application.Shared.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MiniETicaret.Application.Shared.Helpers;
using MiniETicaret.WebApi.Middlewares;
using Hangfire;
using MiniETicaret.Application.Abstracts.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssembly(typeof(ProductCreateDtoValidator).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MiniETicaret API",
        Version = "v1"
    });

    // JWT üçün t?hlük?sizlik sxemini ?lav? et
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT token daxil edin. Format: Bearer {token}"
    });

    // T?hlük?sizlik t?l?bi ?lav? olunur
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<MiniETicaretDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));

});
builder.Services.RegisterService();

builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("Default")));


builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
})
    .AddEntityFrameworkStores<MiniETicaretDbContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWTSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Dev üçün. Prod-da `true` olmal?d?r.
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))

    };
});

builder.Services.AddAuthorization(options =>
{
    foreach (var permission in PermissionHelper.GetAllPermissionList())
    {
        options.AddPolicy(permission, policy =>
        {
            policy.RequireClaim("Permission", permission);
        });
    }
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddHangfireServer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard("/hangfire");
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<IJobService>(
     "freeze-inactive-users",
    service => service.FreezeInactiveUsersAsync(),
    Cron.Daily
    );

app.MapControllers();

app.Run();
