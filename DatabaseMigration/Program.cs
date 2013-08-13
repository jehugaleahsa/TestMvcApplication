using System.Data.Entity;
using DataModeling.DataModel;

namespace DatabaseMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            IDatabaseInitializer<EntityContext> initializer = new EntityContextInitializer();
            EntityContext context = new EntityContext();
            initializer.InitializeDatabase(context);
        }
    }
}
