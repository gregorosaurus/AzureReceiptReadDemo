#nullable disable
using System;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace AzFuncProcessReceipts.Model
{
    public class ReceiptsContext : DbContext
    {
        public ReceiptsContext(DbContextOptions<ReceiptsContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            #region  Schema Cutomizations
            var entities = modelBuilder.Model.GetEntityTypes();
            //we set all foreign keys to be cascade. 
            foreach (var entityType in entities)
            {
                entityType.GetForeignKeys().ToList().ForEach(x => x.DeleteBehavior = DeleteBehavior.Cascade);
            }
            #endregion

        }

        public DbSet<Receipt> Receipts { get; set; }
    }
}

