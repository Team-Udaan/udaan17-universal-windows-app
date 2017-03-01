using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using udaan17_universal_windows_app.Common;
using System.Threading.Tasks;
using Windows.Storage;
using System.Net;

namespace udaan17_universal_windows_app
{
    public sealed partial class App : Application
    {
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                        
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                    }
                }
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif
                
                if (!rootFrame.Navigate(typeof(HubPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            //await CheckDataAsync();
            Window.Current.Activate();
        }

        private async Task CheckDataAsync()
        {
            Uri dataUri = new Uri("ms-appx:///DataModel/Data.json");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            var props = await file.GetBasicPropertiesAsync();
            DateTime local = props.DateModified.DateTime;
            HttpWebRequest req = WebRequest.CreateHttp("https://raw.githubusercontent.com/Team-Udaan/udaan16-windows-phone-app/master/Udaan16/Udaan16/DataModel/Data.json");
            WebResponse res = (HttpWebResponse)await req.GetResponseAsync();
            DateTime remote = DateTime.Parse(res.Headers[HttpRequestHeader.LastModified]);
            if (remote.CompareTo(local) != 0)
            {
                using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                using (StreamWriter sw = new StreamWriter(await file.OpenStreamForWriteAsync()))
                {
                    sw.WriteLine(sr.ReadToEnd());
                }
            }
        }

#if WINDOWS_PHONE_APP
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}