using GestionAssociatifERP.Infrastructure.Persistence;
using GestionAssociatifERP.Infrastructure.Persistence.Models;

namespace GestionAssociatifERP.Repositories
{
    public class PersonalSituationRepository : GenericRepository<PersonalSituation>, IPersonalSituationRepository
    {
        public PersonalSituationRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}