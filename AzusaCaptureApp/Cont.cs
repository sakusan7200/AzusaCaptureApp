using Microsoft.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Color = Windows.UI.Color;

namespace AzusaCaptureApp;

public static class Cont
{
    public static void Init()
    {

    }

    public static string AppName = "AzusaCapture";
    public static readonly string RomingDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    public static readonly string SettingDir = RomingDir + "\\AzusaCaptureApp";

    public static readonly string MyPictureDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
#if DEBUG
    public static readonly string DefaultSaveDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\AC_DEBUG\\";
#else
    public static readonly string DefaultSaveDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
#endif
    public static string WinUsrName { get;private set; }

    public static readonly Color SelectionRectBorder = Colors.Blue;
    public static readonly Color SelectionRectArea = Color.FromArgb(50, 0, 0, 255);
    public static readonly Color Gray = Color.FromArgb(100, 26, 26, 26);

    public const int FullHD_W = 1980;
    public const int FullHD_H = 1080;
    public const int MyMonitor_W = 1920;
}
