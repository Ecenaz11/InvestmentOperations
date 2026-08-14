using InvestmentOperations.Business.Abstract;
using InvestmentOperations.Business.Concrete;

namespace InvestmentOperations.API.Extensions
{
    public static class ExternalApiExtensions
{
    public static IServiceCollection AddExternalPriceApis(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<IGoldApiClient, GoldApiClient>(client =>
        {
            client.BaseAddress = new Uri(
                configuration["ExternalPriceApis:GoldApi:BaseUrl"]!);

            client.DefaultRequestHeaders.Add(
                "x-access-token",
                configuration["ExternalPriceApis:GoldApi:ApiKey"]);
        });

        services.AddHttpClient<IFrankfurterClient, FrankfurterClient>(client =>
        {
            client.BaseAddress = new Uri(
                configuration["ExternalPriceApis:Frankfurter:BaseUrl"]!);
        });

        return services;
    }
}
}