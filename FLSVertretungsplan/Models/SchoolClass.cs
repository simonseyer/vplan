using System;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public class SchoolClass : IComparable
    {

        public string Name { get; private set; }
        public string School { get; private set; }

        public SchoolClass(string name, string school)
        {
            Name = name;
            School = school;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            SchoolClass schoolClass = obj as SchoolClass;
            if (schoolClass != null)
            {
                var schoolComparisonResult = School.CompareTo(schoolClass.School);
                if (schoolComparisonResult != 0)
                {
                    return schoolComparisonResult;
                }
                else
                {
                    return Name.CompareTo(schoolClass.Name);
                }
            }
            else
            {
                throw new ArgumentException("Object is not a SchoolClass");
            }
        }

        public override bool Equals(object obj)
        {
            var @class = obj as SchoolClass;
            return @class != null &&
                   Name == @class.Name &&
                   School == @class.School;
        }

        public override int GetHashCode()
        {
            var hashCode = 134277599;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(School);
            return hashCode;
        }
    }
}
