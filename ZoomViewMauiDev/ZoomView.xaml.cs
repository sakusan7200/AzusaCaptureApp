using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Diagnostics;
using Windows.UI.Core;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ZoomViewMauiDev;

public sealed partial class ZoomView : UserControl
{
    double imgWidth;
    double imgHeight;

    //[ObservableProperty] private ImageSource imgSource;

    private ImageSource _imageSource;
    public ImageSource ImgSource
    {
        get
        {
            return _imageSource;
        }

        set
        {
            _imageSource = value;
            fullSizeImg.Source = value;
        }
    }


    public ZoomView()
    {
        InitializeComponent();

        //scrollViewer.HorizontalOffset

        Canvas.SetLeft(selectionAreaRectangle, 0);
        Canvas.SetTop(selectionAreaRectangle, 0);

        fullSizeImg.SizeChanged += ImgSizeChanged;

        TopLeftThumb.DragDelta += (sender, args) =>
        {
            //Debug.WriteLine("TopLeft");
            var newLeft = Canvas.GetLeft(selectionAreaRectangle) + args.HorizontalChange;
            var newTop = Canvas.GetTop(selectionAreaRectangle) + args.VerticalChange;
            var newWidth = selectionAreaRectangle.Width - args.HorizontalChange;
            var newHeight = selectionAreaRectangle.Height - args.VerticalChange;

            UpdatePos(newTop, newLeft, newWidth, newHeight);
        };

        TopRightThumb.DragDelta += (sender, args) =>
        {
            //Debug.WriteLine("TopRight");
            //getrightなんてないので、getleftから頑張る

            var newLeft = Canvas.GetLeft(selectionAreaRectangle);
            var newTop = Canvas.GetTop(selectionAreaRectangle) + args.VerticalChange;
            var newWidth = selectionAreaRectangle.Width + args.HorizontalChange;
            var newHeight = selectionAreaRectangle.Height - args.VerticalChange;

            UpdatePos(newTop, newLeft, newWidth, newHeight);
        };

        BottomLeftThumb.DragDelta += (sender, args) =>
        {
            //Debug.WriteLine("BottomLeft");
            //getrightなんてないので、getleftから頑張る
            var newLeft = Canvas.GetLeft(selectionAreaRectangle) + args.HorizontalChange;
            var newTop = Canvas.GetTop(selectionAreaRectangle);
            var newWidth = selectionAreaRectangle.Width - args.HorizontalChange;
            var newHeight = selectionAreaRectangle.Height + args.VerticalChange;

            UpdatePos(newTop, newLeft, newWidth, newHeight);
        };

        BottomRightThumb.DragDelta += (sender, args) =>
        {
            //Debug.WriteLine("BottomLeft");
            //getrightなんてないので、getleftから頑張る
            var newLeft = Canvas.GetLeft(selectionAreaRectangle);
            var newTop = Canvas.GetTop(selectionAreaRectangle);
            var newWidth = selectionAreaRectangle.Width + args.HorizontalChange;
            var newHeight = selectionAreaRectangle.Height + args.VerticalChange;

            UpdatePos(newTop, newLeft, newWidth, newHeight);
        };

    }


    //プロパティ用============
    public static readonly DependencyProperty ImgProperty =
    DependencyProperty.Register(
        nameof(Img),
        typeof(ImageSource),
        typeof(ZoomView),
        new PropertyMetadata(null, OnImgChanged)
    );

    public ImageSource Img
    {
        get => (ImageSource)GetValue(ImgProperty);
        set => SetValue(ImgProperty, value);
    }

    private static void OnImgChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ZoomView)d;
        control.fullSizeImg.Source = (ImageSource)e.NewValue;
    }
    //============

    public double HorizontalOffset
    {
        get => scrollViewer.HorizontalOffset;
    }

    public double VerticalOffset
    {
        get => scrollViewer.VerticalOffset;
    }

    public float ZoomFactor
    {
        get => scrollViewer.ZoomFactor;
    }


    public Rectangle GetRect()
    {
        return selectionAreaRectangle;
    }
    public Rectangle GetSelectionArea()
    {
        return selectionAreaRectangle;
    }

    public void ResetZoom()
    {

        scrollViewer.ChangeView(0, 0, 0);
    }

    private void UpdatePos(double t, double l, double w, double h)
    {
        UpdatePos((int)Math.Round(t), (int)Math.Round(l), (int)Math.Round(w), (int)Math.Round(h));
    }

    private void UpdatePos(int t, int l, int w, int h)
    {
        if (l < 0 || t < 0 || w <= 0 || h <= 0 || w > imgWidth || h > imgHeight) return;

        //勢いよくやると微量超えてしまう
        //横
        if (l + w <= imgWidth)
        {
            selectionAreaRectangle.Width = w;
            Canvas.SetLeft(selectionAreaRectangle, l);

            Canvas.SetLeft(TopLeftThumb, l);
            Canvas.SetLeft(TopRightThumb, l + w - 10);
            Canvas.SetLeft(BottomLeftThumb, l);
            Canvas.SetLeft(BottomRightThumb, l + w - 10);
        }

        if(t + h <= imgHeight)
        {
            selectionAreaRectangle.Height = h;
            Canvas.SetTop(selectionAreaRectangle, t);

            Canvas.SetTop(TopLeftThumb, t);
            Canvas.SetTop(TopRightThumb, t);
            Canvas.SetTop(BottomLeftThumb, t + h - 10);
            Canvas.SetTop(BottomRightThumb, t + h - 10);
        }



        //外側のオーバーレイ
        LeftOverlay.Width = l;
        LeftOverlay.Height = imgHeight;

        Canvas.SetLeft(RightOverlay, l + w);
        if (imgWidth - l - w >= 0)
        {
            RightOverlay.Width = imgWidth - l - w;
        }
        RightOverlay.Height = imgHeight;

        Canvas.SetLeft(TopOverlay, l);
        TopOverlay.Width = w;
        TopOverlay.Height = t;

        Canvas.SetLeft(BottomOverlay, l);
        Canvas.SetTop(BottomOverlay, t + h);
        BottomOverlay.Width = w;
        if (imgHeight - h - t >= 0)
        {
            BottomOverlay.Height = imgHeight - h - t;
        }

    }

    private void ImgSizeChanged(object sender, SizeChangedEventArgs args)
    {
        Canvas.SetLeft(selectionAreaRectangle, 0);
        Canvas.SetTop(selectionAreaRectangle, 0);


        imgWidth = args.NewSize.Width;
        imgHeight = args.NewSize.Height;


        cnv.Width = args.NewSize.Width;
        cnv.Height = args.NewSize.Height;

        imgContent.Width = args.NewSize.Width + 100;
        imgContent.Height = args.NewSize.Height + 100;


        selectionAreaRectangle.Width = args.NewSize.Width;
        selectionAreaRectangle.Height = args.NewSize.Height;



        Canvas.SetLeft(TopRightThumb, args.NewSize.Width - 10);
        Canvas.SetTop(BottomLeftThumb, args.NewSize.Height - 10);
        Canvas.SetLeft(BottomRightThumb, args.NewSize.Width - 10);
        Canvas.SetTop(BottomRightThumb, args.NewSize.Height - 10);

        Canvas.SetLeft(LeftOverlay, 0);

        UpdatePos(0, 0, imgWidth, imgHeight);
    }

    private void ScrollViewer_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var props = e.GetCurrentPoint(scrollViewer).Properties;

        // Shiftキーが押されていない場合は通常のスクロールに任せる
        if (!props.IsHorizontalMouseWheel)
        {
            var shiftState = InputKeyboardSource
                .GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
            bool isShiftDown = shiftState.HasFlag(CoreVirtualKeyStates.Down);

            if (isShiftDown)
            {
                // デルタ値を取得（通常は±120単位）
                float delta = props.MouseWheelDelta;

                // 横スクロール量を計算（符号を反転して自然な方向に）
                double newOffset = scrollViewer.HorizontalOffset - delta;
                newOffset = Math.Clamp(newOffset, 0, scrollViewer.ScrollableWidth);

                scrollViewer.ChangeView(newOffset, null, null, disableAnimation: false);

                // 縦スクロールに渡さないようにマーク
                e.Handled = true;
            }
        }
    }
}
