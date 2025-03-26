using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessControl
{
    public class DBContext : DbContext
    {
        public DbSet<Door> Doors;

        public DbSet<Card> Cards;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("accessControl");
        }
    }
}
