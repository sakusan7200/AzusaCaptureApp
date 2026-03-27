using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AzusaCaptureApp;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
    public static MainPage Singleton { get; private set; }

    double imgWidth;
    double imgHeight;

    


    public MainPage()
    {
        Singleton = this;
        InitializeComponent();

        taskbaricon.DataContext = App.VM;

    }

    public void StartTriming()
    {
        //cnv.Visibility = Visibility.Visible;
        cmdBar.Visibility = Visibility.Collapsed;
        trimCmdBar.Visibility = Visibility.Visible;
        //fullSizeImg.Visibility = Visibility.Visible;
        //img.Visibility = Visibility.Collapsed;
        zoomView.Visibility = Visibility.Visible;
    }

    public Microsoft.UI.Xaml.Shapes.Rectangle DoneTriming()
    {
        //cnv.Visibility = Visibility.Collapsed;
        cmdBar.Visibility = Visibility.Visible;
        trimCmdBar.Visibility = Visibility.Collapsed;
        //fullSizeImg.Visibility = Visibility.Collapsed;
        //img.Visibility = Visibility.Visible;
        zoomView.Visibility = Visibility.Collapsed;

        scrollViewer.ChangeView(0, 0, 0);


        return zoomView.GetRect();
    }

    private void AppBarButton_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        //App.VM.ShowOpeNAnotherAppDialogComand.Execute(null);
    }
}
