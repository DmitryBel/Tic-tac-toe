namespace Gameserver.Core
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model : DbContext
    {
        public Model()
            : base("name=Model")
        {
        }

        public virtual DbSet<games> games { get; set; }
        public virtual DbSet<logs> logs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<games>()
                .HasMany(e => e.logs)
                .WithOptional(e => e.games)
                .HasForeignKey(e => e.gameID);
        }
    }
}
