using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using udaan17_universal_windows_app.Data;
using udaan17_universal_windows_app.Common;


namespace udaan17_universal_windows_app
{
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
        }
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var comingup = await DataSource.GetUpcomingAsync();
            this.DefaultViewModel["ComingUp"] = comingup;
            var tevents = await DataSource.GetTeventsAsync();
            this.DefaultViewModel["TechEvents"] = tevents;
            var ntevents = await DataSource.GetDepartmentAsync("non-tech");
            this.DefaultViewModel["NonTechEvents"] = ntevents;
            var cultural = await DataSource.GetDepartmentAsync("cultural");
            this.DefaultViewModel["Cultural"] = cultural;
            var gs = await DataSource.GetDepartmentAsync("girls-special");
            this.DefaultViewModel["GirlsSpecial"] = gs;
        }
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            this.Frame.Navigate(typeof(SectionPage), ((SampleDataGroup)group).UniqueId);
        }
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((Department)e.ClickedItem).Alias;
            this.Frame.Navigate(typeof(SectionPage), itemId);
        }
        void ItemView_EventItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((Event)e.ClickedItem).name;
            this.Frame.Navigate(typeof(ItemPage), itemId);
        }
        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DevsPage));
        }
    }
}
