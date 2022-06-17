using igrwijaya.Net.Identity.MongoDB.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace igrwijaya.Net.Identity.MongoDB
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder UseMongoDb(this IdentityBuilder builder)
            => builder
                .AddMongoUserStore()
                .AddMongoRoleStore();

        private static IdentityBuilder AddMongoUserStore(this IdentityBuilder builder)
        {
            var userStoreType = typeof(MongoUserStore<,>).MakeGenericType(builder.UserType, builder.RoleType);

            builder.Services.AddScoped(
                typeof(IUserStore<>).MakeGenericType(builder.UserType),
                userStoreType
            );

            return builder;
        }

        private static IdentityBuilder AddMongoRoleStore(
            this IdentityBuilder builder
        )
        {
            var roleStoreType = typeof(MongoRoleStore<>).MakeGenericType(builder.RoleType);

            builder.Services.AddScoped(
                typeof(IRoleStore<>).MakeGenericType(builder.RoleType),
                roleStoreType
            );

            return builder;
        }
    }
}