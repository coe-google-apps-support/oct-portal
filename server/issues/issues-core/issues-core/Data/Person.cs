using System;
using System.Collections.Generic;
using System.Text;

namespace CoE.Issues.Core.Data
{
    public class Person
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }


        internal static Person Create(int id, string name, string email)
        {
            return new Person() { Id = id, Name = name, Email = email };
        }
    }
}
