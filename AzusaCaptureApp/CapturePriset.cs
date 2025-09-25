using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO.Packaging;
using Windows.ApplicationModel;

namespace AzusaCaptureApp;

public partial class AppSetting : ObservableObject
{
    public AppSetting()
    {
        PropertyChanged += AppSetting_PropertyChanged;
    }

    private void AppSetting_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (!Directory.Exists(Cont.SettingDir))
        {
            Directory.CreateDirectory(Cont.SettingDir);
        }
        var jsonstr = JsonConvert.SerializeObject(this);
        //File.WriteAllText(Cont.SettingDir + "\\settings.json", jsonstr);

        using var sw = new StreamWriter(Cont.SettingDir + "\\settings.json");
        sw.Write(jsonstr);

        var fullpath = Windows.ApplicationModel.Package.Current.Id;
    }

    [ObservableProperty] public string saveTo;
    [ObservableProperty] public CaptureWay defaultWay;
    [ObservableProperty] public bool autoSave;
    [ObservableProperty] public bool autoCopy;
}

public enum CaptureWay
{
    Nodefinded,
    FullScreen,
    AreaCapture,
    FreeHand,
    OneWindow,
    FullScreenGIF,
    OneWindowGIF
}