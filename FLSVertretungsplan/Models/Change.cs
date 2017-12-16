using System;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public class Change
    {
        public SchoolClass SchoolClass { get; private set; }
        public DateTime Day { get; private set; }
        public string Hours { get; private set; }

        public Lesson OldLesson { get; private set; }
        public Lesson NewLesson { get; private set; }

        public string Attribute { get; private set; }
        public string Info { get; private set; }

        public Change(SchoolClass schoolClass, DateTime day, string hours, Lesson oldLesson, Lesson newLesson, string attribute, string info)
        {
            SchoolClass = schoolClass;
            Day = day;
            Hours = hours;
            OldLesson = oldLesson;
            NewLesson = newLesson;
            Attribute = attribute;
            Info = info;
        }

        public override bool Equals(object obj)
        {
            var change = obj as Change;
            return change != null &&
                   EqualityComparer<SchoolClass>.Default.Equals(SchoolClass, change.SchoolClass) &&
                   Day == change.Day &&
                   Hours == change.Hours &&
                   EqualityComparer<Lesson>.Default.Equals(OldLesson, change.OldLesson) &&
                   EqualityComparer<Lesson>.Default.Equals(NewLesson, change.NewLesson) &&
                   Attribute == change.Attribute &&
                   Info == change.Info;
        }

        public override int GetHashCode()
        {
            var hashCode = -1259238801;
            hashCode = hashCode * -1521134295 + EqualityComparer<SchoolClass>.Default.GetHashCode(SchoolClass);
            hashCode = hashCode * -1521134295 + Day.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Hours);
            hashCode = hashCode * -1521134295 + EqualityComparer<Lesson>.Default.GetHashCode(OldLesson);
            hashCode = hashCode * -1521134295 + EqualityComparer<Lesson>.Default.GetHashCode(NewLesson);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Attribute);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Info);
            return hashCode;
        }
    }
}
