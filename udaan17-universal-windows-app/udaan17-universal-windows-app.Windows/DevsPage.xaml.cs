using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using udaan17_universal_windows_app.Common;
using udaan17_universal_windows_app.Data;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace udaan17_universal_windows_app
{
    public sealed partial class DevsPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public DevsPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //object navigationParameter;
            //if (e.PageState != null && e.PageState.ContainsKey("SelectedItem"))
            //{
            //    navigationParameter = e.PageState["SelectedItem"];
            //}

            var item = await DataSource.GetDevsAsync();
            this.DefaultViewModel["Item"] = item;
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
