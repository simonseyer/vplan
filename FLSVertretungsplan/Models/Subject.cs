using System;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public class Subject
    {
        public string Identifier { get; private set; }
        public string Name { get; private set; }

        public Subject(string identifier, string name)
        {
            Identifier = identifier;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var subject = obj as Subject;
            return subject != null &&
                   Name == subject.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }
}
