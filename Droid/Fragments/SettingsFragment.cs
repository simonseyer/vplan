
using System;
using System.Linq;
using System.Diagnostics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Com.Xiaofeng.Flowlayoutmanager;
using Android.Support.V7.Widget;
using static Android.Views.View;
using Android.App;
using System.Collections.Generic;
using Android.Graphics.Drawables.Shapes;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace FLSVertretungsplan.Droid
{
    public class SettingsFragment : Android.Support.V4.App.Fragment
    {

        RecyclerView SchoolGridView;
        RecyclerView SchoolClassGridView;

        ChipAdapter SchoolChipAdapter;
        ChipAdapter SchoolClassChipAdapter;

        RecyclerView.LayoutManager SchoolLayoutManager;
        RecyclerView.LayoutManager SchoolClassLayoutManager;

        SettingsViewModel ViewModel;

        public static SettingsFragment NewInstance() =>
            new SettingsFragment { Arguments = new Bundle() };

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ViewModel = new SettingsViewModel();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_settings, container, false);

            SchoolGridView = view.FindViewById<RecyclerView>(Resource.Id.schoolGridView);
            SchoolLayoutManager = new FlowLayoutManager();
            SchoolGridView.SetLayoutManager(SchoolLayoutManager);
            SchoolChipAdapter = new ChipAdapter(Activity, SchoolGridView, ViewModel.Schools);
            SchoolChipAdapter.ItemClick += SchoolClicked;

            SchoolClassGridView = view.FindViewById<RecyclerView>(Resource.Id.schoolClassGridView);
            SchoolClassLayoutManager = new FlowLayoutManager();
            SchoolClassGridView.SetLayoutManager(SchoolClassLayoutManager);
            SchoolClassChipAdapter = new ChipAdapter(Activity, SchoolClassGridView, ViewModel.SchoolClasses);
            SchoolClassChipAdapter.ItemClick += SchoolClassClicked;

            return view;
        }

        void SchoolClicked(object sender, ItemClickEventArgs e) 
        {
            ViewModel.ToggleSchoolBookmarkAtIndex(e.Position);
        }

        void SchoolClassClicked(object sender, ItemClickEventArgs e)
        {
            ViewModel.ToggleSchoolClassBookmarkAtIndex(e.Position);
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void OnStop()
        {
            base.OnStop();           
        }
    }

    class ChipViewHolder : RecyclerView.ViewHolder {
        
        public readonly View View;
        public readonly TextView TextView;
        public readonly GradientDrawable Background;

        public ChipViewHolder(View view) : base(view)
        {
            View = view;
            TextView = view.FindViewById<TextView>(Resource.Id.textView1);
            Background = (GradientDrawable)TextView.Background;
        }
    }

    public class ItemClickEventArgs : EventArgs
    {
        public int Position { get; private set; }
        public ItemClickEventArgs(int position)
        {
            Position = position;
        }
    }

    public delegate void ItemClickEventHandler(object sender,
                                               ItemClickEventArgs e);

    class ChipAdapter : RecyclerView.Adapter, IOnClickListener
    {
        readonly Activity Activity;
        readonly RecyclerView RecyclerView;
        ObservableCollection<ChipPresentationModel> ObservableItems;
        List<ChipPresentationModel> Items;

        public event ItemClickEventHandler ItemClick;

        public ChipAdapter(Activity activity, RecyclerView recyclerView, ObservableCollection<ChipPresentationModel> items)
        {
            Activity = activity;
            RecyclerView = recyclerView;
            ObservableItems = items;
            Items = items.ToList();

            RecyclerView.SetAdapter(this);
            ObservableItems.CollectionChanged += CollectionChanged;
        }

        public override int ItemCount { 
            get => Items.Count();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var chipViewHolder = holder as ChipViewHolder;
            var model = Items[position];
            chipViewHolder.TextView.Text = model?.Name;
            chipViewHolder.Background.SetColor(new Color(model.OutlineColor.Red, model.OutlineColor.Green, model.OutlineColor.Blue));
        }

        public void OnClick(View view)
        {
            int itemPosition = RecyclerView.GetChildLayoutPosition(view);
            ItemClick?.Invoke(this, new ItemClickEventArgs(itemPosition));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_chip, null);

            view.SetOnClickListener(this);

            return new ChipViewHolder(view);
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var items = ObservableItems.ToList();
            Activity.RunOnUiThread(()=> 
            {
                Items = items;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        NotifyItemRangeInserted(e.NewStartingIndex, e.NewItems.Count);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        NotifyItemRangeRemoved(e.OldStartingIndex, e.OldItems.Count);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        NotifyItemRangeChanged(e.NewStartingIndex, e.NewItems.Count);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        throw new NotSupportedException("Moving items is not supported");
                    case NotifyCollectionChangedAction.Reset:
                        NotifyDataSetChanged();
                        break;
                }
            });
        }
    }
}
