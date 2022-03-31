using MongoDB.Bson;
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

        public async Task<User> GetUserByLoginModel(LoginModel loginModel)
        {
            return await (await Users.FindAsync(user => user.Login == loginModel.Login && user.Password == loginModel.Password))
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoles(string[] role)
        {
            return (await Users.FindAsync(user => role.Contains(user.Role))).ToEnumerable();
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return (await Users.FindAsync(_ => true)).ToEnumerable();
        }

        public async Task AddUser(User user)
        {
            await Users.InsertOneAsync(user);
        }
    }
}
