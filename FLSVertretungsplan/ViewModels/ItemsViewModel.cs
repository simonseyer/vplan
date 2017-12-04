using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FLSVertretungsplan
{
    public class ChangePresentationModel {

        public string ClassName { get; set; }
        public string Hours { get; set; }
        public string OldLesson { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool Canceled;

        internal ChangePresentationModel(Change model)
        {
            ClassName = "" + model.ClassName;
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
        public ObservableCollection<String> Dates { get; }
        public ObservableCollection<Collection<ChangePresentationModel>> Changes { get; }
        public Command LoadItemsCommand { get; }

        public ItemsViewModel()
        {
            Title = "Vertretungsplan";
            Dates = new ObservableCollection<String>();
            Changes = new ObservableCollection<Collection<ChangePresentationModel>>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Dates.Clear();
                Changes.Clear();

                var items = await DataStore.GetChangesAsync(true);
                foreach (var item in items)
                {
                    var date = item.Day.ToString("dd.MM.yy");
                    var dateIndex = Dates.IndexOf(date);
                    if (dateIndex < 0)
                    {
                        dateIndex = Dates.Count;
                        Dates.Add(date);
                        Changes.Add(new Collection<ChangePresentationModel>());
                    }

                    Changes[dateIndex].Add(new ChangePresentationModel(item));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
