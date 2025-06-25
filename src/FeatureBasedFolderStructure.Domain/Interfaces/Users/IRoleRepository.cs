using FeatureBasedFolderStructure.Domain.Entities.Users;
using FS.EntityFramework.Library.Interfaces;

namespace FeatureBasedFolderStructure.Domain.Interfaces.Users;

public interface IRoleRepository : IRepository<Role, int>
{
    Task<Role?> GetByNameAsync(string roleName);
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId);
    Task<bool> RoleExistsAsync(string roleName);
    Task<IEnumerable<RoleClaim>> GetRoleClaimsAsync(int roleId);
}