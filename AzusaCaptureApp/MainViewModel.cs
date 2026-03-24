using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ImageMagick;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
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
        if (File.Exists(Cont.SettingDir + "\\settings.json"))
        {
            var jsonstr = File.ReadAllText(Cont.SettingDir + "\\settings.json");
            Setting = JsonConvert.DeserializeObject<AppSetting>(jsonstr);

        }
        else
        {
            Setting = new AppSetting();
            Setting.SaveTo = Cont.DefaultSaveDir;
        }

        AllFormats = new ObservableCollection<CompatibleFormat>() 
        { 
            new CompatibleFormat(MagickFormat.Png, "png", "PNG画像"),
            new CompatibleFormat(MagickFormat.Jpg, "jpg", "JPG画像"),
            new CompatibleFormat(MagickFormat.Avif, "avif", "AVIF画像"),
            new CompatibleFormat(MagickFormat.Heic, "heic", "HEIC画像"),

        };

        //標準ではエリアキャプチャ、設定で変更可能にする予定
        AreaCapChecked = true;

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(CurrentImg2))
            {
                OnPropertyChanged(nameof(IsDataNonNull));
            }

            if(e.PropertyName == nameof(WindowChecked))
            {
                //OnPropertyChanged(nameof(WindowChecked));

                cws.SwitchWindowRects(WindowChecked);
            }
        };
    }


    private void MainViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        //トグルボタンの切り替え
        if (e.PropertyName.EndsWith("Checked"))
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

    private BIandMS current = new();
    public BitmapImage CurrentImg2
    {
        get => current.bi;
    }
    private BIandMS fullsize = new();
    public BitmapImage FullSizeImg2
    {
        get => fullsize.bi;
    }

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

    public bool IsDataNonNull
    {
        get => CurrentImg2 != null;
    }

    public ObservableCollection<CompatibleFormat> AllFormats { get; set; } = new ObservableCollection<CompatibleFormat>();


    private CaptureWay WhatWay()
    {
        if (FullScrChecked) return CaptureWay.FullScreen;
        if (AreaCapChecked) return CaptureWay.AreaCapture;
        if (FreeHandChecked) return CaptureWay.FreeHand;
        if (FullScrChecked) return CaptureWay.OneWindow;
        if (WindowChecked) return CaptureWay.FullScreen;
        if (FullScrGIFChecked) return CaptureWay.FullScreenGIF;
        if (WindowGIFChecked) return CaptureWay.OneWindow;
        return CaptureWay.Nodefinded;
    }

    //[RelayCommand]
    //フルスクリーンでキャプチャし、メモリーストリームとImgSourceに保存する
    public void GetShot()
    {
        current.Set(current.ms, CaptureMng.CaptureFullScreen(current.ms));
        //CurrentImgSource = CaptureMng.CaptureFullScreen(current.ms);
        fullsize.Set(new MemoryStream(), CaptureMng.ConvertFrom(current.ms));   
        OnPropertyChanged(nameof(FullSizeImg2));
        OnPropertyChanged(nameof(CurrentImg2));

        current.ms.Position = 0;
        current.ms.CopyTo(fullsize.ms);

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
    private void StartTriming()
    {
        mws.StartTriming();
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
        CaptureMng.Init(Setting.SaveTo, current.ms);

        //mws.Close();
        GetShot();

        mws.MoveToMainWindow();

        var f_name = setting.GetFilenameFromFormat();

        CaptureMng.SaveTo(Setting.SaveTo + $"\\{f_name}.png", current.ms, Setting.DefalutFormat.magickFormat);
        var btn = new AppNotificationButton("表示");
        btn.AddArgument("show", "current");
        // TODO: com exception 要素が見つかりません


        new ToastContentBuilder()
        .AddText($"スクリーンショットを\n{f_name}.png\nとして保存し、クリップボードにコピーしました。")
        .AddButton(
            new ToastButton()
            .SetContent("Reply")
            .AddArgument("action", "reply")
            .SetBackgroundActivation()
        )
        .Show();

        mws.MoveToMainWindow();
    }


    [RelayCommand]
    private void FinishTrim()
    {
        var rect = mws.FinishTrim();
        var img = CaptureMng.Trim((int)Canvas.GetLeft(rect), (int)Canvas.GetTop(rect),
            (int)rect.Width, (int)rect.Height,
            current.ms, fullsize.ms);

        current.Set(current.ms, img);
        OnPropertyChanged(nameof(CurrentImg2));
    }


    [RelayCommand]
    private void SetClipboard()
    {
        CaptureMng.SetCipboard(current.ms);
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
    }

    [RelayCommand]
    private async void StartCapture2()
    {
        CaptureMng.Init(Setting.SaveTo, current.ms);
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
        CaptureMng.Init(Setting.SaveTo, current.ms);
        if (mws.IsActive())
        {
            mws.Minimaize();
        }

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

        savePicker.FileTypeChoices.Clear();
        foreach (var f in AllFormats)
        {
            //Debug.WriteLine(f.formatName + " " + f.extension);
            savePicker.FileTypeChoices.Add(f.formatName, new List<string>() { "." + f.extension });
        }
        var file = await savePicker.PickSaveFileAsync();
        if(file != null)
        {
            CaptureMng.SaveTo(file.Path, current.ms, MagickFormat.Png);
        }
    }

    public void ShotByRect(AzusaRect rect)
    {

        DoTrim(rect.left, rect.top, rect.width, rect.height);
        cws.MoveToMainWindow();
        cws.Close();


        SetClipboard();

        //通知
    }

    public void CursorPressed(Point position)
    {
        var f_name = setting.GetFilenameFromFormat();
        switch (WhatWay())
        {
            case CaptureWay.FullScreen:
                GetShot();
                CaptureMng.SaveTo(Setting.SaveTo + $"\\{f_name}.png", current.ms, Setting.DefalutFormat.magickFormat);
                
                var btn = new AppNotificationButton("表示");
                btn.AddArgument("show", "current");
                var notification = new AppNotificationBuilder()
                    .AddText(Cont.AppName)
                    .AddText($"スクリーンショットを\n{f_name}.png\nとして保存し、クリップボードにコピーしました。")
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
        //TODO: これおshotbyrectに統合する

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

        DoTrim(x,y,width,height);

        cws.MoveToMainWindow();
        cws.Close();


        SetClipboard();

        //通知
        var btn = new AppNotificationButton("表示");
        btn.AddArgument("show", "current");

    }

    private void DoTrim(int x, int y, int w, int h)
    {
        current.Set(current.ms, CaptureMng.Trim(x, y, w, h, current.ms));
        OnPropertyChanged(nameof(CurrentImg2));
        CaptureMng.SaveTo(Setting.SaveTo + $"\\{setting.GetFilenameFromFormat()}.png", current.ms, Setting.DefalutFormat.magickFormat);
    }
}
