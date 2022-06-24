using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace igrwijaya.Net.Identity.MongoDB.Roles
{
    public class MongoUserRole
    {
        [BsonId]
        public ObjectId Id { get; set; }
        
        public virtual ObjectId UserId { get; set; }

        public virtual ObjectId RoleId { get; set; }
    }
}
