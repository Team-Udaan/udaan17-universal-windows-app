using udaan17_universal_windows_app.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using udaan17_universal_windows_app.Data;

namespace udaan17_universal_windows_app
{
    public sealed partial class Architects : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public Architects()
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
            var item = await DataSource.GetDevsAsync();
            this.DefaultViewModel["Item"] = item;
        }
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        
        private void PlaceCall(string contact, string name)
        {
            Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(contact, name);
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
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string name;
            string Contact;
            StackPanel s;
            try
            {
                s = (e.OriginalSource as TextBlock).Parent as StackPanel;
                name = (s.Children[0] as TextBlock).Text;
                Contact = (s.Children[2] as TextBlock).Text;
                PlaceCall(Contact, name);
            }
            catch (Exception) { }
        }
    }
}
