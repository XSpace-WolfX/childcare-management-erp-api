using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

namespace ChildcareManagementERP.Api.Repositories
{
    public interface IGuardianChildRepository : IGenericRepository<GuardianChild>
    {
        Task<IEnumerable<GuardianChild>> GetGuardiansByChildIdAsync(int childId);
        Task<IEnumerable<GuardianChild>> GetChildrenByGuardianIdAsync(int guardianId);
        Task<GuardianChild?> GetLinkAsync(int guardianId, int childId);
        Task<bool> LinkExistsAsync(int guardianId, int childId);
        Task RemoveLinkAsync(int guardianId, int childId);
    }
}