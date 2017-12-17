
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace FLSVertretungsplan.Droid
{
    public class VplanFragment : Android.Support.V4.App.Fragment
    {
        public static VplanFragment NewInstance() =>
            new VplanFragment { Arguments = new Bundle() };

        VplanAdapter adapter;
        SwipeRefreshLayout refresher;

        public static VplanViewModel ViewModel { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewModel = new VplanViewModel(false);
            ViewModel.IsRefreshing.PropertyChanged += IsRefreshing_PropertyChanged;

            View view = inflater.Inflate(Resource.Layout.fragment_vplan, container, false);
            var recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(adapter = new VplanAdapter(Activity, ViewModel));

            refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            refresher.SetColorSchemeColors(Resource.Color.accent);

            return view;
        }

        public override void OnStart()
        {
            base.OnStart();
            refresher.Refresh += Refresher_Refresh;
        }

        public override void OnStop()
        {
            base.OnStop();
            refresher.Refresh -= Refresher_Refresh;
        }

        void Refresher_Refresh(object sender, EventArgs e)
        {
            ViewModel.LoadItemsCommand.Execute(null);
        }

        void IsRefreshing_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Activity.RunOnUiThread(() => {
                refresher.Refreshing = ViewModel.IsRefreshing.Value;
            });
        }

    }

    class VplanAdapter : RecyclerView.Adapter
    {
        Activity Activity;
        VplanViewModel ViewModel;

        public VplanAdapter(Activity activity, VplanViewModel viewModel)
        {
            Activity = activity;
            ViewModel = viewModel;

            viewModel.Dates.PropertyChanged += (sender, args) =>
            {
                Activity.RunOnUiThread(NotifyDataSetChanged);
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_vplan, parent, false);
            return new VplanViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = ViewModel.Dates.Value[0].Items[position];

            var vplanHolder = holder as VplanViewHolder;
            vplanHolder.SchoolClassTextView.Text = item.ClassName;
            vplanHolder.HoursTextView.Text = item.Hours;
            vplanHolder.OldLessonTextView.Text = item.OldLesson;
            vplanHolder.ChangeTextView.Text = item.Type;
            vplanHolder.NewLessonTextView.Text = item.Description;
        }

        public override int ItemCount { 
            get => ViewModel.Dates.Value.Any() ? ViewModel.Dates.Value[0].Items.Count : 0;
        }
    }

    public class VplanViewHolder : RecyclerView.ViewHolder
    {
        public TextView SchoolClassTextView { get; private set; }
        public TextView HoursTextView { get; private set; }
        public TextView OldLessonTextView { get; private set; }
        public TextView ChangeTextView { get; private set; }
        public TextView NewLessonTextView { get; private set; }

        public VplanViewHolder(View itemView) : base(itemView)
        {
            SchoolClassTextView = itemView.FindViewById<TextView>(Resource.Id.schoolClassText);
            HoursTextView = itemView.FindViewById<TextView>(Resource.Id.hoursText);
            OldLessonTextView = itemView.FindViewById<TextView>(Resource.Id.oldLessonText);
            ChangeTextView = itemView.FindViewById<TextView>(Resource.Id.changeText);
            NewLessonTextView = itemView.FindViewById<TextView>(Resource.Id.newLessonText);
        }
    }
}
