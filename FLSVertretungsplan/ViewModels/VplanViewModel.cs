﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace FLSVertretungsplan
{
    public class DatePresentationModel
    {

        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string LastUpdate { get; set; }
        public Property<bool> IsRefreshing { get; set; }
        public Collection<ChangePresentationModel> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
    }

    public class TextComponent
    {

        public string PrimaryText;
        public string SecondaryText;
        public Icon IconIdentifier = Icon.None;

        public enum Icon { None, Info, Person, Location, Subject };  

    }

    public class ChangePresentationModel {

        public string ClassName { get; private set; }
        public string Day { get; private set; }
        public DateTime Date { get; private set; }
        public string Hours { get; private set; }
        public ReadOnlyCollection<TextComponent> OldLesson { get; private set; }
        public string Type { get; private set; }
        public ReadOnlyCollection<TextComponent> Description { get; private set; }
        public Gradient FillColor { get; private set; }
        public ReadOnlyCollection<TextComponent> TextRepresentation { get; private set; }
        public ReadOnlyCollection<TextComponent> EventTitle { get; private set; }
        public ReadOnlyCollection<TextComponent> EventText { get; private set; }

        internal ChangePresentationModel(Change model)
        {
            ClassName = model.SchoolClass.Name;
            Day = model.Day.ToString("ddd d.M.");
            Date = model.Day;
            Hours = model.Hours;
            FillColor = ChipPresentationModel.GetFillColor(model.SchoolClass.School);



            string oldTeacherName;
            if (model.OldLesson.Teacher.FirstName != null && 
                model.OldLesson.Teacher.LastName != null)
            {
                oldTeacherName = string.Format("{0}. {1}", 
                                               model.OldLesson.Teacher.FirstName[0],
                                               model.OldLesson.Teacher.LastName);
            }
            else
            {
                oldTeacherName = model.OldLesson.Teacher.Identifier;
            }

            var oldLesson = new Collection<TextComponent> {
                new TextComponent { PrimaryText = model.OldLesson.Subject.Name ?? model.OldLesson.Subject.Identifier },
                new TextComponent { SecondaryText = " " },
                new TextComponent { SecondaryText = "change_connector_teacher" },
                new TextComponent { SecondaryText = " " },
                new TextComponent { PrimaryText = oldTeacherName },
                new TextComponent { SecondaryText = " " },
                new TextComponent { SecondaryText = "change_connector_room" },
                new TextComponent { SecondaryText = " " },
                new TextComponent { PrimaryText = model.OldLesson.Room },
            };
            OldLesson = new ReadOnlyCollection<TextComponent>(oldLesson);

            var description = new Collection<TextComponent>();

            var subjectChanged = model.NewLesson.Subject != null && model.NewLesson.Subject.Identifier != model.OldLesson.Subject.Identifier;
            var teacherChanged = model.NewLesson.Teacher != null && model.NewLesson.Teacher.Identifier != model.OldLesson.Teacher.Identifier;
            var roomChanged = model.NewLesson.Room != null && model.NewLesson.Room != model.OldLesson.Room;

            if (teacherChanged)
            {
                Type = "change_type_teacher";
            }
            else if (subjectChanged)
            {
                Type = "change_type_subject";    
            }
            else if (roomChanged)
            {
                Type = "change_type_room";
            }
            else
            {
                Type = "change_type_cancelled";
            }

            if (subjectChanged)
            {
                var text = model.NewLesson.Subject.Name ?? model.NewLesson.Subject.Identifier;
                description.Add(new TextComponent { IconIdentifier = TextComponent.Icon.Subject });
                description.Add(new TextComponent { SecondaryText = "change_prefix_subject" });
                description.Add(new TextComponent { SecondaryText = " " });
                description.Add(new TextComponent { PrimaryText = text });
                description.Add(new TextComponent { SecondaryText = "\n" });
            }

            if (teacherChanged)
            {
                String text;

                if (model.NewLesson.Teacher.FirstName != null &&
                    model.NewLesson.Teacher.LastName != null)
                {
                    text = string.Format("{0}. {1} ",
                                         model.NewLesson.Teacher.FirstName[0],
                                         model.NewLesson.Teacher.LastName);
                }
                else
                {
                    text = string.Format("{0} ", model.NewLesson.Teacher.Identifier);
                }

                description.Add(new TextComponent { IconIdentifier = TextComponent.Icon.Person });
                description.Add(new TextComponent { PrimaryText = text});
                description.Add(new TextComponent { SecondaryText = "\n" });
            }
            if (roomChanged)
            {
                description.Add(new TextComponent { IconIdentifier = TextComponent.Icon.Location });
                description.Add(new TextComponent { SecondaryText = "change_prefix_room" });
                description.Add(new TextComponent { SecondaryText = " " });
                description.Add(new TextComponent { PrimaryText = model.NewLesson.Room });
                description.Add(new TextComponent { SecondaryText = "\n" });
            }
            if (model.Info != null || model.Attribute != null)
            {
                description.Add(new TextComponent { IconIdentifier = TextComponent.Icon.Info });
                if (model.Info != null)
                {
                    description.Add(new TextComponent { PrimaryText = model.Info });
                }
                if (model.Info != null && model.Attribute != null)
                {
                    description.Add(new TextComponent { SecondaryText = " - " });
                }
                if (model.Attribute != null)
                {
                    description.Add(new TextComponent { PrimaryText =  model.Attribute });
                }
            }
            // Trim trailing newline
            if (description.Count > 0 && description[description.Count - 1].SecondaryText == "\n")
            {
                description.RemoveAt(description.Count - 1);
            }
            Description = new ReadOnlyCollection<TextComponent>(description);

            var textRepresentation = new List<TextComponent>
            {
                new TextComponent { PrimaryText = model.SchoolClass.School },
                new TextComponent { SecondaryText = " " },
                new TextComponent { PrimaryText = model.SchoolClass.Name },
                new TextComponent { SecondaryText = "\n" },
                new TextComponent { PrimaryText = Day },
                new TextComponent { SecondaryText = ", " },
                new TextComponent { PrimaryText = Hours },
                new TextComponent { SecondaryText = " " },
                new TextComponent { SecondaryText = "change_suffix_time" },
                new TextComponent { SecondaryText = "\n" },
            };
            textRepresentation.AddRange(oldLesson);
            textRepresentation.Add(new TextComponent { SecondaryText = "\n" });
            textRepresentation.Add(new TextComponent { PrimaryText = Type });
            textRepresentation.Add(new TextComponent { SecondaryText = "\n" });
            textRepresentation.AddRange(description);
            TextRepresentation = new ReadOnlyCollection<TextComponent>(textRepresentation);

            var eventTitle = new List<TextComponent>
            {
                new TextComponent { PrimaryText = Type },
                new TextComponent { SecondaryText = " " },
                new TextComponent { PrimaryText = Hours },
                new TextComponent { SecondaryText = " " },
                new TextComponent { SecondaryText = "change_suffix_time" },
            };
            EventTitle = new ReadOnlyCollection<TextComponent>(eventTitle);

            var eventText = new List<TextComponent>
            {
                new TextComponent { PrimaryText = model.SchoolClass.School },
                new TextComponent { SecondaryText = " " },
                new TextComponent { PrimaryText = model.SchoolClass.Name },
                new TextComponent { SecondaryText = "\n" },
            };
            eventText.AddRange(oldLesson);
            eventText.Add(new TextComponent { SecondaryText = "\n" });
            eventText.Add(new TextComponent { PrimaryText = Type });
            eventText.Add(new TextComponent { SecondaryText = "\n" });
            eventText.AddRange(description);
            EventText = new ReadOnlyCollection<TextComponent>(eventText);
        }

    }

    public class VplanViewModel
    {
        IVplanDataStore DataStore => ServiceLocator.Instance.Get<IVplanDataStore>();

        public Property<bool> IsRefreshing { get; private set; }
        public Property<bool> LastRefreshFailed { get; private set; }
        public Property<List<DatePresentationModel>> Dates { get; private set; }
        public Property<bool> ShowsNewClasses { get; private set; }
        public Command LoadItemsCommand { get; }

        public bool UseBookmarkedVplan { get; private set; }

        public VplanViewModel(bool useBookmarkedVplan)
        {
            UseBookmarkedVplan = useBookmarkedVplan;
            IsRefreshing = DataStore.IsRefreshing;
            LastRefreshFailed = DataStore.LastRefreshFailed;
            Dates = new Property<List<DatePresentationModel>>()
            {
                Value = new List<DatePresentationModel>()
            };
            ShowsNewClasses = new Property<bool>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            if (UseBookmarkedVplan)
            {
                DataStore.BookmarkedVplan.PropertyChanged += BookmarkedVplanChanged;
                BookmarkedVplanChanged(null, null);
            }
            else
            {
                DataStore.Vplan.PropertyChanged += VplanChanged;
                VplanChanged(null, null);
            }
        }

        async Task ExecuteLoadItemsCommand()
        {
            await DataStore.Refresh();
        }

        private void VplanChanged(object sender, EventArgs e)
        {
            UpdateData(DataStore.Vplan.Value);
        }

        private void BookmarkedVplanChanged(object sender, EventArgs e)
        {
            UpdateData(DataStore.BookmarkedVplan.Value);
        }

        private void UpdateData(Vplan vplan)
        {
            if (vplan == null)
            {
                Dates.Value = new List<DatePresentationModel>();
                return;
            }

            var dates = new List<DatePresentationModel>();
            var datesDict = new Dictionary<string, DatePresentationModel>();
            var lastUpdate = vplan.LastUpdate.ToString("dd.MM.yy hh:mm");
            foreach (var item in vplan.Changes)
            {
                var date = item.Day.ToString("dd.MM.yy");
                DatePresentationModel presentationModel;
                if (datesDict.ContainsKey(date))
                {
                    presentationModel = datesDict[date];
                }
                else
                {
                    presentationModel = new DatePresentationModel() {
                        Title = item.Day.ToString("dddd"),
                        SubTitle = date,
                        LastUpdate = lastUpdate,
                        IsRefreshing = IsRefreshing,
                        Items = new Collection<ChangePresentationModel>(),
                        LoadItemsCommand = LoadItemsCommand
                    };
                    dates.Add(presentationModel);
                    datesDict[date] = presentationModel;
                }

                presentationModel.Items.Add(new ChangePresentationModel(item));
            }
            Dates.Value = dates;
        }
    }
}
