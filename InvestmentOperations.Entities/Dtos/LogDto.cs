using InvestmentOperations.Core.Entities;
using InvestmentOperations.Entities.Enums;

namespace InvestmentOperations.Entities.Dtos
{
    public class LogDto: IDto
    {
         public int LogId {get;set;}
        public int UserId {get; set;}
        public string Action {get;set;}
        public String Details{get;set;}
        public DateTime CreatedAt{get;set;}
        public LogStatus Status {get; set;}

    }
}