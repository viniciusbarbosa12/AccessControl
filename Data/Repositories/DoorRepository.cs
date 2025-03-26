using Data.Context;
using Data.Repositories.Base;
using Data.Repositories.Interfaces;
using Domain.Entities;

namespace Data.Repositories
{
    public class DoorRepository : Repository<Door>, IDoorRepository
    {
        public DoorRepository(AppDbContext context) : base(context)
        {
        }
    }
}
