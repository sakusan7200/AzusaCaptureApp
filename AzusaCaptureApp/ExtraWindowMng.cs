using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
//using static WinFormsApp1.NativeMethods;
public struct RECT
{
    public int left;
    public int top;
    public int right;
    public int bottom;
}
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
public static class WindowEnumerator
{
    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);


    
    // Win32 API のインポート
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd); // 最小化チェック

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TOOLWINDOW = 0x00000080; // タスクバーに出ないウィンドウ
    private const int WS_EX_NOACTIVATE = 0x08000000; // フォーカスを奪わないウィンドウ

    // コールバック関数の型定義
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    // ウィンドウ情報を保持するクラス
    public class WindowInfo
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; }
        public uint ProcessId { get; set; }

        public override string ToString() =>
            $"Handle: 0x{Handle:X8} | PID: {ProcessId,6} | Title: {Title}";
    }

    /// <summary>
    /// 表示中の全ウィンドウを列挙します（タイトルなしは除外）
    /// </summary>
    public static List<WindowInfo> EnumerateWindows(bool visibleOnly = true)
    {
        var windows = new List<WindowInfo>();

        EnumWindows((hWnd, lParam) =>
        {
            // 非表示ウィンドウ、最小化をスキップ
            if (visibleOnly && (!IsWindowVisible(hWnd) || IsIconic(hWnd)))
                return true;

            // タイトルを取得
            int length = GetWindowTextLength(hWnd);
            if (length == 0)
                return true; // タイトルなしはスキップ

            var sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);

            // プロセスIDを取得
            GetWindowThreadProcessId(hWnd, out uint pid);

            windows.Add(new WindowInfo
            {
                Handle = hWnd,
                Title = sb.ToString(),
                ProcessId = pid
            });

            return true; // 列挙を継続
        }, IntPtr.Zero);

        return windows;
    }

    public static bool IsUserWindow(IntPtr hWnd)
    {
        if (!IsWindowVisible(hWnd)) return false;
        if (IsIconic(hWnd)) return false;
        

        var textLength = GetWindowTextLength(hWnd);
        var sb = new StringBuilder(textLength + 1);
        GetWindowText(hWnd, sb, sb.Capacity);

        if (sb.ToString() == "Windows 入力エクスペリエンス") return false;

        int len = GetWindowTextLength(hWnd);
        if (len == 0) return false;

        int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);

        // タスクバーに表示されないウィンドウを除外
        if ((exStyle & WS_EX_TOOLWINDOW) != 0) return false;

        // フォーカスを奪わない常駐系を除外（オーバーレイ等）
        if ((exStyle & WS_EX_NOACTIVATE) != 0) return false;

        return true;
    }

    public static AzusaRect GetAzusaRect(this WindowInfo wi)
    {
        var rect = new RECT();
        GetWindowRect(wi.Handle, out rect);
        var r = new AzusaRect() { left = rect.left, right = rect.right, bottom = rect.bottom, top = rect.top };
        return r;
    }
}