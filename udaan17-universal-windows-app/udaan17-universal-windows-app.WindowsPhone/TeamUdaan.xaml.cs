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
using Windows.ApplicationModel.Email;

namespace udaan17_universal_windows_app
{
    public sealed partial class TeamUdaan : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public TeamUdaan()
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
            var devs = await DataSource.GetDevsAsync();
            this.DefaultViewModel["Devs"] = devs;
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

        private void Phone_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string Contact;
            StackPanel s;
            try
            {
                s = ((e.OriginalSource as Image).Parent as Border).Parent as StackPanel;
                Contact = (s.Children[1] as TextBlock).Text;
                var d = (DefaultViewModel["Item"] as List<Devs>).Select(dev => dev).Where(item => Contact.Equals(item.Contact));
                PlaceCall(Contact, d.First().Name);
            }
            catch (Exception) { }
        }
        private async void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string Contact;
            StackPanel s;
            try
            {
                s = ((e.OriginalSource as Image).Parent as Border).Parent as StackPanel;
                Contact = (s.Children[1] as TextBlock).Text;
                var d = (DefaultViewModel["Item"] as List<Devs>).Select(dev => dev).Where(item => Contact.Equals(item.Contact));
                EmailRecipient sendTo = new EmailRecipient()
                {
                    Address = d.First().Email
                };
                EmailMessage mail = new EmailMessage();
                mail.To.Add(sendTo);
                await EmailManager.ShowComposeNewEmailAsync(mail);
            }
            catch (Exception) { }
        }
    }
}
