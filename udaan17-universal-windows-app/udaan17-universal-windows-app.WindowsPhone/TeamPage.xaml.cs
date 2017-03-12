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
    public sealed partial class TeamPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public TeamPage()
        {
            this.InitializeComponent();

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
            //var devs = await DataSource.GetDevsAsync();
            //this.DefaultViewModel["Devs"] = devs;
            var team = await DataSource.GetTeamAsync();
            this.defaultViewModel["Team"] = team;
        }
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }
        private void PlaceCall(string contact, string name)
        {
            Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(contact, name);
        }
        #region NavigationHelper registration
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
