using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzusaCaptureApp;

public class BIandMS
{
    public BIandMS()
    {
        ms = new();
    }

    public MemoryStream ms { get; private set; } = new();
    public BitmapImage bi { get;private set; }

    public void Set(MemoryStream m, BitmapImage b)
    {
        ms = m;
        bi = b;
        //callback(bi);
    }

    public BIandMS CopyInstance()
    {
        var r = new BIandMS();
        ms.CopyTo(r.ms);
        r.Set(r.ms, CaptureMng.ConvertFromBytes(ms.ToArray()));

        return r;
    }
}
