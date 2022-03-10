using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicQueueServer.Models
{
    public class AppDB
    {
        private readonly MongoClient mongoClient;
        public readonly IMongoDatabase ElectonicQueueDB;
        public readonly IMongoCollection<User> Users;
        public AppDB()
        {
            var localConnectionString = "mongodb://localhost:27017";
            var setting = MongoClientSettings.FromConnectionString(localConnectionString);
            mongoClient = new MongoClient(setting);
            ElectonicQueueDB = mongoClient.GetDatabase("ElectronicQueue");
            Users = ElectonicQueueDB.GetCollection<User>("Users");
        }

        public User GetUserByLoginModel(LoginModel loginModel)
        {
            return Users.Find(user => user.Login == loginModel.Login && user.Password == loginModel.Password)?.Single();
        }

        public IEnumerable<User> GetUsersByRoles(string[] role)
        {
            return Users.Find(user => role.Contains(user.Role)).ToEnumerable();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return Users.Find(_ => true).ToEnumerable();
        }

        public void AddUser(User user)
        {
            Users.InsertOne(user);
        }
    }
}
