using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace FLSVertretungsplan
{
    public static class VplanParser
    {

        static Int64 TimeZoneOffset = 7200;

        public static async Task<Vplan> Parse(string xml)
        {
            return await Task.Run(() =>
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);

                var vPlanNode = doc.DocumentElement;
                var changes = ParseVPlanNode(doc.DocumentElement);
                SortChanges(changes);

                var lastUpdateTimestamp = vPlanNode.Attributes["lastUpdate"]?.Value;
                var lastUpdate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(lastUpdateTimestamp)).DateTime;

                return new Vplan(lastUpdate, changes);
            });
        }

        private static List<Change> ParseVPlanNode(XmlNode vplanNode)
        {
            var changes = new List<Change>();
            foreach (XmlNode schoolNode in vplanNode.ChildNodes)
            {
                changes.AddRange(ParseSchoolNode(schoolNode));
            }
            return changes;
        }

        private static List<Change> ParseSchoolNode(XmlNode schoolNode)
        {
            var changes = new List<Change>();
            foreach (XmlNode classNode in schoolNode.ChildNodes)
            {
                changes.AddRange(ParseClassNode(schoolNode, classNode));
            }
            return changes;
        }

        private static List<Change> ParseClassNode(XmlNode schoolNode, XmlNode classNode)
        {
            var changes = new List<Change>();
            foreach (XmlNode dateNode in classNode.ChildNodes)
            {
                changes.AddRange(ParseDateNode(schoolNode, classNode, dateNode));
            }
            return changes;
        }

        private static List<Change> ParseDateNode(XmlNode schoolNode, XmlNode classNode, XmlNode dateNode)
        {
            var changes = new List<Change>();
            foreach (XmlNode lessonNode in dateNode.ChildNodes)
            {
                changes.Add(ParseLessonNode(schoolNode, classNode, dateNode, lessonNode));
            }
            return changes;
        }

        private static Change ParseLessonNode(XmlNode schoolNode, XmlNode classNode, XmlNode dateNode, XmlNode lessonNode)
        {
            XmlNode oldNode = lessonNode.ChildNodes.Item("original");
            XmlNode newNode = lessonNode.ChildNodes.Item("substitute");

            var oldSubjectNode = oldNode.ChildNodes.Item("subject");
            var newSubjectNode = newNode.ChildNodes.Item("subject");

            var oldTeacherNode = oldNode.ChildNodes.Item("tutor");
            var newTeacherNode = newNode.ChildNodes.Item("tutor");

            var stringDay = dateNode.Attributes["timestamp"]?.Value;
            var day = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(stringDay) + TimeZoneOffset).DateTime;
            var hours = oldNode.ChildText("hours");

            var schoolClass = new SchoolClass(classNode.Attributes["name"]?.Value,
                                              schoolNode.Attributes["name"]?.Value);

            var oldLesson = new Lesson(new Subject(oldSubjectNode?.ChildText("shortcut"),
                                                   oldSubjectNode?.ChildText("name")),
                                       new Teacher(oldTeacherNode?.ChildText("firstname"),
                                                   oldTeacherNode?.ChildText("lastname"),
                                                   oldTeacherNode?.ChildText("shortcut")),
                                       oldNode.ChildText("room"));

            var attribute = newNode.ChildText("attribute");
            var info = newNode.ChildText("info");

            Subject newSubject = null;
            if (newSubjectNode != null)
            {
                newSubject = new Subject(newSubjectNode.ChildText("shortcut"),
                                         newSubjectNode.ChildText("name"));
            }

            Teacher newTeacher = null;
            if (newTeacherNode != null)
            {
                newTeacher = new Teacher(newTeacherNode.ChildText("firstname"),
                                         newTeacherNode.ChildText("lastname"),
                                         newTeacherNode.ChildText("shortcut"));
            }

            var newLesson = new Lesson(newSubject,
                                       newTeacher,
                                       newNode.ChildText("room"));

            return new Change(schoolClass, day, hours, oldLesson, newLesson, attribute, info);
        }

        private static void SortChanges(List<Change> changes)
        {
            changes.Sort(delegate (Change x, Change y)
            {
                if (x.Day != y.Day)
                {
                    return x.Day.CompareTo(y.Day);
                }
                if (x.SchoolClass.School != y.SchoolClass.School)
                {
                    return string.Compare(x.SchoolClass.School, y.SchoolClass.School, StringComparison.CurrentCulture);
                }
                if (x.SchoolClass.Name != y.SchoolClass.Name)
                {
                    return string.Compare(x.SchoolClass.Name, y.SchoolClass.Name, StringComparison.CurrentCulture);
                }
                return string.Compare(x.Hours, y.Hours, StringComparison.CurrentCulture);
            });
        }

    }

    static class Extensions
    {
        public static XmlNode Item(this XmlNodeList list, String name)
        {
            foreach (XmlNode childNode in list)
            {
                if (childNode.Name == name)
                {
                    return childNode;
                }
            }
            return null;
        }

        public static string ChildText(this XmlNode node, String name)
        {
            var value = node.ChildNodes.Item(name)?.InnerText;
            if (value != null)
            {
                value = value.Trim();
                if (value.Length == 0)
                {
                    value = null;
                }
            }
            return value;
        }
    }

}
