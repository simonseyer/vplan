using System;

namespace FLSVertretungsplan
{
    
    public class Change
    {
        public string SchoolName { get; set; }
        public string ClassName { get; set; }
        public DateTime Day { get; set; }
        public string Hours { get; set; }

        public Lesson OldLesson { get; set; }
        public Lesson NewLesson { get; set; }

        public string Attribute { get; set; }
        public string Info { get; set; }
    }

    public class Lesson
    {
        public Subject Subject { get; set; }
        public Teacher Teacher { get; set; }
        public string Room { get; set; }
    }

    public class Teacher
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Identifier { get; set; }
    }

    public class Subject
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
    }
}
