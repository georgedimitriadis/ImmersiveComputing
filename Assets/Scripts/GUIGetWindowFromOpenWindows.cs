using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;



public class GUIGetWindowFromOpenWindows : MonoBehaviour
{

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);


    IntPtr hWnd;
    RECT WndRect;
    int WndWidth;
    int WndHeight;

    List<IntPtr> windows;
    GameObject dropDownPanel;
    GameObject scrollBar;
    GameObject buttonPrefab;

    bool dropDownOn = false;
    bool clickedAgain = false;


    void Awake()
    {
        hWnd = IntPtr.Zero;
        windows = new List<IntPtr>();
        dropDownPanel = GameObject.Find("CurrentlyOpenWindowsPanel");
        scrollBar = GameObject.Find("CurrentlyOpenWindowsScrollbar");
    }


    public void FindWindowsAndMakeDropDownMenu()
    {
        FindLargeWindowsWithTitle();
        string[] titles = new string[windows.Count];
        for (int i = 0; i < windows.Count; i++ )
        {
            titles[i] = GetWindowText(windows[i]);
        }

        Button[] buttons = dropDownPanel.GetComponent<PopulateDropdownMenu>().generateMenu(titles);

        Scrollbar sb = scrollBar.GetComponent<Scrollbar>();

        if (buttons != null)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                GameObject textObject = buttons[i].transform.FindChild("Text").gameObject;
                string text = textObject.GetComponent<Text>().text;
                buttons[i].onClick.AddListener(() => StartANewWindow(GetWindowWithTitle(text)));
            }
        }
    }




    private void StartANewWindow(IntPtr window) 
    {
        //GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //plane.name = window.ToString();
        //CaptureWindowFromWin capt = plane.AddComponent<CaptureWindowFromWin>();
        //plane.AddComponent<Pickable>();
        GameObject win = Instantiate(Resources.Load("Prefabs/ControlSphere")) as GameObject;
        GameObject view = win.transform.FindChild("MainView").gameObject;
        view.name = window.ToString();
        CaptureWindowFromWin capt = view.AddComponent<CaptureWindowFromWin>();
        capt.hWnd = window;
        dropDownPanel.GetComponent<PopulateDropdownMenu>().dropDownOn = false;
        dropDownPanel.GetComponent<PopulateDropdownMenu>().clickedAgain = false;
        dropDownPanel.GetComponent<PopulateDropdownMenu>().KillDropDownButtons();

        positionWindowToZero(window);
    }


    private void positionWindowToZero(IntPtr window)
    {
        const short SWP_NOMOVE = 0X2;
        const short SWP_NOSIZE = 1;
        const short SWP_NOZORDER = 0X4;
        const int SWP_SHOWWINDOW = 0x0040;

        RECT bounds = new RECT();
        GetWindowRect(window, ref bounds);
        int width = bounds.Right - bounds.Left;
        int height = bounds.Bottom - bounds.Top;
        SetWindowPos(window, 0, 0, 0, width, height, SWP_NOZORDER | SWP_SHOWWINDOW);
    }


    private void FindLargeWindowsWithTitle()
    {
        EnumWindowsProc ewp = new EnumWindowsProc(CheckWindowsText);
        EnumWindows(ewp, IntPtr.Zero);
    }


    private bool CheckWindowsText(IntPtr window, IntPtr param)
    {
        
        RECT bounds = new RECT();
        GetWindowRect(window, ref bounds);
        int x = bounds.Right - bounds.Left;
        int y = bounds.Bottom - bounds.Top;
        
        string text = GetWindowText(window);
        if (text != string.Empty  && !windows.Contains(window) && IsWindowVisible(window)) //&& x > 100 && y > 100)
        {
            windows.Add(window);
        }
        return true;
    }


    private string GetWindowText(IntPtr hWnd)
    {
        int size = GetWindowTextLength(hWnd);
        if (size++ > 0)
        {
            var builder = new StringBuilder(size);
            GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        return String.Empty;
    }


    private IntPtr GetWindowWithTitle(string title)
    {
        foreach (IntPtr window in windows)
        {
            IntPtr windowCaptured = window;
            if (title == GetWindowText(window))
            {
                return window;
            }
        }
        return (IntPtr)0;
    }
    

}

