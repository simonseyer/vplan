using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLSVertretungsplan
{
    public class MockChangeDataStore: IChangeDataStore
    {

        Vplan vplan;

        public MockChangeDataStore()
        {
            List<Change> changes = new List<Change>
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
            vplan = new Vplan
            {
                Changes = changes,
                LastUpdate = DateTime.Now
            };
        }

        public async Task<Vplan> GetVplanAsync(bool forceRefresh = false)
        {
            return vplan;
        }

    }
}
