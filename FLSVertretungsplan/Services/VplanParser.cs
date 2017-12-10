using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace FLSVertretungsplan
{
    public class VplanParser
    {
        public static Vplan Parse(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var vPlanNode = doc.DocumentElement;
            var changes = ParseVPlanNode(doc.DocumentElement);
            SortChanges(changes);

            var lastUpdateTimestamp = vPlanNode.Attributes["lastUpdate"]?.Value;
            var lastUpdate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(lastUpdateTimestamp)).DateTime;

            return new Vplan
            {
                Changes = changes,
                LastUpdate = lastUpdate
            };
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
            var day = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(stringDay)).DateTime;

            var change = new Change
            {
                SchoolClass = new SchoolClass(classNode.Attributes["name"]?.Value,
                                              schoolNode.Attributes["name"]?.Value),
                Day = day,
                Hours = oldNode.ChildText("hours"),
                OldLesson = new Lesson
                {
                    Subject = new Subject
                    {
                        Name = oldSubjectNode?.ChildText("name"),
                        Identifier = oldSubjectNode?.ChildText("shortcut"),
                    },
                    Teacher = new Teacher
                    {
                        FirstName = oldTeacherNode?.ChildText("firstname"),
                        LastName = oldTeacherNode?.ChildText("lastname"),
                        Identifier = oldTeacherNode?.ChildText("shortcut")
                    },
                    Room = oldNode.ChildText("room")
                },
                NewLesson = new Lesson
                {
                    Room = newNode.ChildText("room")
                },
                Attribute = newNode.ChildText("attribute"),
                Info = newNode.ChildText("info")
            };

            if (newSubjectNode != null)
            {
                change.NewLesson.Subject = new Subject
                {
                    Name = newSubjectNode.ChildText("name"),
                    Identifier = newSubjectNode.ChildText("shortcut"),
                };
            }

            if (newTeacherNode != null)
            {
                change.NewLesson.Teacher = new Teacher
                {
                    FirstName = newTeacherNode.ChildText("firstname"),
                    LastName = newTeacherNode.ChildText("lastname"),
                    Identifier = newTeacherNode.ChildText("shortcut")
                };
            }

            return change;
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
                    return x.SchoolClass.School.CompareTo(y.SchoolClass.School);
                }
                if (x.SchoolClass.Name != y.SchoolClass.Name)
                {
                    return x.SchoolClass.Name.CompareTo(y.SchoolClass.Name);
                }
                return x.Hours.CompareTo(y.Hours);
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
