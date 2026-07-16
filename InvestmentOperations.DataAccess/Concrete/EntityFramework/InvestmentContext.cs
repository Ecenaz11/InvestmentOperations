using InvestmentOperations.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class InvestmentContext : DbContext 
    {
        DbSet<User> Users { get; set; }
        DbSet<Asset> Assets { get; set; }
        DbSet<Price> Prices { get; set; }
        DbSet<Trade> Transactions { get; set; }
    }
}
