using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ServiceInterfaces
{
    public interface IEntityLoader<TEntity>
        where TEntity : class
    {
        void Load<TRelation>(Expression<Func<TEntity, ICollection<TRelation>>> accessor) where TRelation : class;

        void Load<TRelation>(Expression<Func<TEntity, TRelation>> accessor) where TRelation : class;

        IQueryable<TRelation> LoadQuery<TRelation>(Expression<Func<TEntity, ICollection<TRelation>>> accessor) where TRelation : class;

        IQueryable<TRelation> LoadQuery<TRelation>(Expression<Func<TEntity, TRelation>> accessor) where TRelation : class;

        void Reload();
    }
}
