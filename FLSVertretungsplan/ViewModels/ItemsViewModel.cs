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
            OldLesson = string.Format("{0} bei {1}. {2} in Raum {3}", 
                                      model.OldLesson.Subject.Name ?? model.OldLesson.Subject.Identifier, 
                                      model.OldLesson.Teacher.FirstName[0],
                                      model.OldLesson.Teacher.LastName,
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
                Description += string.Format("bei {0}. {1} ", 
                                             model.NewLesson.Teacher.FirstName[0],
                                             model.NewLesson.Teacher.LastName);
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

    public class ItemsViewModel : BaseViewModel
    {
        public Property<List<DatePresentationModel>> Dates { get; set; }
        public Property<string> LastUpdate { get; set; }
        public Command LoadItemsCommand { get; }
        public bool BookmarkedVplan { get; private set; }

        public ItemsViewModel(bool bookmarkedVplan)
        {
            BookmarkedVplan = bookmarkedVplan;
            Title = "Vertretungsplan";
            LastUpdate = new Property<string>() {
                Value = ""
            };
            Dates = new Property<List<DatePresentationModel>>()
            {
                Value = new List<DatePresentationModel>()
            };
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            if (BookmarkedVplan)
            {
                DataStore.GetBookmarkedVplan().PropertyChanged += VplanChanged;
                VplanChanged(null, null);
            }
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var plan = await DataStore.GetVplanAsync(true);
                if (!BookmarkedVplan)
                {
                    updateData(plan);
                }
            }
            catch (Exception ex)
            {
                LastUpdate.Value = "Laden fehlgeschlagen";
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void VplanChanged(object sender, EventArgs e)
        {
            updateData(DataStore.GetBookmarkedVplan().Value);
        }

        private void updateData(Vplan vplan)
        {
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
