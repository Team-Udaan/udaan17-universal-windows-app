using udaan17_universal_windows_app.Common;
using udaan17_universal_windows_app.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.System;

namespace udaan17_universal_windows_app
{
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public HubPage()
        {
            this.InitializeComponent();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }
        
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //var comingup = await DataSource.GetUpcomingAsync();
            //this.DefaultViewModel["ComingUp"] = comingup;
            var tevents = await DataSource.GetTeventsAsync();
            this.DefaultViewModel["TechEvents"] = tevents;
            var ntevents = await DataSource.GetDepartmentAsync("nonTech");
            this.DefaultViewModel["NonTechEvents"] = ntevents;
            var cultural = await DataSource.GetDepartmentAsync("cultural");
            this.DefaultViewModel["Cultural"] = cultural;
        }
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }
        
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((Department)e.ClickedItem).Alias;
            if (!Frame.Navigate(typeof(SectionPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }
        private void GroupSection_EventItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((Event)e.ClickedItem).name;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void pinAppBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(TeamUdaan)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private async void review_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?PFN={Package.Current.Id.FamilyName}"));
        }

        private void teamUdaan_Click(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(TeamPage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }
    }
}