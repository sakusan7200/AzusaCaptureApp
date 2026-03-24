using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzusaCaptureApp;

public struct AzusaRect
{
    public int left;
    public int top;
    public int right;
    public int bottom;
    public int width
    {
        get => right - left;
    }

    public int height
    {
        get => bottom - top;
    }

    public override string ToString()
    {
        return $"{left},{top} {right},{bottom}";
    }
}


public static class Calc
{


    //aはbに含まれているか
    public static bool isContainedAnotherRect(AzusaRect a, AzusaRect b)
    {
        return a.left >= b.left && a.left + a.width <= b.left + b.width && a.top >= b.top && a.top + a.height <= b.top + b.height;
    }
}
