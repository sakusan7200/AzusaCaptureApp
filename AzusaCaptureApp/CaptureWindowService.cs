using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using System.Linq;
using System.IO;

namespace AzusaCaptureApp;

public class CaptureWindowService : ICaptureWindowService
{
    CaptureWindow w;
    public CaptureWindowService()
    {
        w = App.CurrentCaptureWindow;
        //this.w = w;
    }

    public void AddRectToCanvas(Rectangle rect)
    {
        App.CurrentCaptureWindow.AddRectToCanvas(rect);
    }
    public void Close()
    {
        App.CurrentCaptureWindow.Close();
    }

    bool isOnceActivated = false;
    public void GoFullScr()
    {
        //フルスクリーン
        if (!isOnceActivated)
        {
            //isOnceActivated = true;
            //File.AppendAllText("C:\\Users\\sakua\\Desktop\\.log", $"{DateTime.Now.ToString()} before_set_presenter\n");
            App.CurrentCaptureWindow.AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen);

            //File.AppendAllText("C:\\Users\\sakua\\Desktop\\.log", $"{DateTime.Now.ToString()} after_set_presenter\n");
        }
    }

    public void MoveToMainWindow()
    {
        if(App.CurrentCaptureWindow != null)
        {
            App.CurrentMainWindow.Activate();
        }
        else
        {
            App.ShowMainWindow();
        }
    }

    public void DisableWayButtons(CaptureWay wa)
    {
        //w.DisableButtons(wa);
    }


}
public interface ICaptureWindowService
{
    public void AddRectToCanvas(Rectangle rect);
    public void Close();
    public void GoFullScr();
    public void MoveToMainWindow();
    public void DisableWayButtons(CaptureWay w);
}
