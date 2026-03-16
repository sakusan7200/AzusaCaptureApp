using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using NHotkey;
using NHotkey.WinUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AzusaCaptureApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static CaptureWindow CurrentCaptureWindow { get; private set; }
        public static MainWindow CurrentMainWindow { get; private set; }
        public static IServiceProvider Services { get; private set; }
        public static MainViewModel VM { get; private set; }


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            var sc = new ServiceCollection();
            sc.AddTransient<MainViewModel>();
            //sc.AddSingleton<CaptureWindow>();
            sc.AddSingleton<ICaptureWindowService, CaptureWindowService>();
            sc.AddSingleton<IMainWindowService, MainWindowService>();
            Services = sc.BuildServiceProvider();

            //キーボードフックの設定
            //TODO: ms-screenclipへの対応
            var a = new KeyboardAccelerator();
            a.Key = Windows.System.VirtualKey.Snapshot;
            a.Modifiers = Windows.System.VirtualKeyModifiers.Control;
            HotkeyManager.Current.AddOrReplace("OnPrintScreenPressed", a, OnPrintScreenPressed);
        }

        private void OnPrintScreenPressed(object sender, HotkeyEventArgs args)
        {
            VM.StartCaptureCommand.Execute(null);
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            VM = Services.GetRequiredService<MainViewModel>();
            CurrentMainWindow = new MainWindow();
            CurrentMainWindow.Activate();
        }

        public static void ShowCaptureWindow()
        {
            CurrentCaptureWindow = new CaptureWindow();
            CurrentCaptureWindow.Activate();
        }

        public static void ShowMainWindow()
        {
            CurrentMainWindow = new MainWindow();
            CurrentMainWindow.Activate();
        }

        public static bool TryGoBack()
        {
            var rootFrame = CurrentMainWindow.RootFrame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                return true;
            }
            return false;
        }
    }
}
