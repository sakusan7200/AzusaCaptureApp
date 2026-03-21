using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomViewMauiDev;

internal static class Extensions
{
    internal static Rect ConvertToInternalRect(this Microsoft.UI.Xaml.Shapes.Rectangle a)
    {
        return new Rect(Canvas.GetLeft(a), Canvas.GetTop(a), a.Width, a.Height);
    }
}
