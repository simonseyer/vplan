using System;

using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class NewSchoolClassesViewController : UIViewController
    {
        NewSchoolClassesViewModel ViewModel;
        ChipCollectionViewDataSource SchoolClassesDataSource { get; set; }
        ChipCollectionViewDelegate SchoolClassesDelegate { get; set; }

        public NewSchoolClassesViewController() : base("NewSchoolClassesViewController", null)
        {
        }

        public NewSchoolClassesViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new NewSchoolClassesViewModel();

            SchoolClassesDataSource = new ChipCollectionViewDataSource(CollectionView, ViewModel.SchoolClasses);
            CollectionView.DataSource = SchoolClassesDataSource;
            SchoolClassesDelegate = new ChipCollectionViewDelegate(ViewModel.SchoolClasses, ViewModel.ToggleSchoolClassBookmarkAtIndex);
            CollectionView.Delegate = SchoolClassesDelegate;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SchoolClassesDelegate.ActivateFeedback(true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            SchoolClassesDelegate.ActivateFeedback(false);
        }

        partial void DoneTapped(Foundation.NSObject sender)
        {
            ViewModel.Finish();
            DismissViewController(true, null);
        }

        partial void CloseTapped(Foundation.NSObject sender)
        {
            DismissViewController(true, null);
        }
    }
}