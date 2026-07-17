using InvestmentOperations.DataAccess.Abstract;
using InvestmentOperations.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

namespace InvestmentOperations.DataAccess.Concrete.FileContext
{
    public class JsonUserDal : IUserDal
    {
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "users.txt");
        private List<User>ReadFromFile()
        {
            if (!File.Exists(_filePath))
                return new List<User>();
            string jsonText = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<User>>(jsonText) ?? new List<User>();
        }
        private void WriteToFile(List<User> userList)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string updatedJson = JsonSerializer.Serialize(userList, options);
            File.WriteAllText(_filePath, updatedJson);
        }
        public void Add(User user)
        {
            var list = ReadFromFile();
            user.UserId = list.Count > 0 ? list.Max(u => u.UserId) + 1 : 1;
            list.Add(user);
            WriteToFile(list);
        }

        public void Delete(User user)
        {
            var list = ReadFromFile();
            var userToDelete = list.FirstOrDefault(u => u.UserId == user.UserId);
            if (userToDelete!= null)
            {
                list.Remove(userToDelete);
                WriteToFile(list);
            }
        }

        public User Get(Expression<Func<User, bool>> filter)
        {
            var list = ReadFromFile();
            return list.AsQueryable().FirstOrDefault(filter);
        }

        public List<User> GetAll(Expression<Func<User, bool>> filter = null)
        {
            var list = ReadFromFile();
            if(filter ==null)
            {
                return list;
            }
            else
            {
                return list.AsQueryable().Where(filter).ToList();
            }
        }

        public void Update(User user)
        {
            var list = ReadFromFile();
            var userToUpdate = list.FirstOrDefault(u => u.UserId == user.UserId);
            if(userToUpdate!=null)
            {
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.Email = user.Email;
                userToUpdate.PasswordHash = user.PasswordHash;
                userToUpdate.IsActive = user.IsActive;

                WriteToFile(list);
            }
        }
    }
}
