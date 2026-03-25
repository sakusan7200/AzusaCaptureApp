using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AzusaCaptureApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public Frame RootFrame { get; private set; }

        public MainWindow()
        {
            Instance = this;

            InitializeComponent();
            var rootFrame = new Frame();
            rootFrame.Navigate(typeof(MainPage));
            rootFrame.DataContext = App.VM;
            Content = rootFrame;
            RootFrame = rootFrame;

            //InitializeComponent();
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            if (AppWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                //AppWindow.TitleBar.HeightOption = TitleBarHeightOption.Standard;
                AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;
            }


            Closed += (sender, args) =>
            {
                args.Handled = true;
                this.AppWindow.Hide();
            };
        }
    }
}
