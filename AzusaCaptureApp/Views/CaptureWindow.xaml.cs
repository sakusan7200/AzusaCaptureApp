using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    public CaptureWindow()
    {
        Instance = this;
        InitializeComponent();
        this.vm = App.VM;
        mainContent.DataContext = vm;

        //nvidiaのオーバーレイを埋もれさせるためのreverse
        var list = WindowEnumerator
            .EnumerateWindows()
            .AsEnumerable()
            .Reverse()
            .Where(WindowEnumerator.IsDecendWindow);

        //諸々の要件を満たすものだけを簡易Rectにする
        var resultrects = new List<AzusaRect>();

        foreach (var item in list)
        {
            var arect = item.GetAzusaRect();

            var flag = false;

            foreach (var a in resultrects)
            {
                if(Calc.isContainedAnotherRect(arect, a))
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

        var i = 0;
        foreach (var a in resultrects)
        {
            var r = new Rectangle();
            //r.Fill = new SolidColorBrush(Color.FromArgb(10, 0, 0, 255));
            r.Stroke = new SolidColorBrush(Color.FromArgb(250, 0xe6, 0xe6, 0xfa));
            r.StrokeThickness = 5;

            windowCnv.Children.Add(r);
            Canvas.SetZIndex(r, 1000 + i);
            Canvas.SetLeft(r, a.left);
            Canvas.SetTop(r, a.top);
            r.Width = a.width;
            r.Height = a.height;

            r.PointerEntered += (sender, e) =>
            {
                var list2 = WindowEnumerator.EnumerateWindows();
                r.Fill = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255));
                //r.SetValue(r.Fill, new SolidColorBrush(Color.FromArgb(200, 255, 255, 255));
                r.UpdateLayout();

                Debug.WriteLine(r.Name);
            };

            r.PointerExited += (sender, e) =>
            {
                r.Fill = null;
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


}
