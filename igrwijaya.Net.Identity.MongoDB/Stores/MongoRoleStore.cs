using igrwijaya.Net.Identity.MongoDB.Roles;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace igrwijaya.Net.Identity.MongoDB.Stores
{
    public class MongoRoleStore<TRole> : IRoleStore<TRole> where TRole : MongoIdentityRole
    {
        #region Private Methods

        private readonly IMongoCollection<TRole> _mongoCollection;

        #endregion

        public MongoRoleStore()
        {
            var client = new MongoClient(
                "mongodb+srv://<username>:<password>@<cluster-address>/test?w=majority"
            );
            
            var mongoDatabase = client.GetDatabase("test");
            _mongoCollection = mongoDatabase.GetCollection<TRole>("AspNet_Roles");
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

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.Id = Guid.NewGuid().ToString("N");

            await _mongoCollection.InsertOneAsync(role, cancellationToken: cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            await _mongoCollection
                .ReplaceOneAsync(item => item.Id == role.Id, role, cancellationToken: cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            await _mongoCollection
                .DeleteOneAsync(item => item.Id == role.Id, cancellationToken);

            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.Name = roleName;

            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(normalizedName))
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            cancellationToken.ThrowIfCancellationRequested();
            var identityRole = await ReadRoleAsync(roleId, cancellationToken);

            return identityRole;
        }

        public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var query = await _mongoCollection
                .FindAsync(item => item.NormalizedName == normalizedRoleName, cancellationToken: cancellationToken);

            return query.FirstOrDefault();
        }

        private async Task<TRole> ReadRoleAsync(string roleId, CancellationToken cancellationToken)
        {
            var query = await _mongoCollection
                .FindAsync(item => item.Id == roleId, cancellationToken: cancellationToken);

            return query.FirstOrDefault();
        }
    }
}