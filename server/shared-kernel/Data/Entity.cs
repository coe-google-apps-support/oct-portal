using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoE.Ideas.Shared.Data
{
    // based on "Domain-Driven Design Fundamentals" PluralSight course by Julie Lerman and Steve Smith
    public abstract class Entity<TId> : IEquatable<Entity<TId>>, IAuditEntity
    {
        public TId Id { get; protected set; }

        protected Entity(TId id)
        {
            if (object.Equals(id, default(TId)))
            {
                throw new ArgumentException("The ID Cannot be the type's default value.", "id");
            }

            this.Id = id;
        }

        // EF requires an empty constructor
        protected Entity() { }


        public AuditRecord AuditRecord { get; private set; }


        // For simple entities, this may suffice
        // As Evans notes earlier in Julie Lerman's DDD course, 
        /// equality of Entities is frequently not a simple operator
        public override bool Equals(object otherObject)
        {
            var entity = otherObject as Entity<TId>;
            if (entity != null)
            {
                return this.Equals(entity);
            }
            return base.Equals(otherObject);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(Entity<TId> other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Id.Equals(other.Id);
        }
    }
}