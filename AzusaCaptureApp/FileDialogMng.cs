using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AzusaCaptureApp;

internal static class FileDialogMng
{
    internal static async Task<(string, CompatibleFormat)> ShowSaveFileDialog(IEnumerable<CompatibleFormat> formatslist, CompatibleFormat defaultformat)
    {
        // Create a file picker
        var savePicker = new FileSavePicker();

        // See the sample code below for how to make the window accessible from the App class.
        var window = App.CurrentMainWindow;

        // Retrieve the window handle (HWND) of the current WinUI 3 window.
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        // Initialize the file picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

        // Set options for your file picker
        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        // Dropdown of file types the user can save the file as

        savePicker.FileTypeChoices.Clear();

        //デフォルト設定のフォーマットが頭に来るようにする
        savePicker.FileTypeChoices.Add(defaultformat.formatName, new List<string>() { "." + defaultformat.extension });

        var defaultformatIdx = formatslist.ToList().FindIndex(x => x.magickFormat == defaultformat.magickFormat);

        for (var i = 0; i < CompatibleFormat.AllFormats.Count; i++)
        {
            if (i == defaultformatIdx) continue;
            var f = CompatibleFormat.AllFormats[i];
            savePicker.FileTypeChoices.Add(f.formatName, new List<string>() { "." + f.extension });
        }

        var file = await savePicker.PickSaveFileAsync();
        if (file != null)
        {
            var magickformat = CompatibleFormat.AllFormats
                .Find(x => "." + x.extension == file.FileType)
                .magickFormat;
            var r = formatslist.First(x => x.magickFormat == magickformat);
            return (file.Path, r);
        }
        else
        {
            return (null, new CompatibleFormat());
        }

    }

    internal static async Task<string> ShowOpenFolderDialog()
    {
        // Create a folder picker
        var openPicker = new Windows.Storage.Pickers.FolderPicker();

        // See the sample code below for how to make the window accessible from the App class.
        var window = App.CurrentMainWindow;

        // Retrieve the window handle (HWND) of the current WinUI 3 window.
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        // Initialize the folder picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your folder picker
        openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
        openPicker.FileTypeFilter.Add("*");

        // Open the picker for the user to pick a folder
        var folder = await openPicker.PickSingleFolderAsync();
        if (folder != null)
        {
            return folder.Path;
            //Setting.SaveTo = folder.Path;
        }
        else
        {
            return null;
        }
    }
}
