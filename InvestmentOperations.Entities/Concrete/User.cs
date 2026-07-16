using InvestmentOperations.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvestmentOperations.Entities.Concrete
{
    public class User : IEntity
    {
        public int UserId { get; set; }
        public  string FirstName { get; set; }
        public  string  LastName { get; set; }
        public  string  Email { get; set; }
        public  string PasswordHash  { get; set; }
        public  DateTime CreatedAt { get; set; }
        public  bool IsActive { get; set; }
    }
}
