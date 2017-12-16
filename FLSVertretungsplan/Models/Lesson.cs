using System;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public class Lesson
    {
        public Subject Subject { get; private set; }
        public Teacher Teacher { get; private set; }
        public string Room { get; private set; }

        public Lesson(Subject subject, Teacher teacher, string room)
        {
            Subject = subject;
            Teacher = teacher;
            Room = room;
        }

        public override bool Equals(object obj)
        {
            var lesson = obj as Lesson;
            return lesson != null &&
                   EqualityComparer<Subject>.Default.Equals(Subject, lesson.Subject) &&
                   EqualityComparer<Teacher>.Default.Equals(Teacher, lesson.Teacher) &&
                   Room == lesson.Room;
        }

        public override int GetHashCode()
        {
            var hashCode = -1473767562;
            hashCode = hashCode * -1521134295 + EqualityComparer<Subject>.Default.GetHashCode(Subject);
            hashCode = hashCode * -1521134295 + EqualityComparer<Teacher>.Default.GetHashCode(Teacher);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Room);
            return hashCode;
        }
    }
}
