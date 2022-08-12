using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using igrwijaya.Net.Identity.MongoDB.Models;
using igrwijaya.Net.Identity.MongoDB.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace igrwijaya.Net.Identity.MongoDB.Stores
{
    public class MongoUserStore<TUser, TRole> :
        IUserEmailStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserRoleStore<TUser>
        where TUser : MongoIdentityUser
        where TRole : MongoIdentityRole
    {
        #region Private Methods

        private readonly IMongoCollection<TRole> _mongoRoleCollection;
        private readonly IMongoCollection<TUser> _mongoUserCollection;
        private readonly IMongoCollection<MongoUserRole> _mongoUserRoleCollection;

        #endregion

        public MongoUserStore(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["Identity:MongoDbConnection"]);
            
            var mongoDatabase = client.GetDatabase(configuration["Identity:Db"]);
            _mongoRoleCollection = mongoDatabase.GetCollection<TRole>("AspNet_Roles");
            _mongoUserCollection = mongoDatabase.GetCollection<TUser>("AspNet_Users");
            _mongoUserRoleCollection = mongoDatabase.GetCollection<MongoUserRole>("AspNet_UserRoles");
        }

        #region IDisposable

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private bool _disposed;

        public void Dispose()
        {
            _disposed = true;
        }

        #endregion

        #region User Store

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.UserName = userName;

            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(normalizedName))
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();

            user.Id = ObjectId.GenerateNewId(DateTime.UtcNow);

            await _mongoUserCollection
                .InsertOneAsync(user, cancellationToken: cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _mongoUserCollection
                .ReplaceOneAsync(item => item.Id == user.Id, user, cancellationToken: cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _mongoUserCollection
                .DeleteOneAsync(item => item.Id == user.Id, cancellationToken: cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            cancellationToken.ThrowIfCancellationRequested();
            var user = await ReadUserAsync(userId, cancellationToken);

            return user;
        }

        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var query = await _mongoUserCollection
                .FindAsync(item => item.NormalizedUserName == normalizedUserName, cancellationToken: cancellationToken);

            return query.FirstOrDefault();
        }

        private async Task<TUser> ReadUserAsync(string userId, CancellationToken cancellationToken)
        {
            var query = await _mongoUserCollection
                .FindAsync(item => item.Id == ObjectId.Parse(userId), cancellationToken: cancellationToken);

            return query.FirstOrDefault();
        }

        #endregion

        #region User Password

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(passwordHash) || user == null)
            {
                throw new ArgumentNullException(nameof(passwordHash));
            }

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(string.IsNullOrEmpty(user.PasswordHash));
        }

        #endregion

        #region User Role

        public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var roleCursor = await _mongoRoleCollection
                .FindAsync(item => item.NormalizedName == roleName, cancellationToken: cancellationToken);

            var role = roleCursor.FirstOrDefault();
            
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            await _mongoUserRoleCollection
                .InsertOneAsync(new MongoUserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                }, cancellationToken: cancellationToken);
        }

        public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var roleCursor = await _mongoRoleCollection
                .FindAsync(item => item.NormalizedName == roleName, cancellationToken: cancellationToken);

            var role = roleCursor.FirstOrDefault();
            
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            
            await _mongoUserRoleCollection
                .DeleteOneAsync(item => 
                    item.UserId == user.Id && item.RoleId == role.Id, 
                    cancellationToken: cancellationToken);
        }

        public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userRoles = await _mongoUserRoleCollection
                .FindAsync(item => item.UserId == user.Id, cancellationToken: cancellationToken);

            var userRoleList = userRoles.ToList();

            if (!userRoleList.Any())
            {
                return new List<string>();
            }

            var roleIds = userRoleList.Select(item => item.RoleId);
            var roles = await _mongoRoleCollection.FindAsync(item => 
                    roleIds.Contains(item.Id),
                    cancellationToken: cancellationToken);

            return roles.ToList()
                .Select(item => item.Name)
                .ToList();
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var roleCursor = await _mongoRoleCollection
                .FindAsync(item => item.NormalizedName == roleName, cancellationToken: cancellationToken);

            var role = roleCursor.FirstOrDefault();
            
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var userRoles = await _mongoUserRoleCollection
                .CountDocumentsAsync(item => 
                    item.UserId == user.Id && item.RoleId == role.Id,
                    cancellationToken: cancellationToken);

            return userRoles > 0;
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var roleCursor = await _mongoRoleCollection
                .FindAsync(item => item.NormalizedName == roleName, cancellationToken: cancellationToken);

            var role = roleCursor.FirstOrDefault();
            
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var userRoleCursor = await _mongoUserRoleCollection
                .FindAsync(item => 
                        item.RoleId == role.Id,
                        cancellationToken: cancellationToken);

            var userRoles = userRoleCursor.ToList();
            var userIds = userRoles.Select(item => item.UserId);

            var users = await _mongoUserCollection
                .FindAsync(item => userIds.Contains(item.Id), cancellationToken: cancellationToken);

            return users.ToList();
        }

        #endregion

        #region User Email

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(email) || user == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            user.PasswordHash = email;

            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.EmailConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var query = await _mongoUserCollection
                .FindAsync(item => item.NormalizeEmail == normalizedEmail, cancellationToken: cancellationToken);

            var user = query.FirstOrDefault();

            return user;
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.NormalizeEmail);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.NormalizeEmail = normalizedEmail;

            return Task.FromResult(0);
        }

        #endregion
    }
}