using ChildcareManagementERP.Api.Infrastructure.Persistence;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

namespace ChildcareManagementERP.Api.Repositories
{
    public class PersonalSituationRepository : GenericRepository<PersonalSituation>, IPersonalSituationRepository
    {
        public PersonalSituationRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}