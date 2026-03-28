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
using ImageMagick;
using Microsoft.UI.Xaml;

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
        using var sw = new StreamWriter(Cont.SettingDir + "\\settings.json");
        sw.Write(jsonstr);
    }

    public CompatibleFormat DefaultSaveFormat
    {
        get
        {
            return CompatibleFormat.AllFormats[DefaultSaveFormatIndex];
        }
    }

    [ObservableProperty] public string saveTo;
    [ObservableProperty] public bool autoSave;
    [ObservableProperty] public bool autoCopy;
    //[ObservableProperty] public CompatibleFormat defaultFormat;
    [ObservableProperty] public int defaultSaveFormatIndex;

    public bool AutoStart
    {
        get
        {
            Microsoft.Win32.RegistryKey regkey =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                Cont.AutoStartRegDir, true);

            var a = regkey.GetValue(Cont.AppName);

            return a != null;
        }

        set
        {
            Microsoft.Win32.RegistryKey regkey =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                Cont.AutoStartRegDir, true);
            if (value == true)
            {
                regkey.SetValue(Cont.AppName, Environment.ProcessPath + " -b");
            }
            else
            {
                regkey.DeleteValue(Cont.AppName);
            }
            regkey.Close();

            OnPropertyChanged(nameof(AutoStart));
        }
    }

    public string GetFilenameFromFormat()
    {
        return DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
    }
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

public struct CompatibleFormat
{
    public static List<CompatibleFormat> AllFormats { get; } = new List<CompatibleFormat>()
    {
            new CompatibleFormat(MagickFormat.Png, "png", "PNG画像"),
            new CompatibleFormat(MagickFormat.Jpg, "jpg", "JPG画像"),
            new CompatibleFormat(MagickFormat.Avif, "avif", "AVIF画像"),
            new CompatibleFormat(MagickFormat.WebP, "webp", "WEBP画像"),
            new CompatibleFormat(MagickFormat.Gif, "gif", "GIF画像"),
            new CompatibleFormat(MagickFormat.Heic, "heic", "HEIC画像"),
            new CompatibleFormat(MagickFormat.Bmp, "bmp", "ビットマップイメージ"),
            new CompatibleFormat(MagickFormat.Icon, "ico", "アイコン"),
            new CompatibleFormat(MagickFormat.Eps, "eps", "EPS画像"),
            new CompatibleFormat(MagickFormat.Raw, "raw", "RAW画像"),
            new CompatibleFormat(MagickFormat.Pdf, "pdf", "PDFドキュメント"),
            new CompatibleFormat(MagickFormat.Svg, "svg", "svg画像"),
            new CompatibleFormat(MagickFormat.Tiff, "tiff", "TIFF画像"),
    };

    public CompatibleFormat(MagickFormat f, string e, string fn)
    {
        magickFormat = f;
        extension = e;
        formatName = fn;
    }
    public MagickFormat magickFormat { get; private set; }
    public string extension { get; private set; }
    public string formatName { get; private set; }


    public override string ToString()
    {
        return formatName;
    }

    public static implicit operator CompatibleFormat(int v)
    {
        throw new NotImplementedException();
    }
}