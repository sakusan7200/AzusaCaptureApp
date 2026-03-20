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

        //Canvas.SetLeft(selectionAreaRectangle, 0);
        //Canvas.SetTop(selectionAreaRectangle, 0);

        //fullSizeImg.SizeChanged += ImgSizeChanged;

        //TopLeftThumb.DragDelta += (sender,args)=>
        //{
        //    Debug.WriteLine("TopLeft");
        //    var newLeft = Canvas.GetLeft(selectionAreaRectangle) + args.HorizontalChange;
        //    var newTop = Canvas.GetTop(selectionAreaRectangle) + args.VerticalChange;
        //    var newWidth = selectionAreaRectangle.Width - args.HorizontalChange;
        //    var newHeight = selectionAreaRectangle.Height - args.VerticalChange;
        //    if (newLeft < 0 || newTop < 0 || newWidth <= 0 || newHeight <= 0 || newWidth > imgWidth || newHeight > imgHeight) return;


        //    if (newWidth > 0 && newHeight > 0)
        //    {
        //        Canvas.SetLeft(selectionAreaRectangle, newLeft);
        //        Canvas.SetTop(selectionAreaRectangle, newTop);

        //        UpdatePos(newTop, newLeft, newWidth, newHeight);
        //    }
        //};

        //TopRightThumb.DragDelta += (sender, args) =>
        //{
        //    Debug.WriteLine("TopRight");
        //    //getrightなんてないので、getleftから頑張る

        //    var newLeft = Canvas.GetLeft(selectionAreaRectangle);
        //    var newTop = Canvas.GetTop(selectionAreaRectangle) + args.VerticalChange;
        //    var newWidth = selectionAreaRectangle.Width + args.HorizontalChange;
        //    var newHeight = selectionAreaRectangle.Height - args.VerticalChange;

        //    if (newLeft < 0 || newTop < 0 || newWidth <= 0 || newHeight <= 0 || newWidth > imgWidth || newHeight > imgHeight) return;

        //    //Canvas.SetLeft(selectionAreaRectangle, newLeft);
        //    Canvas.SetTop(selectionAreaRectangle, newTop);

        //    if (newWidth > 0 && newHeight > 0)
        //    {
        //        UpdatePos(newTop, newLeft, newWidth, newHeight);
        //    }

        //};

        //BottomLeftThumb.DragDelta += (sender, args) =>
        //{
        //    Debug.WriteLine("BottomLeft");
        //    //getrightなんてないので、getleftから頑張る
        //    var newLeft = Canvas.GetLeft(selectionAreaRectangle) + args.HorizontalChange;
        //    var newTop = Canvas.GetTop(selectionAreaRectangle);
        //    var newWidth = selectionAreaRectangle.Width - args.HorizontalChange;//
        //    var newHeight = selectionAreaRectangle.Height + args.VerticalChange;

        //    if (newLeft < 0 || newTop < 0 || newWidth <= 0 || newHeight <= 0 || newWidth > imgWidth || newHeight > imgHeight) return;



        //    Canvas.SetLeft(selectionAreaRectangle, newLeft);
        //    //Canvas.SetTop(selectionAreaRectangle, newTop);

        //    if (newWidth > 0 && newHeight > 0)
        //    {
        //        UpdatePos(newTop, newLeft, newWidth, newHeight);
        //    }

        //};

        //BottomRightThumb.DragDelta += (sender, args) =>
        //{
        //    Debug.WriteLine("BottomLeft");
        //    //getrightなんてないので、getleftから頑張る
        //    var newLeft = Canvas.GetLeft(selectionAreaRectangle);
        //    var newTop = Canvas.GetTop(selectionAreaRectangle);
        //    var newWidth = selectionAreaRectangle.Width + args.HorizontalChange;//
        //    var newHeight = selectionAreaRectangle.Height + args.VerticalChange;
        //    if (newLeft < 0 || newTop < 0 || newWidth <= 0 || newHeight <= 0 || newWidth > imgWidth || newHeight > imgHeight) return;


        //    //Canvas.SetLeft(selectionAreaRectangle, newLeft);
        //    //Canvas.SetTop(selectionAreaRectangle, newTop);

        //    if (newWidth > 0 && newHeight > 0)
        //    {
        //        UpdatePos(newTop, newLeft, newWidth, newHeight);
        //    }

        //};

        //zoomView.ImgSource = new BitmapImage(new Uri("C:\\Users\\sakua\\Projects\\ZoomViewMauiDev\\App1\\img1.jpg"));
    }

    //private void ImgSizeChanged(object sender, SizeChangedEventArgs args)
    //{
    //    imgWidth = args.NewSize.Width;
    //    imgHeight = args.NewSize.Height;


    //    cnv.Width = args.NewSize.Width;
    //    cnv.Height = args.NewSize.Height;

    //    imgContent.Width = args.NewSize.Width;
    //    imgContent.Height = args.NewSize.Height;


    //    selectionAreaRectangle.Width = args.NewSize.Width;
    //    selectionAreaRectangle.Height = args.NewSize.Height;



    //    Canvas.SetLeft(TopRightThumb, args.NewSize.Width - 10);
    //    Canvas.SetTop(BottomLeftThumb, args.NewSize.Height - 10);
    //    Canvas.SetLeft(BottomRightThumb, args.NewSize.Width - 10);
    //    Canvas.SetTop(BottomRightThumb, args.NewSize.Height - 10);

    //    Canvas.SetLeft(LeftOverlay, 0);

    //    //UpdatePos(0,0,imgWidth,imgHeight);
    //}

    //private void UpdatePos(double t, double l, double w, double h)
    //{
    //    selectionAreaRectangle.Width = w;
    //    selectionAreaRectangle.Height = h;

    //    Canvas.SetLeft(TopLeftThumb, l);
    //    Canvas.SetTop(TopLeftThumb, t);

    //    Canvas.SetLeft(TopRightThumb, l + w - 10);
    //    Canvas.SetTop(TopRightThumb, t);

    //    Canvas.SetLeft(BottomLeftThumb, l);
    //    Canvas.SetTop(BottomLeftThumb, t + h - 10);

    //    Canvas.SetLeft(BottomRightThumb, l + w - 10);
    //    Canvas.SetTop(BottomRightThumb, t + h - 10);


    //    //外側のオーバーレイ
    //    LeftOverlay.Width = l;
    //    LeftOverlay.Height = imgHeight;
        
    //    Canvas.SetLeft(RightOverlay, l + w);
    //    if(imgWidth - l - w > 0)
    //    {
    //        RightOverlay.Width = imgWidth - l - w;
    //    }
    //    RightOverlay.Height = imgHeight;

    //    Canvas.SetLeft(TopOverlay, l);
    //    TopOverlay.Width = w;
    //    TopOverlay.Height = t;

    //    Canvas.SetLeft(BottomOverlay, l);
    //    Canvas.SetTop(BottomOverlay, t+h);
    //    BottomOverlay.Width = w;
    //    if(imgHeight - h - t > 0)
    //    {
    //        BottomOverlay.Height = imgHeight - h - t;
    //    }
    //}

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

        scrollViewer.ChangeView(zoomView.HorizontalOffset, zoomView.VerticalOffset, scrollViewer.ZoomFactor);


        return zoomView.GetRect();
    }
}
