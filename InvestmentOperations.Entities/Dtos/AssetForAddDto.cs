using InvestmentOperations.Core.Entities;

namespace InvestmentOperations.Entities.Dtos
{
    public class AssetForAddDto : IDto
    {
        public string AssetName { get; set; }
        public string AssetType { get; set; }
        public string AssetCode { get; set; }
    }
}