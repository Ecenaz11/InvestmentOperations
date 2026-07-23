using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Business.Concrete;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.DataAccess.Concrete.EntityFramework;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<InvestmentContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("InvestmentDb")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAssetService,AssetManager>();
builder.Services.AddScoped<IAssetDal, EfAssetDal>();
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IUserDal, EfUserDal>();  
builder.Services.AddScoped<ITradeService, TradeManager>();
builder.Services.AddScoped<ITradeDal, EfTradeDal>();
builder.Services.AddScoped<IPriceService, PriceManager>();
builder.Services.AddScoped<IPriceDal, EfPriceDal>();
builder.Services.AddScoped<IBalanceService, BalanceManager>();
builder.Services.AddScoped<IBalanceDal, EfBalanceDal>();

var app = builder.Build();

//configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
