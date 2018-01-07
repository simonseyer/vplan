using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FLSVertretungsplan
{
    public class ChipViewModel
    {
        public readonly string Name;
        public readonly bool Filled;
        public readonly RgbColor OutlineColor;
        public readonly Gradient FillColor;

        public ChipViewModel(string name, bool filled, RgbColor outlineColor, Gradient fillColor)
        {
            Name = name;
            Filled = filled;
            OutlineColor = outlineColor;
            FillColor = fillColor;
        }

        public static ChipViewModel Create(SchoolBookmark bookmark)
        {
            return new ChipViewModel(bookmark.School,
                                     bookmark.Bookmarked,
                                     Color.GetOutlineColor(bookmark.School),
                                     Color.GetFillColor(bookmark.School));
        }

        public static ChipViewModel Create(SchoolClassBookmark bookmark)
        {
            return new ChipViewModel(bookmark.SchoolClass.Name,
                                     bookmark.Bookmarked,
                                     Color.GetOutlineColor(bookmark.SchoolClass.School),
                                     Color.GetFillColor(bookmark.SchoolClass.School));
        }
    }

    public class ChipCollectionViewModel<T> where T : class
    {
        readonly ObservableCollection<T> Entries;
        readonly Func<T, ChipViewModel> Factory;

        public readonly ObservableCollection<ChipViewModel> ChipViewModels;

        public ChipCollectionViewModel(ObservableCollection<T> entries, Func<T, ChipViewModel> factory)
        {
            Entries = entries;
            Factory = factory;

            ChipViewModels = new ObservableCollection<ChipViewModel>();
            Load();
            Entries.CollectionChanged += Entries_CollectionChanged;
        }

        public void Load()
        {
            ChipViewModels.Clear();
            foreach (T entry in Entries)
            {
                ChipViewModels.Add(Factory.Invoke(entry));
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
                        var newViewModel = Factory.Invoke(entry);
                        ChipViewModels.Insert(e.NewStartingIndex + i, newViewModel);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        ChipViewModels.RemoveAt(e.OldStartingIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        T entry = e.NewItems[i] as T;
                        var newViewModel = Factory.Invoke(entry);
                        ChipViewModels[e.NewStartingIndex + i] = newViewModel;
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
