using igrwijaya.Net.Identity.MongoDB.Models;

namespace igrwijaya.Net.Identity.Test.API;

public class ApplicationUser : MongoIdentityUser
{
    public ApplicationUser()
    {
        
    }

    public ApplicationUser(string userName, string email) : base(userName, email)
    {
    }
}