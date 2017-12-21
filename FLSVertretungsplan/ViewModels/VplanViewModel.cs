using System;
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
        public Property<bool> IsRefreshing { get; set; }
        public Collection<ChangePresentationModel> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
    }

    public class TextComponent
    {

        public string PrimaryText;
        public string SecondaryText;
        public Icon IconIdentifier = Icon.None;

        public enum Icon { None, Info, Person, Location };  

    }

    public class ChangePresentationModel {

        public string ClassName { get; set; }
        public string Hours { get; set; }
        public Collection<TextComponent> OldLesson { get; set; }
        public string Type { get; set; }
        public Collection<TextComponent> Description { get; set; }
        public Gradient FillColor { get; set; }

        internal ChangePresentationModel(Change model)
        {
            ClassName = "" + model.SchoolClass.Name;
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

            OldLesson = new Collection<TextComponent> {
                new TextComponent { PrimaryText = model.OldLesson.Subject.Name ?? model.OldLesson.Subject.Identifier },
                new TextComponent { SecondaryText = " bei " },
                new TextComponent { PrimaryText = oldTeacherName },
                new TextComponent { SecondaryText = " in Raum " },
                new TextComponent { PrimaryText = model.OldLesson.Room },
            };

            Description = new Collection<TextComponent>();

            var subjectChanged = model.NewLesson.Subject != null && model.NewLesson.Subject.Identifier != model.OldLesson.Subject.Identifier;
            var teacherChanged = model.NewLesson.Teacher != null && model.NewLesson.Teacher.Identifier != model.OldLesson.Teacher.Identifier;
            var roomChanged = model.NewLesson.Room != null && model.NewLesson.Room != model.OldLesson.Room;

            if (teacherChanged)
            {
                Type = "Vertretung";
            }
            else if (subjectChanged)
            {
                Type = "Fach-Änderung";    
            }
            else if (roomChanged)
            {
                Type = "Raum-Änderung";
            }
            else
            {
                Type = "Ausfall";
            }

            if (subjectChanged)
            {
                var text = model.NewLesson.Subject.Name ?? model.NewLesson.Subject.Identifier;
                Description.Add(new TextComponent { IconIdentifier = TextComponent.Icon.Info });
                Description.Add(new TextComponent { SecondaryText = "Fach " });
                Description.Add(new TextComponent { PrimaryText = text });
                Description.Add(new TextComponent { SecondaryText = "\n" });
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

                Description.Add(new TextComponent { IconIdentifier = TextComponent.Icon.Person });
                Description.Add(new TextComponent { PrimaryText = text});
                Description.Add(new TextComponent { SecondaryText = "\n" });
            }
            if (roomChanged)
            {
                Description.Add(new TextComponent { IconIdentifier = TextComponent.Icon.Location });
                Description.Add(new TextComponent { SecondaryText = "Raum " });
                Description.Add(new TextComponent { PrimaryText = model.NewLesson.Room });
                Description.Add(new TextComponent { SecondaryText = "\n" });
            }
            if (model.Info != null || model.Attribute != null)
            {
                Description.Add(new TextComponent { IconIdentifier = TextComponent.Icon.Info });
                if (model.Info != null)
                {
                    Description.Add(new TextComponent { PrimaryText = model.Info });
                }
                if (model.Info != null && model.Attribute != null)
                {
                    Description.Add(new TextComponent { SecondaryText = " - " });
                }
                if (model.Attribute != null)
                {
                    Description.Add(new TextComponent { PrimaryText =  model.Attribute });
                }
            }
            // Trim trailing newline
            if (Description.Count > 0 && Description[Description.Count - 1].SecondaryText == "\n")
            {
                Description.RemoveAt(Description.Count - 1);
            }
        }

    }

    public class VplanViewModel
    {
        IVplanDataStore DataStore => ServiceLocator.Instance.Get<IVplanDataStore>();

        public Property<string> LastUpdate { get; private set; }
        public Property<bool> IsRefreshing { get; private set; }
        public Property<List<DatePresentationModel>> Dates { get; private set; }
        public Property<bool> ShowsNewClasses { get; private set; }
        public Command LoadItemsCommand { get; }

        public bool UseBookmarkedVplan { get; private set; }

        public VplanViewModel(bool useBookmarkedVplan)
        {
            UseBookmarkedVplan = useBookmarkedVplan;
            LastUpdate = new Property<string>() {
                Value = ""
            };
            IsRefreshing = DataStore.IsRefreshing;
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
            try
            {
                await DataStore.Refresh();
            }
            catch (Exception ex)
            {
                LastUpdate.Value = "Laden fehlgeschlagen";
                Debug.WriteLine(ex);
            }
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
                LastUpdate.Value = "";
                return;
            }

            var dates = new List<DatePresentationModel>();
            var datesDict = new Dictionary<string, DatePresentationModel>();
            LastUpdate.Value = "Letzte Aktualisierung: " + vplan.LastUpdate.ToString("dd.MM.yy hh:mm") + " h";
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
                        Title = item.Day.ToString("dddd", new CultureInfo("de-DE")),
                        SubTitle = date,
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
