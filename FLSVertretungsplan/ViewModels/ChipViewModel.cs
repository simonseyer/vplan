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

    public class Gradient
    {
        public readonly RgbColor Start;
        public readonly RgbColor End;

        public Gradient(RgbColor start, RgbColor end)
        {
            Start = start;
            End = end;
        }
    }

    public class ChipPresentationModel
    {
        public string Name { get; private set; }
        public bool Filled { get; private set; }
        public RgbColor OutlineColor { get; private set; }
        public Gradient FillColor { get; private set; }

        static readonly Dictionary<string, RgbColor> OutlineColors = new Dictionary<string, RgbColor>
        {
            { "BG", new RgbColor(254, 57, 94) },
            { "BS", new RgbColor(30, 154, 249) },
            { "BFS", new RgbColor(28, 200, 142) },
            { "HBFS", new RgbColor(113, 63, 192) }
        };
        static readonly Dictionary<string, Gradient> FillColors = new Dictionary<string, Gradient>
        {
            { "BG", new Gradient(new RgbColor(254, 150, 116), new RgbColor(254, 57, 94)) },
            { "BS", new Gradient(new RgbColor(30, 154, 249), new RgbColor(41, 182, 199)) },
            { "BFS", new Gradient(new RgbColor(27, 201, 182), new RgbColor(28, 200, 142)) },
            { "HBFS", new Gradient(new RgbColor(113, 63, 192), new RgbColor(135, 70, 231)) }
        };

        public ChipPresentationModel(string name, bool filled, RgbColor outlineColor, Gradient fillColor)
        {
            Name = name;
            Filled = filled;
            OutlineColor = outlineColor;
            FillColor = fillColor;
        }

        public static ChipPresentationModel Create(SchoolBookmark bookmark)
        {
            return new ChipPresentationModel(bookmark.School,
                                             bookmark.Bookmarked,
                                             GetOutlineColor(bookmark.School),
                                             GetFillColor(bookmark.School));
        }

        public static ChipPresentationModel Create(SchoolClassBookmark bookmark)
        {
            return new ChipPresentationModel(bookmark.SchoolClass.Name,
                                             bookmark.Bookmarked,
                                             GetOutlineColor(bookmark.SchoolClass.School),
                                             GetFillColor(bookmark.SchoolClass.School));
        }

        public static RgbColor GetOutlineColor(string school)
        {
            if (!OutlineColors.ContainsKey(school))
            {
                return new RgbColor(201, 29, 29);
            }
            return OutlineColors[school];
        }

        public static Gradient GetFillColor(string school)
        {
            if (!FillColors.ContainsKey(school))
            {
                return new Gradient(new RgbColor(201, 29, 29), new RgbColor(201, 29, 29));
            }
            return FillColors[school];
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
