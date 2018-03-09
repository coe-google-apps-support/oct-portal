using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared
{
    public class EntityNotFoundException : Exception
    {
        public string EntityName { get; private set; }
        public long? Id { get; private set; }


        public EntityNotFoundException(string message) : base(message)
        {
        }

        public EntityNotFoundException(string message, string entityName) : base(message)
        {
            EntityName = entityName;
        }

        public EntityNotFoundException(string message, string entityName, long id) : base(message)
        {
            EntityName = entityName;
            Id = id;
        }

    }
}
