using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoE.Ideas.Shared.Security
{
    [Table("PermissionRoles")]
    internal class PermissionRole
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Permssion { get; set; }
    }
}
