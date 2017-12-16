using System;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public class Teacher
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Identifier { get; private set; }

        public Teacher(string firstName, string lastName, string identifier)
        {
            FirstName = firstName;
            LastName = lastName;
            Identifier = identifier;
        }

        public override bool Equals(object obj)
        {
            var teacher = obj as Teacher;
            return teacher != null &&
                   Identifier == teacher.Identifier;
        }

        public override int GetHashCode()
        {
            return 1186239758 + EqualityComparer<string>.Default.GetHashCode(Identifier);
        }
    }
}
