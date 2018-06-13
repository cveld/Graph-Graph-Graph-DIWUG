using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using EventManagerDemo.Models;

namespace EventManagerDemo.Data {
  public class TokenCacheDataContext : DbContext{
    public TokenCacheDataContext()
      : base("EventManagerDemoTokenCacheDataContext") {

            //Database.SetInitializer<TokenCacheDataContext>(new CreateDatabaseIfNotExists<TokenCacheDataContext>());
            Database.SetInitializer<TokenCacheDataContext>(new DropCreateDatabaseIfModelChanges<TokenCacheDataContext>());
        }

    public DbSet<PerUserWebCache> PerUserCacheList { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
      modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
    }
  }
}