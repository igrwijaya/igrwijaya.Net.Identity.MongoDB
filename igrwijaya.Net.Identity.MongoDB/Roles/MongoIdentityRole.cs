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
        
        public string Id { get; internal set; }

        public string Name { get; internal set; }

        public string NormalizedName { get; internal set; }
    }
}