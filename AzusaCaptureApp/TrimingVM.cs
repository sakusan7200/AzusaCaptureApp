using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzusaCaptureApp;

//トリミング時に使うVM
public partial class MainViewModel
{
    [ObservableProperty] private double rectWidth;
    [ObservableProperty] private double rectHeight;

    [ObservableProperty] private double tL_Left;
    [ObservableProperty] private double tL_Top;
    [ObservableProperty] private double tR_Left;
    [ObservableProperty] private double tR_Top;
    [ObservableProperty] private double bL_Left;
    [ObservableProperty] private double bL_Top;
    [ObservableProperty] private double bR_Left;
    [ObservableProperty] private double bR_Top;
}
