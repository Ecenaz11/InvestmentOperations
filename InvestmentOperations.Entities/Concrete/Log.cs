using InvestmentOperations.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvestmentOperations.Entities.Concrete
{
    public class Log : IEntity
    {
        public int LogId {get;set;}
        public int UserId {get; set;}
        public string Action {get;set;}
        public String Details{get;set;}
        public DateTime CreatedAt{get;set;}
    }
}