using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Business.Concrete;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.DataAccess.Concrete.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<InvestmentContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("InvestmentDb")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter the token in this format: Bearer {token}"
    });
    options.AddSecurityRequirement(document=> new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>() 
        }
    });
});
builder.Services.AddScoped<IAssetService, AssetManager>();
builder.Services.AddScoped<IAssetDal, EfAssetDal>();
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IUserDal, EfUserDal>();
builder.Services.AddScoped<ITradeService, TradeManager>();
builder.Services.AddScoped<ITradeDal, EfTradeDal>();
builder.Services.AddScoped<IPriceService, PriceManager>();
builder.Services.AddScoped<IPriceDal, EfPriceDal>();
builder.Services.AddScoped<IBalanceService, BalanceManager>();
builder.Services.AddScoped<IBalanceDal, EfBalanceDal>();
builder.Services.AddScoped<ILogService, LogManager>();
builder.Services.AddScoped<ILogDal, EfLogDal>();
builder.Services.AddScoped<ITokenService, TokenManager>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization(options=>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

var app = builder.Build();

//configure the HTTP request pipeline.
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
