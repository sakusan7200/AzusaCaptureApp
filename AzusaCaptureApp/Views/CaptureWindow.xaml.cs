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
using System.Diagnostics;
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
    public static CaptureWindow Instance { get;private set; }
    MainViewModel vm;

    //aはbに含まれているか
    private bool isContainedAnotherRect(AzusaRect a, AzusaRect b)
    {
        //var a = _a.ConvertToInternalRect();
        //var b = _b.ConvertToInternalRect();
        return a.left >= b.left&& a.left + a.width <= b.left + b.width && a.top >= b.top && a.top + a.height <= b.top + b.height;
    }

    public CaptureWindow()
    {
        Instance = this;
        InitializeComponent();
        this.vm = App.VM;
        mainContent.DataContext = vm;

        var i = 0;
        var list = WindowEnumerator.EnumerateWindows();
        //下のほうに別の仮想デスクトップとか、nvidiaのオーバーレイを埋もれさせるためのreverse
        list.Reverse();

        var resultrects = new List<AzusaRect>();

        foreach (var item in list)
        {
            if(!WindowEnumerator.IsUserWindow(item.Handle)) continue;

            var arect = item.GetAzusaRect();

            var flag = false;

            foreach (var a in resultrects)
            {
                if(isContainedAnotherRect(arect, a))
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                resultrects.Add(arect);
            }
        }

        foreach(var a in resultrects)
        {
            var r = new Rectangle();
            r.Fill = new SolidColorBrush(Color.FromArgb(10, 0, 0, 255));
            r.Stroke = new SolidColorBrush(Color.FromArgb(250, 255, 0, 0));
            r.StrokeThickness = 5;


            windowCnv.Children.Add(r);

            Canvas.SetZIndex(r, 1000 + i);
            Canvas.SetLeft(r, a.left);
            Canvas.SetTop(r, a.top);
            r.Width = a.width;
            r.Height = a.height;

            //r.Name = item.Title;

            r.PointerEntered += (sender, e) =>
            {
                var list2 = WindowEnumerator.EnumerateWindows();
                r.Fill = new SolidColorBrush(Color.FromArgb(250, 0, 100, 255));

                Debug.WriteLine(r.Name);
            };

            r.PointerExited += (sender, e) =>
            {
                r.Fill = new SolidColorBrush(Color.FromArgb(10, 0, 0, 255));
            };

            r.PointerPressed += (sender, e) =>
            {
                //クロップ
                vm.ShotByRect(((Rectangle)sender).ConvertToInternalRect());
            };

            i++;
        }
    }

    public void AddRectToCanvas(Rectangle rect)
    {
        cnv.Children.Add(rect);
    }

    public void SwitchWindowRects(bool isenabled)
    {
        if (isenabled)
        {
            windowCnv.Visibility = Visibility.Visible;
        }
        else
        {
            windowCnv.Visibility = Visibility.Collapsed;
        }
    }

    private async void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        //File.WriteAllText("C:\\Users\\sakua\\Desktop\\.log", "a\n");
        //File.AppendAllText("C:\\Users\\sakua\\Desktop\\.log", $"{DateTime.Now.ToString()} Window_Activated\n");
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
