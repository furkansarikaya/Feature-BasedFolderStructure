using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Entities.Users;

namespace FeatureBasedFolderStructure.Domain.Interfaces.Users;

public interface IRoleRepository : IRepository<Role, int>
{
    Task<Role?> GetByNameAsync(string roleName);
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId);
    Task<bool> RoleExistsAsync(string roleName);
    Task<IEnumerable<RoleClaim>> GetRoleClaimsAsync(int roleId);
}