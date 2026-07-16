using InvestmentOperations.Entities.Concrete;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InvestmentOperations.Business.Abstract
{
    public interface IPriceService
    {
        void Add(Price price);
        void Update(Price price);
        void Delete(int id);
        Price GetById(int id);
        List<Price> GetAll();


    }
}
