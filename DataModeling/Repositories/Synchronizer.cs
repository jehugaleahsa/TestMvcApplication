using System;
using DataModeling.DataModel;
using ServiceInterfaces.Repositories;

namespace DataModeling.Repositories
{
    public sealed class Synchronizer : ISynchronizer
    {
        private readonly EntitySet entities;

        public Synchronizer(EntitySet entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            this.entities = entities;
        }

        public int Synchronize()
        {
            return entities.SaveChanges();
        }
    }
}
