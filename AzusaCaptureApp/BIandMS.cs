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
    public MemoryStream ms { get; private set; }
    public BitmapImage bi { get;private set; }

    public void Set(MemoryStream m, BitmapImage b)
    {
        ms = m;
        bi = b;
        //callback(bi);
    }
}
