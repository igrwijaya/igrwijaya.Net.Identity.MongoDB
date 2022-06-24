using System;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace igrwijaya.Net.Identity.MongoDB.Models
{
    public class MongoIdentityUser
    {
        public MongoIdentityUser()
        {
        
        }
    
        public MongoIdentityUser(string userName, string email)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }
        
        [BsonId]
        public ObjectId Id { get; internal set; }
        
        public string UserName { get; internal set; }
        
        public string NormalizedUserName { get; internal set; }
        
        public string PasswordHash { get; internal set; }
        
        public string Email { get; internal set; }
        
        public bool EmailConfirmed { get; internal set; }
        
        public string NormalizeEmail { get; internal set; }
    }
}