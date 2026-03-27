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
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI;

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

        Setting = new AppSetting();
        //設定読み出し
        if (File.Exists(Cont.SettingDir + "\\settings.json"))
        {
            var jsonstr = File.ReadAllText(Cont.SettingDir + "\\settings.json");
            Setting = JsonConvert.DeserializeObject<AppSetting>(jsonstr);

            SettingDefaultFormatIndex = Setting.DefaultSaveFormatIndex;

        }
        else
        {
            Setting = new AppSetting();
            Setting.SaveTo = Cont.DefaultSaveDir;
        }

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
                cws.SwitchWindowRects(WindowChecked);
            }

            if(e.PropertyName == nameof(SettingDefaultFormatIndex))
            {
                Setting.DefaultSaveFormatIndex = SettingDefaultFormatIndex;
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

    public BitmapImage CurrentImg2
    {
        get => aih.CurrentBitmapImage;
    }
    public BitmapImage FullSizeImg2
    {
        get => aih.FullBitmapImage;
    }

    Point startPoint;
    Rectangle selectionRect;

    private AzusaImageHelper former_aih;
    private AzusaImageHelper aih = new(isTakeCapture:false);

    [ObservableProperty] private bool fullScrChecked;
    [ObservableProperty] private bool areaCapChecked;
    [ObservableProperty] private bool freeHandChecked;
    [ObservableProperty] private bool windowChecked;
    [ObservableProperty] private bool fullScrGIFChecked;
    [ObservableProperty] private bool windowGIFChecked;

    [ObservableProperty] private bool isAlwaysTopChecked;

    [ObservableProperty] private AppSetting setting;

    public bool IsDataNonNull
    {
        get => CurrentImg2 != null;
    }

    [ObservableProperty] private int settingDefaultFormatIndex;

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
        aih = new();  
        OnPropertyChanged(nameof(FullSizeImg2));
        OnPropertyChanged(nameof(CurrentImg2));
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

        GetShot();

        mws.MoveToMainWindow();

        var f_name = setting.GetFilenameFromFormat();
        aih.Save(Setting.SaveTo + $"\\{f_name}.{Setting.DefaultSaveFormat.extension}", Setting.DefaultSaveFormat.magickFormat);
        ShowSavedNotification($"{ f_name}.{ Setting.DefaultSaveFormat.extension}");
        mws.MoveToMainWindow();
    }


    [RelayCommand]
    private void FinishTrim()
    {
        var rect = mws.FinishTrim();
        var ar = rect.ConvertToInternalRect();
        aih.Trim(ar);
        OnPropertyChanged(nameof(CurrentImg2));
    }

    [RelayCommand]
    private void SetClipboard()
    {
        aih.SetClipboard();
    }

    [RelayCommand]
    private async void OpenInAnotherApp()
    {
        if (!aih.IsSaved)
        {
            await SaveTo();
        }
        var psi = new ProcessStartInfo();
        psi.UseShellExecute = true;
        psi.FileName = aih.LastSavedFilePath;
        Process.Start(psi);

    }

    [RelayCommand]
    private void CancelBtn()
    {
        aih = new AzusaImageHelper(false);

        OnPropertyChanged(nameof(CurrentImg2));
        OnPropertyChanged(nameof(FullSizeImg2));


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
        var path = await FileDialogMng.ShowOpenFolderDialog();
        if(path != null)
        {
            Setting.SaveTo = path;
        }
    }

    [RelayCommand]
    private async void StartCapture2()
    {
        mws.Minimaize();
        await Task.Delay(600);
        GetShot();
        App.ShowCaptureWindow();
    }

    [RelayCommand]
    private async void CaptureRightNow()
    {
        if (mws.IsActive())
        {
            mws.Minimaize();
        }

        GetShot();

        mws.MoveToMainWindow();
    }

    [RelayCommand]
    private void SetForeground()
    {
        mws.SetForeground();
    }


    [RelayCommand]
    private async Task SaveTo()
    {
        var r = await FileDialogMng.ShowSaveFileDialog(CompatibleFormat.AllFormats, Setting.DefaultSaveFormat);
        aih.Save(r.Item1, r.Item2.magickFormat);
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
                cws.MoveToMainWindow();
                Task.Delay(600);
                StartCaptureCommand.Execute(null);
                break;
            case CaptureWay.AreaCapture:
                startPoint = position;

                selectionRect = new Rectangle
                {
                    Stroke = new SolidColorBrush(Cont.SelectionRectBorder),
                    StrokeThickness = 2,
                    Fill = new SolidColorBrush(Color.FromArgb(100, 0xe6, 0xe6, 0xfa))
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
        DoTrim(x,y,width,height);

        cws.MoveToMainWindow();
        cws.Close();

        SetClipboard();
    }

    private void DoTrim(int x, int y, int w, int h)
    {
        aih.Trim(new AzusaRect() { left = x, top = y, right = x + w, bottom = y + h });
        OnPropertyChanged(nameof(CurrentImg2));
        aih.Save(Setting.SaveTo + $"{Setting.GetFilenameFromFormat()}.{Setting.DefaultSaveFormat.extension}", Setting.DefaultSaveFormat.magickFormat);
    }

    private void ShowSavedNotification(string filename)
    {
        new ToastContentBuilder()
        .AddText($"スクリーンショットを\n{filename}\nとして保存し、クリップボードにコピーしました。")
        .AddButton(
            new ToastButton()
            .SetContent("Reply")
            .AddArgument("action", "reply")
            .SetBackgroundActivation()
        )
        .Show();
    }
}
