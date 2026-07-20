using InvestmentOperations.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvestmentOperations.DataAccess.Concrete.EntityFramework
{
    public class InvestmentContext : DbContext
    {
        public InvestmentContext(DbContextOptions<InvestmentContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Trade> Trades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.Property(u => u.UserId).HasColumnName("userid");
                entity.Property(u => u.FirstName).HasColumnName("firstname");
                entity.Property(u => u.LastName).HasColumnName("lastname");
                entity.Property(u => u.Email).HasColumnName("email");
                entity.Property(u => u.PasswordHash).HasColumnName("passwordhash");
                entity.Property(u => u.CreatedAt).HasColumnName("createdat").HasColumnType("timestamp without time zone");
                entity.Property(u => u.IsActive).HasColumnName("isactive");
            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.ToTable("assets");
                entity.Property(a => a.AssetId).HasColumnName("assetid");
                entity.Property(a => a.AssetName).HasColumnName("assetname");
                entity.Property(a => a.AssetType).HasColumnName("assettype");
                entity.Property(a => a.AssetCode).HasColumnName("assetcode");
            });

            modelBuilder.Entity<Trade>(entity =>
            {
                entity.ToTable("trades");
                entity.Property(t => t.TradeId).HasColumnName("tradeid");
                entity.Property(t => t.UserId).HasColumnName("userid");
                entity.Property(t => t.AssetId).HasColumnName("assetid");
                entity.Property(t => t.Amount).HasColumnName("amount");
                entity.Property(t => t.UnitPrice).HasColumnName("unitprice");
                entity.Property(t => t.TotalPrice).HasColumnName("totalprice");
                entity.Property(t => t.TradeType).HasColumnName("tradetype");
                entity.Property(t => t.TradeDate).HasColumnName("tradedate").HasColumnType("timestamp without time zone");
            });

            modelBuilder.Entity<Price>(entity =>
            {
                entity.ToTable("prices");
                entity.Property(p => p.PriceId).HasColumnName("priceid");
                entity.Property(p => p.AssetId).HasColumnName("assetid");
                entity.Property(p => p.CurrentPrice).HasColumnName("currentprice");
                entity.Property(p => p.UpdatedAt).HasColumnName("updatedat").HasColumnType("timestamp without time zone");
            });
        }
    }
}
