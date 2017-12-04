using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace FLSVertretungsplan
{
    public class CloudChangeDataStore: IChangeDataStore
    {

        HttpClient client;
        Vplan vplan;

        public CloudChangeDataStore()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri($"{App.BackendUrl}/");

            vplan = new Vplan {
                Changes = new List<Change>()
            };
        }

        public async Task<Vplan> GetVplanAsync(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                var json = await client.GetStringAsync($"raw/vplan?version=1.2.4");
                vplan = await Task.Run(() => DeserializeChanges(json));
            }
            return vplan;
        }

        private Vplan DeserializeChanges(string xml)
        {
            var parsedChanges = new List<Change>();
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var vPlanNode = doc.DocumentElement;

            Debug.Assert(vPlanNode != null);
            Debug.Assert(vPlanNode.ChildNodes.Count > 0);

            var lastUpdateTimestamp = vPlanNode.Attributes["lastUpdate"]?.Value;
            var lastUpdate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(lastUpdateTimestamp)).DateTime;

            foreach (XmlNode schoolNode in vPlanNode.ChildNodes)
            {
                Debug.Assert(schoolNode.ChildNodes.Count > 0);
                foreach (XmlNode classNode in schoolNode.ChildNodes)
                {
                    Debug.Assert(classNode.ChildNodes.Count > 0);
                    foreach (XmlNode dateNode in classNode.ChildNodes)
                    {
                        Debug.Assert(dateNode.ChildNodes.Count > 0);
                        foreach (XmlNode lessonNode in dateNode.ChildNodes)
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
                                SchoolName = dateNode.Attributes["name"]?.Value,
                                ClassName = classNode.Attributes["name"]?.Value,
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

                            parsedChanges.Add(change);
                        }
                    }
                }
            }

            parsedChanges.Sort(delegate (Change x, Change y)
            {
                if (x.Day != y.Day)
                {
                    return x.Day.CompareTo(y.Day);
                }
                if (x.ClassName != y.ClassName)
                {
                    return x.ClassName.CompareTo(y.ClassName);
                }
                return x.Hours.CompareTo(y.Hours);
            });

            return new Vplan
            {
                Changes = parsedChanges,
                LastUpdate = lastUpdate
            };
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
