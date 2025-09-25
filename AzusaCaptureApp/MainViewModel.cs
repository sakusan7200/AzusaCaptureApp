using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Foundation;
//using System.Windows.Media.Imaging;
using Point = Windows.Foundation.Point;
using Rectangle = Microsoft.UI.Xaml.Shapes.Rectangle;

namespace AzusaCaptureApp;

public partial class MainViewModel : ObservableObject
{
    ICaptureWindowService cws;
    IMainWindowService mws;
    public MainViewModel(ICaptureWindowService cws, IMainWindowService mws)
    {


        this.cws = cws;
        this.mws = mws;
        PropertyChanged += MainViewModel_PropertyChanged;
        //CaptureMng.Init();

        //FreeHandChecked = true;

        Setting = new AppSetting();

       TestValue = "A";


        //設定読み出し
        if(File.Exists(Cont.SettingDir + "\\settings.json"))
        {
            var jsonstr = File.ReadAllText(Cont.SettingDir + "\\settings.json");
            Setting = JsonConvert.DeserializeObject<AppSetting>(jsonstr);

        }
        else
        {
            Setting = new AppSetting();
            Setting.SaveTo = Cont.DefaultSaveDir;
        }
    }

    private void MainViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        //トグルボタンの切り替え
        if(e.PropertyName.EndsWith("Checked"))
        {
            switch (e.PropertyName)
            {
                case nameof(FullScrChecked):
                    if (FullScrChecked == true)
                    {
                        AreaCapChecked = false;
                        FreeHandChecked = false;
                        WindowChecked = false;
                        FullScrGIFChecked = false;
                        WindowGIFChecked = false;
                    }
                    break;
                case nameof(AreaCapChecked):
                    if (AreaCapChecked == true)
                    {
                        FullScrChecked = false;
                        FreeHandChecked = false;
                        WindowChecked = false;
                        FullScrGIFChecked = false;
                        WindowGIFChecked = false;
                    }
                    break;
                case nameof(FreeHandChecked):
                    if (FreeHandChecked == true)
                    {
                        FullScrChecked = false;
                        AreaCapChecked = false;
                        WindowChecked = false;
                        FullScrGIFChecked = false;
                        WindowGIFChecked = false;
                    }
                    break;
                case nameof(WindowChecked):
                    if (WindowChecked == true)
                    {
                        FullScrChecked = false;
                        AreaCapChecked = false;
                        FreeHandChecked = false;
                        FullScrGIFChecked = false;
                        WindowGIFChecked = false;
                    }
                    break;
                case nameof(FullScrGIFChecked):
                    if (FullScrGIFChecked == true)
                    {
                        FullScrChecked = false;
                        AreaCapChecked = false;
                        FreeHandChecked = false;
                        WindowChecked = false;
                        WindowGIFChecked = false;
                    }
                    break;
                case nameof(WindowGIFChecked):
                    if (WindowGIFChecked == true)
                    {
                        FullScrChecked = false;
                        AreaCapChecked = false;
                        FreeHandChecked = false;
                        WindowChecked = false;
                        FullScrGIFChecked = false;
                    }
                    break;
            }
        }
    }

    [ObservableProperty] private BitmapImage currentImgSource;//スクショされた画像が変わることはないよね

    [ObservableProperty] private BitmapImage fullSizeImgSource;


    Point startPoint;
    Rectangle selectionRect;

    [ObservableProperty] private string testValue;

    [ObservableProperty] private bool fullScrChecked;
    [ObservableProperty] private bool areaCapChecked;
    [ObservableProperty] private bool freeHandChecked;
    [ObservableProperty] private bool windowChecked;
    [ObservableProperty] private bool fullScrGIFChecked;
    [ObservableProperty] private bool windowGIFChecked;

    [ObservableProperty] private AzusaCaptureApp.AppSetting setting;

    

    MemoryStream memStrm = new MemoryStream();

    private CaptureWay WhatWay()
    {
        if (fullScrChecked) return CaptureWay.FullScreen;
        if (areaCapChecked) return CaptureWay.AreaCapture;
        if (freeHandChecked) return CaptureWay.FreeHand;
        if (fullScrChecked) return CaptureWay.OneWindow;
        if (windowChecked) return CaptureWay.FullScreen;
        if (fullScrGIFChecked) return CaptureWay.FullScreenGIF;
        if (windowGIFChecked) return CaptureWay.OneWindow;
        return CaptureWay.Nodefinded;
    }

    //[RelayCommand]
    //暫定措置
    public void GetShot()
    {
        //ms = CaptureMng.CaptureFullScreen();
        CurrentImgSource = CaptureMng.CaptureFullScreen(memStrm);
        FullSizeImgSource = CaptureMng.ConvertFrom(memStrm);
    }

    public void ActivationProcess()
    {
        var rect = new Rectangle
        {
            Width = Cont.MyMonitor_W,
            Height = Cont.FullHD_H,
            StrokeThickness = 2,
            Fill = new SolidColorBrush(Cont.Gray) // 半透明
        };
        cws.AddRectToCanvas(rect);
        cws.GoFullScr();
    }


    [RelayCommand]
    private void StartTrim()
    {
        mws.StartTrim();
    }

    [RelayCommand]
    private void ExitApp()
    {
        Environment.Exit(0);
    }

    [RelayCommand]
    private void ReturnToMainWindow()
    {
        mws.MoveToMainWindow();
    }

    [RelayCommand]
    private async void StartCapture()
    {
        if (mws.IsActive())
        {
            mws.Minimaize();
            //TODO: おかしい
            await Task.Delay(600);
        }
        CaptureMng.Init(Setting.SaveTo, memStrm);

        //mws.Close();
        GetShot();

        CaptureMng.SaveTo(Setting.SaveTo + $"\\{System.DateTime.Now.ToString("YYYY_MM_DD_hh_mm_ss")}.png", memStrm);
        var btn = new AppNotificationButton("表示");
        btn.AddArgument("show", "current");
        var notification = new AppNotificationBuilder()
    .AddText(Cont.AppName)
    .AddText($"スクリーンショットを\n{System.DateTime.Now.ToString("YYYY_MM_DD_hh_mm_ss")}.png\nとして保存し、クリップボードにコピーしました。")
    .AddButton(btn)
    .BuildNotification();
        AppNotificationManager.Default.Show(notification);

        mws.MoveToMainWindow();
        
        //App.ShowCaptureWindow();
        //cws.GoFullScr();
    }


    [RelayCommand]
    private void FinishTrim()
    {
        var rect = mws.FinishTrim();
        CurrentImgSource = CaptureMng.Trim((int)Canvas.GetLeft(rect), (int)Canvas.GetTop(rect), (int)rect.Width, (int)rect.Height, memStrm);
        
    }

    [RelayCommand]
    private async void StartCapture2()
    {
        CaptureMng.Init(Setting.SaveTo, memStrm);
        mws.Minimaize();

        //TODO: おかしい
        await Task.Delay(600);
        //mws.Close();
        GetShot();
        App.ShowCaptureWindow();
    }

    [RelayCommand]
    private async void CaptureRightNow()
    {
        CaptureMng.Init(Setting.SaveTo, memStrm);
        mws.Minimaize();
        //mws.Close();
        GetShot();

        mws.MoveToMainWindow();
    }

    [RelayCommand]
    private async Task SaveTo()
    {
        // Create a file picker
        var savePicker = new FileSavePicker();

        // See the sample code below for how to make the window accessible from the App class.
        var window = App.CurrentMainWindow;

        // Retrieve the window handle (HWND) of the current WinUI 3 window.
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        // Initialize the file picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

        // Set options for your file picker
        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        // Dropdown of file types the user can save the file as
        savePicker.FileTypeChoices.Add("PNG画像", new List<string>() { ".png" });

        var file = await savePicker.PickSaveFileAsync();

        Debug.WriteLine(file.Name);


        CaptureMng.SaveTo(file.Path, memStrm);
    }

    [RelayCommand]
    private void SetClipboard()
    {
        CaptureMng.SetCipboard(memStrm);
    }

    [RelayCommand]
    private void OpenInAnotherApp()
    {
        Process.Start("C:\\Users\\sakua\\Desktop\\a.png");
    }

    [RelayCommand]
    private void CancelBtn()
    {
        cws.MoveToMainWindow();
        cws.Close();
    }

    [RelayCommand]
    private void OpenSetting()
    {
        mws.OpenSettingPage();
    }

    [RelayCommand]
    private async void ShowOFDForSetting()
    {
        // Create a folder picker
        FolderPicker openPicker = new Windows.Storage.Pickers.FolderPicker();

        // See the sample code below for how to make the window accessible from the App class.
        var window = App.CurrentMainWindow;

        // Retrieve the window handle (HWND) of the current WinUI 3 window.
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        // Initialize the folder picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your folder picker
        openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
        openPicker.FileTypeFilter.Add("*");

        // Open the picker for the user to pick a folder
        var folder = await openPicker.PickSingleFolderAsync();
        if (folder != null)
        {
            StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
            //PickFolderOutputTextBlock.Text = "Picked folder: " + folder.Name;
            Setting.SaveTo = folder.Path;
        }
        else
        {
            //PickFolderOutputTextBlock.Text = "Operation cancelled.";
        }

        //re-enable the button
        //senderButton.IsEnabled = true;

    }
    
    public void CursorPressed(Point position)
    {
        switch (WhatWay())
        {
            case CaptureWay.FullScreen:
                GetShot();
                CaptureMng.SaveTo(Setting.SaveTo + $"\\{System.DateTime.Now.ToString("YYYY_MM_DD_hh_mm_ss")}.png", memStrm);
                var btn = new AppNotificationButton("表示");
                btn.AddArgument("show", "current");
                var notification = new AppNotificationBuilder()
            .AddText(Cont.AppName)
            .AddText($"スクリーンショットを\n{System.DateTime.Now.ToString("YYYY_MM_DD_hh_mm_ss")}.png\nとして保存し、クリップボードにコピーしました。")
            .AddButton(btn)
            .BuildNotification();
                cws.MoveToMainWindow();
                cws.Close();
                break;
            case CaptureWay.AreaCapture:
                startPoint = position;

                selectionRect = new Rectangle
                {
                    Stroke = new SolidColorBrush(Cont.SelectionRectBorder),
                    StrokeThickness = 2,
                    Fill = new SolidColorBrush(Cont.SelectionRectArea) // 半透明
                };

                Canvas.SetLeft(selectionRect, startPoint.X);
                Canvas.SetTop(selectionRect, startPoint.Y);
                cws.AddRectToCanvas(selectionRect);
                break;
        }
    }
    
    public void CursorMoved(Point position)
    {

        if (selectionRect == null) return;


        var x = Math.Min(position.X, startPoint.X);
        var y = Math.Min(position.Y, startPoint.Y);
        var width = Math.Abs(position.X - startPoint.X);
        var height = Math.Abs(position.Y - startPoint.Y);

        Canvas.SetLeft(selectionRect, x);
        Canvas.SetTop(selectionRect, y);
        selectionRect.Width = width;
        selectionRect.Height = height;
    }

    public void CursorReleased(Point position)
    {
        var x = (int)Math.Min(position.X, startPoint.X);
        var y = (int)Math.Min(position.Y, startPoint.Y);
        var width = (int)Math.Abs(position.X - startPoint.X);
        var height = (int)Math.Abs(position.Y - startPoint.Y);
        //トリミング
        //var srcrect = new System.Drawing.Rectangle(x,y,width,height);

        Triming(x,y,width,height);

        cws.MoveToMainWindow();
        cws.Close();


        SetClipboard();

        //通知
        var btn = new AppNotificationButton("表示");
        btn.AddArgument("show", "current");

        var notification = new AppNotificationBuilder()
    .AddText(Cont.AppName)
    .AddText($"スクリーンショットを\n{System.DateTime.Now.ToString("YYYY_MM_DD_hh_mm_ss")}.png\nとして保存し、クリップボードにコピーしました。")
    .AddButton(btn)
    .BuildNotification();

        AppNotificationManager.Default.Show(notification);
    }

    private void Triming(int x, int y, int w, int h)
    {
        CurrentImgSource = CaptureMng.Trim(x,y,w,h, memStrm);
        CaptureMng.SaveTo(Setting.SaveTo + $"\\{System.DateTime.Now.ToString("YYYY_MM_DD_hh_mm_ss")}.png", memStrm);
    }


    [RelayCommand]
    private void FullScreenCap()
    {

    }
}
