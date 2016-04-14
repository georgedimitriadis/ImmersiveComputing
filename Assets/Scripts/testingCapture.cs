using UnityEngine;
using System;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;



public class testingCapture : MonoBehaviour {

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);


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


    IntPtr hWnd;
    RECT WndRect;
    int WndWidth;
    int WndHeight;

    List<IntPtr> windows;
    string title;

	void Awake () {

        hWnd = IntPtr.Zero;
        windows = new List<IntPtr>();

        title = "Firefox";
        FindWindowsWithText();

        hWnd = windows[0];

        GameObject.Find("Plane1").GetComponent<CaptureWindowFromWin>().hWnd = hWnd;
	}
	



	void Update () {
	}



    public string GetWindowText(IntPtr hWnd)
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


    public void FindWindowsWithText()
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
        //UnityEngine.Debug.Log(text + ":  " + x.ToString() + ", " + y.ToString());
        if (text.Contains(title))
        {
            windows.Add(window);
        }
        return true;
    }




    

}
