using System.Runtime.CompilerServices;

namespace ZoomViewMauiDev;

internal static class Calcer
{
    //a‚Íb‚Ì“à•”‚É‚ ‚é‚©
    internal static bool IsContainRectOnRect(Rect a, Rect b)
    {
        return a.Left >= b.Left && a.Left + a.Width <= b.Left + b.Width && a.Top >= b.Top && a.Top + a.Height <= b.Top + b.Height;
    }
    //a‚Íb‚Ì“à•”‚É‚ ‚é‚©
    internal static bool IsContainPointOnRect(Point a, Rect b)
    {
        return a.X >= b.Left && a.X <= b.Left + b.Width && a.Y >= b.Top && a.Y <= b.Top + b.Height;
    }
}

internal struct Point
{
    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }
    public double X { get; set; }
    public double Y { get; set; }
}

internal struct Rect
{

    public Rect(double left, double top, double width, double height)
    {
        Left = left;
        Top = top;
        Width = width;
        Height = height;
    }

    public Rect(Point topLeft, Point topRight, Point bottomLeft, Point bottomRight)
    {
        Left = topLeft.X;
        Top = topLeft.Y;
        Width = topRight.X - topLeft.X;
        Height = bottomLeft.Y - topLeft.Y;
    }

    public double Left { get; set; }
    public double Top { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}