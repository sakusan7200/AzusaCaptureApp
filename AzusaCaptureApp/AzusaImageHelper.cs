using ImageMagick;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AzusaCaptureApp;

//画像のライフサイクル関係
internal class AzusaImageHelper
{
    /// <summary>
    /// 全画面キャプチャし、ローカルのmsにセット
    /// </summary>
    /// <param name="edited_callback">編集され、保存されたときのコールバック</param>
    public AzusaImageHelper(bool isTakeCapture = true)
    {
        if (isTakeCapture)
        {
            current.Set(current.ms, CaptureMng.CaptureFullScreen(current.ms));
            full.Set(new MemoryStream(), CaptureMng.ConvertFrom(current.ms));
            current.ms.Position = 0;
            current.ms.CopyTo(full.ms);
        }
    }

    public bool IsSaved { get; private set; } = false;
    public string LastSavedFilePath { get; private set; } = "";

    private BIandMS current { get; set; } = new();
    private BIandMS full { get; set; } = new();

    public BitmapImage CurrentBitmapImage { get => current.bi; }
    public BitmapImage FullBitmapImage { get => full.bi; }

    public void Trim(AzusaRect r)
    {
        var img = CaptureMng.Trim(r.left, r.top, r.width, r.height, current.ms, full.ms);
        current.Set(current.ms, img);
        IsSaved = false;
    }

    public void Save(string path, MagickFormat format)
    {
        CaptureMng.SaveTo(path, current.ms, format);
        LastSavedFilePath = path;
        IsSaved = true;
    }

    public void SetClipboard()
    {
        CaptureMng.SetCipboard(current.ms);
    }

}
