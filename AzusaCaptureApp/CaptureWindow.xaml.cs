using Microsoft.UI;
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Color = Windows.UI.Color;
using Rectangle = Microsoft.UI.Xaml.Shapes.Rectangle;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AzusaCaptureApp;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CaptureWindow : Window
{
    MainViewModel vm;

    public CaptureWindow()
    {
        InitializeComponent();
        this.vm = App.VM;
        mainContent.DataContext = vm;
        //AppWindow.Hide();
    }

    public void AddRectToCanvas(Rectangle rect)
    {
        cnv.Children.Add(rect);
    }


    private async void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        File.WriteAllText("C:\\Users\\sakua\\Desktop\\.log", "a\n");
        File.AppendAllText("C:\\Users\\sakua\\Desktop\\.log", $"{DateTime.Now.ToString()} Window_Activated\n");
        vm.ActivationProcess();
    }

    private void cnv_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        vm.CursorMoved(e.GetCurrentPoint(cnv).Position);
    }

    private void cnv_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        vm.CursorPressed(e.GetCurrentPoint(cnv).Position);
    }

    private void cnv_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        vm.CursorReleased(e.GetCurrentPoint(cnv).Position);

    }

    private void CommandBar_Loaded(object sender, RoutedEventArgs e)
    {
        (sender as CommandBar).OverflowButtonVisibility = CommandBarOverflowButtonVisibility.Collapsed;
    }

    private void CommandBar_Closing(object sender, object e)
    {
        (sender as CommandBar).IsOpen = true;
    }

}
