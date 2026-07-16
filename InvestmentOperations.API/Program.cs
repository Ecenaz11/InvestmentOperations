using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Business.Concrete;
using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.DataAccess.Concrete.FileContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAssetService,AssetManager>();
builder.Services.AddScoped<IAssetDal, JsonAssetDal>();
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IUserDal, JsonUserDal>();
builder.Services.AddScoped<ITradeService, TradeManager>();
builder.Services.AddScoped<ITradeDal, JsonTradeDal>();
builder.Services.AddScoped<IPriceService, PriceManager>();
builder.Services.AddScoped<IPriceDal, JsonPriceDal>();

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
