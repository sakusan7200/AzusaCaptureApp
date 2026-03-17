using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using ImageMagick;
using System.Diagnostics;

namespace AzusaCaptureApp;

/// <summary>
/// msにはスクショ時以外書き込まない方針
/// </summary>
public static class CaptureMng
{
    const int w = 1920;
    const int h = 1080;
    //static MemoryStream ms;
    static string saveto = "";

    public static void Init(string s, MemoryStream ms)
    {
        ms = new MemoryStream();
        saveto = s;
    }

    public static BitmapImage CaptureFullScreen(MemoryStream ms)
    {
        var img = new MagickImage();
        img.Read("SCREENSHOT", MagickFormat.Screenshot);
        img.Format = MagickFormat.Bmp;

        ms.SetLength(0);
        ms.Write(img.ToByteArray());
        
        return ConvertFromBytes(img.ToByteArray());

    }

    public static BitmapImage Trim(int x, int y, int w, int h, MemoryStream ms)
    {
        return Trim(x, y, w, h, ms, ms);
    }

    public static BitmapImage Trim(int x, int y, int w, int h, MemoryStream ms, MemoryStream fullSizeBitmap)
    {
        //msにはクロップ済みのものが入ってるから、二回目に使っちゃダメ
        var bytes = new byte[fullSizeBitmap.Length];
        fullSizeBitmap.Position = 0;
        fullSizeBitmap.Read(bytes, 0, (int)fullSizeBitmap.Length);
        var img = new MagickImage(bytes);
        
        
        img.Crop(new MagickGeometry(x,y,(uint)w,(uint)h));
        ms.SetLength(0);
        ms.Capacity = 0;
        ms.Position = 0;
        ms.Write(img.ToByteArray());

        return ConvertFromBytes(img.ToByteArray());
    }

    public static BitmapImage ConvertFromBytes(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);

        var bitmapiamge = new BitmapImage();
        bitmapiamge.SetSource(ms.AsRandomAccessStream());
        return bitmapiamge;
    }
    public static BitmapImage ConvertFrom(MemoryStream m)
    {
        m.Position = 0;
        var bitmapiamge = new BitmapImage();
        bitmapiamge.SetSource(m.AsRandomAccessStream());
        return bitmapiamge;
    }



    public static void SaveTo(string path, MemoryStream ms, MagickFormat format)
    {
        if(format == MagickFormat.Unknown)
        {
            format = MagickFormat.Png;
        }
        ms.Position = 0;
        var img = new MagickImage(ms);
        img.Format = format;
        using var fs = new FileStream(path, FileMode.Create);
        Debug.WriteLine("aaaa");
        img.Write(fs);
    }
    
    public static void SetCipboard(MemoryStream ms)
    {
        //msにはpng形式の画像が入ってる
        var bmp = new System.Drawing.Bitmap(ms);
       System.Windows.Forms.Clipboard.SetImage(bmp);
    }
}
