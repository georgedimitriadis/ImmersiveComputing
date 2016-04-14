using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsInput;
using System.Diagnostics;
using System.Threading;
using WindowsInput.Native;

public class KeysAndMouseServer : MonoBehaviour {

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    static extern short VkKeyScan(char ch);

    public IntPtr currentWindowInView;

    private InputSimulator inputSimulator = new InputSimulator();

    private ClickDetector clickDetector;

    private int winInpScrRes = 65535;

    private IntPtr thisAppHandle;



	void Start () {
        currentWindowInView = IntPtr.Zero;
        clickDetector = GameObject.Find("Player").GetComponent<ClickDetector>();

        thisAppHandle = gameObject.GetComponent<SpecificInstanceOfGameExample>().windowHandle;
	}
	

	void Update () {
        if (Input.anyKeyDown)
        {

            if (currentWindowInView != IntPtr.Zero && Input.inputString.Length > 0)
            {
                print(Input.inputString[0].ToString());
                sendKeystroke(currentWindowInView, Input.inputString[0]);
            }

           
        }
	}



    [StructLayout(LayoutKind.Explicit)]
    struct Helper
    {
        [FieldOffset(0)]
        public short Value;
        [FieldOffset(0)]
        public byte Low;
        [FieldOffset(1)]
        public byte High;
    }

    private void sendKeystroke(IntPtr hWind, char k)
    {
        SetForegroundWindow(hWind);
        var helper = new Helper { Value = VkKeyScan(k) };
        byte virtualKeyCode = helper.Low;

        inputSimulator.Keyboard.KeyDown((VirtualKeyCode)virtualKeyCode);
        
        //Thread.Sleep(3);
        //SetForegroundWindow(thisAppHandle);
    }

    void OnLeftClick(RaycastHit hit)
    {
        Renderer renderer = hit.transform.GetComponent<MeshCollider>().GetComponents<Renderer>()[0] as Renderer;
        Vector2 uvCoords = new Vector2((int)(hit.textureCoord.x * renderer.material.mainTexture.width), (int)(hit.textureCoord.y * renderer.material.mainTexture.height));

        IntPtr hWind = (IntPtr)UInt32.Parse(hit.transform.name);
        SetForegroundWindow(hWind);

        CaptureWindowFromWin capWind = hit.transform.GetComponent<CaptureWindowFromWin>();

        int currentCursorXpos = (int)((System.Windows.Forms.Cursor.Position.X * winInpScrRes) / SystemInformation.VirtualScreen.Width);
        int currentCursorYpos = (int)((System.Windows.Forms.Cursor.Position.Y * winInpScrRes) / SystemInformation.VirtualScreen.Height);

        int newCursorXpos = (int)((uvCoords.x * winInpScrRes) / SystemInformation.VirtualScreen.Width);
        int newCursorYPos = (int)(((capWind.WndHeight - uvCoords.y) * winInpScrRes) / SystemInformation.VirtualScreen.Height);

        inputSimulator.Mouse.MoveMouseToPositionOnVirtualDesktop(newCursorXpos, newCursorYPos);
        inputSimulator.Mouse.LeftButtonDoubleClick();
        inputSimulator.Mouse.MoveMouseToPositionOnVirtualDesktop(currentCursorXpos, currentCursorYpos);
        SetForegroundWindow(thisAppHandle);
    }

    private void sendMousePress(IntPtr hWind, Vector2 position, int mouseButton, int pressType)
    {
        

    }

}
