using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace FLSVertretungsplan
{
    public class VplanDayViewModel
    {
        public readonly string Title;
        public readonly string SubTitle;
        public readonly string LastUpdate;
        public readonly ReadOnlyCollection<ChangeViewModel> Items;
        public readonly Property<bool> IsRefreshing;
        public readonly Command LoadItemsCommand;

        public VplanDayViewModel(string title, string subTitle, string lastUpdate, Collection<ChangeViewModel> items, Property<bool> isRefreshing, Command loadItemsCommand)
        {
            Title = title;
            SubTitle = subTitle;
            LastUpdate = lastUpdate;
            Items = new ReadOnlyCollection<ChangeViewModel>(items);
            IsRefreshing = isRefreshing;
            LoadItemsCommand = loadItemsCommand;
        }
    }

    class VplanDayViewModelBuilder
    {
        public readonly string Title;
        public readonly string SubTitle;
        public readonly string LastUpdate;
        public readonly Collection<ChangeViewModel> Items;
        public readonly Property<bool> IsRefreshing;
        public readonly Command LoadItemsCommand;

        public VplanDayViewModelBuilder(string title, string subTitle, string lastUpdate, Property<bool> isRefreshing, Command loadItemsCommand)
        {
            Title = title;
            SubTitle = subTitle;
            LastUpdate = lastUpdate;
            Items = new Collection<ChangeViewModel>();
            IsRefreshing = isRefreshing;
            LoadItemsCommand = loadItemsCommand;
        }

        public VplanDayViewModel Build() {
            return new VplanDayViewModel(Title, SubTitle, LastUpdate, Items, IsRefreshing, LoadItemsCommand);
        }
    }

    public class VplanViewModel
    {
        IVplanDataStore DataStore => ServiceLocator.Instance.Get<IVplanDataStore>();

        public readonly Property<bool> IsRefreshing;
        public readonly Property<bool> LastRefreshFailed;
        public readonly Property<Collection<VplanDayViewModel>> Dates;
        public readonly Command LoadItemsCommand;

        public readonly bool MyVplan;

        public VplanViewModel(bool myVplan)
        {
            MyVplan = myVplan;
            IsRefreshing = DataStore.IsRefreshing;
            LastRefreshFailed = DataStore.LastRefreshFailed;
            Dates = new Property<Collection<VplanDayViewModel>>()
            {
                Value = new Collection<VplanDayViewModel>()
            };
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            if (MyVplan)
            {
                DataStore.MyVplan.PropertyChanged += MyVplanChanged;
                MyVplanChanged(null, null);
            }
            else
            {
                DataStore.SchoolVplan.PropertyChanged += SchoolVplanChanged;
                SchoolVplanChanged(null, null);
            }
        }

        async Task ExecuteLoadItemsCommand()
        {
            await DataStore.Refresh();
        }

        private void SchoolVplanChanged(object sender, EventArgs e)
        {
            UpdateData(DataStore.SchoolVplan.Value);
        }

        private void MyVplanChanged(object sender, EventArgs e)
        {
            UpdateData(DataStore.MyVplan.Value);
        }

        private void UpdateData(Vplan vplan)
        {
            if (vplan == null)
            {
                Dates.Value = new Collection<VplanDayViewModel>();
                return;
            }

            var dayViewModelBuilders = new Dictionary<string, VplanDayViewModelBuilder>();
            var lastUpdate = vplan.LastUpdate.ToString("dd.MM.yy hh:mm");
            foreach (var item in vplan.Changes)
            {
                var date = item.Day.ToString("dd.MM.yy");
                VplanDayViewModelBuilder dayViewModelBuilder;
                if (dayViewModelBuilders.ContainsKey(date))
                {
                    dayViewModelBuilder = dayViewModelBuilders[date];
                }
                else
                {
                    dayViewModelBuilder = new VplanDayViewModelBuilder(
                        item.Day.ToString("dddd"),
                        date,
                        lastUpdate,
                        IsRefreshing,
                        LoadItemsCommand
                    );
                    dayViewModelBuilders[date] = dayViewModelBuilder;
                }
                dayViewModelBuilder.Items.Add(new ChangeViewModel(item));
            }

            var dayViewModels = new Collection<VplanDayViewModel>();
            foreach (VplanDayViewModelBuilder builder in dayViewModelBuilders.Values)
            {
                dayViewModels.Add(builder.Build());
            }
            Dates.Value = dayViewModels;
        }
    }
}
