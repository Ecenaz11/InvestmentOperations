using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.Business.Abstract
{
    public interface ITradeService
    {
        void Add(Trade trade);
        void Update(Trade trade);
        void Delete(int id);
        List<Trade> GetAll();
        Trade GetById(int id);


    }
}
