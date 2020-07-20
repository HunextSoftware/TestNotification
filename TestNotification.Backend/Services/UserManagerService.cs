using LiteDB;
using System;
using TestNotificationBackend.Models;

namespace TestNotificationBackend.Services
{
    public class UserManagerService
    {
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
