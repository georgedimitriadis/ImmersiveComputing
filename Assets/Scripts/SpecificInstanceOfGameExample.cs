using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Text;

public class SpecificInstanceOfGameExample : MonoBehaviour
{
    [DllImport("kernel32.dll")]
    static extern uint GetCurrentThreadId();

     [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
     private static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);
 
     private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
 
     [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
     private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr extraData);


    public IntPtr windowHandle = IntPtr.Zero;
    bool bUnityHandleSet = false;
    HandleRef unityWindowHandle;


  
    void Start()
    {
        uint threadId = GetCurrentThreadId();

        EnumWindowsProc ewp = new EnumWindowsProc(EnumWindowsCallBack);
        EnumWindows(ewp, IntPtr.Zero);

        windowHandle = unityWindowHandle.Handle;
    }


    public bool EnumWindowsCallBack(IntPtr hWnd, IntPtr lParam)
    {
        int procid;
        int returnVal = GetWindowThreadProcessId(new HandleRef(this, hWnd), out procid);

        int currentPID = System.Diagnostics.Process.GetCurrentProcess().Id;
        HandleRef handle = new HandleRef(this, System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle);
        if (procid == currentPID)
        {
            unityWindowHandle = new HandleRef(this, hWnd);
            bUnityHandleSet = true;
            return false;
        }

        return true;
    }
}

