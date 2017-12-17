using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FLSVertretungsplan
{
    public class RgbColor 
    {
        public readonly byte Red;
        public readonly byte Green;
        public readonly byte Blue;

        public RgbColor(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }
    }

    public class ChipPresentationModel
    {
        public string Name { get; private set; }
        public RgbColor Color { get; private set; }

        static readonly Dictionary<string, RgbColor> Colors = new Dictionary<string, RgbColor>
        {
            { "BG", new RgbColor(93, 148, 204) },
            { "BS", new RgbColor(115, 219, 67) },
            { "BFS", new RgbColor(221, 123, 53) },
            { "HBFS", new RgbColor(30, 201, 164) }
        };

        public ChipPresentationModel(string name, RgbColor color)
        {
            Name = name;
            Color = color;
        }

        public static ChipPresentationModel Create(SchoolBookmark bookmark)
        {
            return new ChipPresentationModel(bookmark.School,
                                             bookmark.Bookmarked ? GetColor(bookmark.School) : new RgbColor(219, 220, 221));
        }

        public static ChipPresentationModel Create(SchoolClassBookmark bookmark)
        {
            return new ChipPresentationModel(bookmark.SchoolClass.Name,
                                             bookmark.Bookmarked ? GetColor(bookmark.SchoolClass.School) : new RgbColor(219, 220, 221));
        }

        private static RgbColor GetColor(string school)
        {
            if (!Colors.ContainsKey(school))
            {
                return new RgbColor(201, 29, 29);
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
