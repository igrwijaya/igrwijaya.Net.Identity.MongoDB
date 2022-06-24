using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace igrwijaya.Net.Identity.MongoDB.Roles
{
    public class MongoIdentityRole
    {
        public MongoIdentityRole()
        {
        
        }

        public MongoIdentityRole(string name)
        {
            Name = name;
        }
        
        [BsonId]
        public ObjectId Id { get; internal set; }

        public string Name { get; internal set; }

        public string NormalizedName { get; internal set; }
    }
}