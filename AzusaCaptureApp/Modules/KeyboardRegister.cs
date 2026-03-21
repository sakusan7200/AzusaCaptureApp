using Microsoft.UI.Xaml.Input;
using NHotkey.WinUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzusaCaptureApp;

internal static class KeyboardRegister
{
    public static void Regist()
    {

        var a = new KeyboardAccelerator();
        a.Key = Windows.System.VirtualKey.Snapshot;
        a.Modifiers = Windows.System.VirtualKeyModifiers.Control;
        HotkeyManager.Current.AddOrReplace("OnPrintScreenPressed", a, (a,b) =>
            App.VM.StartCaptureCommand.Execute(null));
    }
}
