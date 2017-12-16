using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FLSVertretungsplan
{
    public class ChipPresentationModel
    {
        public string Name { get; private set; }
        public int Color { get; private set; }

        static readonly Dictionary<string, int> Colors = new Dictionary<string, int>
        {
            { "BG", 0x5d94cc },
            { "BS", 0x73db43 },
            { "BFS", 0xdd7b35 },
            { "HBFS", 0x1ec9a4 }
        };

        public ChipPresentationModel(string name, int color)
        {
            Name = name;
            Color = color;
        }

        public static ChipPresentationModel Create(SchoolBookmark bookmark)
        {
            return new ChipPresentationModel(bookmark.School,
                                             bookmark.Bookmarked ? GetColor(bookmark.School) : 0xdbdcdd);
        }

        public static ChipPresentationModel Create(SchoolClassBookmark bookmark)
        {
            return new ChipPresentationModel(bookmark.SchoolClass.Name,
                                             bookmark.Bookmarked ? GetColor(bookmark.SchoolClass.School) : 0xdbdcdd);
        }

        private static int GetColor(string school)
        {
            if (!Colors.ContainsKey(school))
            {
                return 0xc91d1d;
            }
            return Colors[school];
        }
    }

    public class ChipViewModel<T> where T : class
    {
        ObservableCollection<T> Entries;
        Func<T, ChipPresentationModel> Factory;
        public ObservableCollection<ChipPresentationModel> PresentationModels;

        public ChipViewModel(ObservableCollection<T> entries, Func<T, ChipPresentationModel> factory)
        {

            Entries = entries;
            Factory = factory;

            PresentationModels = new ObservableCollection<ChipPresentationModel>();
            Load();
            Entries.CollectionChanged += Entries_CollectionChanged;
        }

        public void Load()
        {
            PresentationModels.Clear();
            foreach (T entry in Entries)
            {
                PresentationModels.Add(Factory.Invoke(entry));
            }
        }

        void Entries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        T entry = e.NewItems[i] as T;
                        var newPresentationModel = Factory.Invoke(entry);
                        PresentationModels.Insert(e.NewStartingIndex + i, newPresentationModel);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        PresentationModels.RemoveAt(e.OldStartingIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        T entry = e.NewItems[i] as T;
                        var newPresentationModel = Factory.Invoke(entry);
                        PresentationModels[e.NewStartingIndex + i] = newPresentationModel;
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotSupportedException("Moving Schools is not supported");
                case NotifyCollectionChangedAction.Reset:
                    Load();
                    break;
            }
        }

    }
}
