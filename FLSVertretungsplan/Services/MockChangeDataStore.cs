using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLSVertretungsplan
{
    public class MockChangeDataStore: IChangeDataStore
    {

        List<Change> changes;

        public MockChangeDataStore()
        {
            changes = new List<Change>
            {
                new Change { 
                    ClassName = "1212", 
                    Day = DateTime.Now, 
                    Hours = "1.-3.", 
                    OldLesson = new Lesson { 
                        Room = "A102", 
                        Subject = new Subject
                        {
                            Name = "Informatik",
                            Identifier = "EDV"
                        }, 
                        Teacher = new Teacher
                        {
                            FirstName = "Michael",
                            LastName = "Schlosser",
                            Identifier = "SCLO"
                        }
                    },
                    NewLesson = new Lesson { 
                        Room = "A103", 
                        Subject = new Subject
                        {
                            Name = "Informatik",
                            Identifier = "EDV"
                        }, 
                        Teacher = new Teacher
                        {
                            FirstName = "Michael",
                            LastName = "Schlosser",
                            Identifier = "SCLO"
                        }
                    },
                    Attribute = "Bla",
                    Info = "Blub"
                },
                new Change {
                    ClassName = "1212",
                    Day = DateTime.Now,
                    Hours = "1.-3.",
                    OldLesson = new Lesson {
                        Room = "A102",
                        Subject = new Subject
                        {
                            Name = "Informatik",
                            Identifier = "EDV"
                        },
                        Teacher = new Teacher
                        {
                            FirstName = "Michael",
                            LastName = "Schlosser",
                            Identifier = "SCLO"
                        }
                    },
                    NewLesson = new Lesson {
                        
                    }
                },
                new Change {
                    ClassName = "1212",
                    Day = DateTime.Now.AddDays(1),
                    Hours = "1.-3.",
                    OldLesson = new Lesson {
                        Room = "A102",
                        Subject = new Subject
                        {
                            Name = "Informatik",
                            Identifier = "EDV"
                        },
                        Teacher = new Teacher
                        {
                            FirstName = "Michael",
                            LastName = "Schlosser",
                            Identifier = "SCLO"
                        }
                    },
                    NewLesson = new Lesson {
                        Room = "A102",
                        Subject = new Subject
                        {
                            Name = "Informatik",
                            Identifier = "EDV"
                        },
                        Teacher = new Teacher
                        {
                            FirstName = "Bi",
                            LastName = "Ba",
                            Identifier = "BIBA"
                        }
                    }
                },
            };
        }

        public async Task<IEnumerable<Change>> GetChangesAsync(bool forceRefresh = false)
        {
            return changes;
        }
    }
}
