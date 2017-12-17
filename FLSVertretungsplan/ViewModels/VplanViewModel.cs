using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FLSVertretungsplan
{
    public class DatePresentationModel {

        public string Title { get; set; }
        public Collection<ChangePresentationModel> Items { get; set; }
    }

    public class ChangePresentationModel {

        public string ClassName { get; set; }
        public string Hours { get; set; }
        public string OldLesson { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool Canceled;

        internal ChangePresentationModel(Change model)
        {
            ClassName = "" + model.SchoolClass.Name;
            Hours = model.Hours + " h";

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

            OldLesson = string.Format("{0} bei {1} in Raum {2}", 
                                      model.OldLesson.Subject.Name ?? model.OldLesson.Subject.Identifier, 
                                      oldTeacherName,
                                      model.OldLesson.Room);
            Description = "";

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
                Canceled = true;
                Type = "Ausfall";
            }

            if (subjectChanged)
            {
                Description += model.NewLesson.Subject.Name ?? model.NewLesson.Subject.Identifier + " ";
            }
            if (teacherChanged)
            {
                if (model.NewLesson.Teacher.FirstName != null &&
                    model.NewLesson.Teacher.LastName != null)
                {
                    Description += string.Format("bei {0}. {1} ",
                                                 model.NewLesson.Teacher.FirstName[0],
                                                 model.NewLesson.Teacher.LastName);
                }
                else
                {
                    Description += string.Format("{0} ", 
                                                 model.NewLesson.Teacher.Identifier);
                }
            }
            if (roomChanged)
            {
                Description += string.Format("in Raum {0}", model.NewLesson.Room);
            }
            if (model.Info != null)
            {
                if (Description.Length > 0) 
                {
                    Description += "\n";
                }
                Description += model.Info;
            }
            if (model.Attribute != null)
            {
                if (Description.Length > 0)
                {
                    Description += "\n";
                }
                Description += model.Attribute;
            }
        }

    }

    public class VplanViewModel
    {
        private IVplanDataStore DataStore => ServiceLocator.Instance.Get<IVplanDataStore>();

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
                        Title = date,
                        Items = new Collection<ChangePresentationModel>()
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
