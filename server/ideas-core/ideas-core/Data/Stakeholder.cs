using CoE.Ideas.Shared.Data;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Core.Data
{
    /// <summary>
    /// People who have some sort of stake in initiatives.
    /// </summary>
    public class Stakeholder : Entity<int>
    {
        public Stakeholder(int id) : base(id) { }

        private Stakeholder() : base() { } 

        public int PersonId { get; private set; }
        public StakeholderType Type { get; private set; }


        internal static Stakeholder Create(int personId, StakeholderType stakeholderType)
        {
            return new Stakeholder() { PersonId = personId, Type = stakeholderType };
        }
    }
}
