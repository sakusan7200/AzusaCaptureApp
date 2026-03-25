using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace AzusaCaptureApp;

public class MainWindowService : IMainWindowService
{
    public MainWindowService()
    {
        //this.w = w;
    }
    public void Close()
    {
        App.CurrentMainWindow.Close();
    }
    public void Minimaize()
    {
        App.CurrentMainWindow.AppWindow.Hide();
        //App.CurrentMainWindow.Close();
    }
    public void StartCapture()
    {//{
    //    App.CurrentCaptureWindow = new CaptureWindow(vm);
    //    App.CurrentCaptureWindow.Activate();

    }
    public void GoFullScr()
    {
        //App.CurrentCaptureWindow.AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen);
    }

    public void OpenSettingPage()
    {
        App.CurrentMainWindow.RootFrame.Navigate(typeof(SettingsPage));
    }

    public void MoveToMainWindow()
    {
        if (App.CurrentMainWindow != null)
        {
            App.CurrentMainWindow.Activate();
        }
        else
        {
            App.ShowMainWindow();
        }
    }

    public bool IsActive()
    {
        return App.CurrentMainWindow.Visible;
    }

    public bool IsOpened()
    {
        return true;
    }

    public void StartTriming()
    {
        MainPage.Singleton.StartTriming();
        
    }

    public void SetForeground()
    {
        var presenter = MainWindow.Instance.AppWindow.Presenter as OverlappedPresenter;
        presenter.IsAlwaysOnTop = !presenter.IsAlwaysOnTop;
        //MainWindow.Instance.AppWindow.SetPresenter(presenter);
    }

    public Microsoft.UI.Xaml.Shapes.Rectangle FinishTrim()
    {
        return MainPage.Singleton.DoneTriming();


        //l,t,w,hを返せばいい
    }
}
public interface IMainWindowService
{
    public void Close();
    public void Minimaize();
    public void StartCapture();
    public void OpenSettingPage();
    public void MoveToMainWindow();
    public bool IsActive();
    public bool IsOpened();
    public void StartTriming();
    public void SetForeground();
    public Rectangle FinishTrim();
}
