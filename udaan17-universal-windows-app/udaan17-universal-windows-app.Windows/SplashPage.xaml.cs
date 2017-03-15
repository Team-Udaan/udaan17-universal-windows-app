using udaan17_universal_windows_app.Common;
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
using System.Threading.Tasks;
using Windows.Storage;
using System.Net;
using udaan17_universal_windows_app.Data;

namespace udaan17_universal_windows_app
{
    public sealed partial class SplashPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private Task dataTask;
        
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }
        
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public SplashPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }
        
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            dataTask = CheckData();
            imageAnimation.Begin();
            imageAnimation.Completed += ImageAnimation_Completed;
        }

        private async void ImageAnimation_Completed(object sender, object e)
        {
            await dataTask;
            Frame.Navigate(typeof(HubPage));
            Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
        }

       private async Task CheckData()
        {
            await DataSource.CheckDataAsync("data.json");
            await DataSource.CheckDataAsync("developers.json");
            //await DataSource.CheckDataAsync("teamUdaan.json");
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
