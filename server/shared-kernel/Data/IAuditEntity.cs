using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Ideas.Shared.Data
{
    public interface IAuditEntity
    {
        AuditRecord AuditRecord { get; }
    }
}
