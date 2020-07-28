using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using TestNotificationBackend.Models;

namespace TestNotificationBackend.Services
{
    public class UserManagerService
    {
        public List<UserData> GetAllUsers()
        {
            using (var db = new LiteDatabase("data.db"))
            {
                var collection = db.GetCollection<UserData>("UserData");
                //return collection.FindAll().ToList();
                return collection.FindAll().Select(x => new UserData(x.Id, x.Username)).ToList();
            }
        }

        public UserData GetUserByUsername(string username)
        {
            using (var db = new LiteDatabase("data.db"))
            {
                var collection = db.GetCollection<UserData>("UserData");
                return collection.FindOne(x => x.Username.Equals(username));
            }
        }

        public UserData GetUserById(Guid id)
        {
            using (var db = new LiteDatabase("data.db"))
            {
                var collection = db.GetCollection<UserData>("UserData");
                return collection.FindOne(x => x.Id.Equals(id));
            }
        }

        public bool AuthenticateUser(string username, string password)
        {
            using (var db = new LiteDatabase("data.db"))
            {
                var collection = db.GetCollection<UserData>("UserData");
                return collection.FindOne(x => x.Username.Equals(username) && x.Password.Equals(password)) != null;
            }
        }
    }
}
