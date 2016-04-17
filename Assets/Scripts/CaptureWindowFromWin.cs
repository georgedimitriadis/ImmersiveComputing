using UnityEngine;
using System;
using System.Linq; 
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class CaptureWindowFromWin : MonoBehaviour {

    [DllImport("user32.dll")]
    private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
    
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    

    Texture2D tex;
    Rectangle screenSize;
    Bitmap target;
    MemoryStream ms;

    RECT WndRect;
    int previousWndWidth;
    int previousWndHeight;


    public IntPtr hWnd = IntPtr.Zero;
    public int WndWidth;
    public int WndHeight;
    public bool captureDesktop = false;
    public bool inView;



	// Use this for initialization
	void Start () {
        WndRect = new RECT();

        WndWidth = WndRect.Right - WndRect.Left;
        WndHeight = WndRect.Bottom - WndRect.Top;
        previousWndWidth = WndWidth;
        previousWndHeight = WndHeight;
        tex = new Texture2D(WndWidth, WndHeight, TextureFormat.RGB24, false);//RGB565 RGB24
       
        GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Diffuse");
        float a = 1.0F;
        GetComponent<Renderer>().material.SetColor("_Color", new UnityEngine.Color(a, a, a, 1.0F));
        GetComponent<Renderer>().material.SetFloat("_Mode", 2.0f);
        GetComponent<Renderer>().material.SetFloat("_Glossiness", 1.0f);


        //I have no clue why the texture requires an offset of 0.02 on the x axis but it does (at least on my implementation)
        //GetComponent<Renderer>().material.SetTextureOffset("_MainTex",  new Vector2(0.02F, 0F));

        //transform.Rotate(new Vector3(90, 180, 0));
        //transform.position = new Vector3(0, 0, 400);

        inView = false;

        InvokeRepeating("draw", 0, 0.1F);
	}
	

	void Update () {

        //Updates the texture only when the panel is in front of the camera
        RaycastHit hit;
        Transform cam = Camera.main.transform;
        inView = false;
        GameObject.Find("Player").GetComponent<KeysAndMouseServer>().currentWindowInView = IntPtr.Zero;
        if(Physics.Raycast (cam.position, cam.forward, out hit, 500))
        {
            if (hit.transform == transform)
            {
                inView = true;
                GameObject.Find("Player").GetComponent<KeysAndMouseServer>().currentWindowInView = hWnd;
            }
        }
	}



    private void draw()
    {
        if (hWnd != IntPtr.Zero && inView)
        {
            GetWindowRect(hWnd, ref WndRect);

            WndWidth = WndRect.Right - WndRect.Left;
            WndHeight = WndRect.Bottom - WndRect.Top;

            //There is a weird bug that skews the texture when the x number of pixels is odd so this keeps it even
            if (WndWidth % 2 == 1)
                WndWidth -= 1;

            target = new Bitmap(WndWidth, WndHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);//Format16bppRgb565

            if (WndWidth != previousWndWidth || WndHeight != previousWndHeight)
            {
                tex.Resize(WndWidth, WndHeight);
                gameObject.transform.parent.transform.localScale = new Vector3((float)WndWidth / 1000F, (float)WndHeight / 1000F, 1.0F);
            }
            previousWndWidth = WndWidth;
            previousWndHeight = WndHeight;


            if (captureDesktop)
            {
                CaptureDesktop();
                screenSize = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            }
            else
                CaptureWindow();
        }
    }


    public void CaptureWindow()
    {
        Rectangle rctForm = System.Drawing.Rectangle.Empty;
        using (System.Drawing.Graphics grfx = System.Drawing.Graphics.FromHdc(GetWindowDC(hWnd)))
        {
            rctForm = System.Drawing.Rectangle.Round(grfx.VisibleClipBounds);
        }
        
        System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(target);
        IntPtr hDC = graphics.GetHdc();
        try
        {
            PrintWindow(hWnd, hDC, (uint)0);
        }
        finally
        {
            graphics.ReleaseHdc(hDC);
        }
        
        PutBitmapOnTexture();
    }




    void CaptureDesktop()
    {
        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(target))
        {
            g.CopyFromScreen(0, 0, 0, 0, new Size(screenSize.Width, screenSize.Height));

        }

        PutBitmapOnTexture();
    }




    void PutBitmapOnTexture()
    {
        ms = new MemoryStream();
        target.Save(ms, ImageFormat.Png);//Bmp
       
        ms.Seek(0, SeekOrigin.Begin);
        tex.LoadImage(ms.ToArray());
		GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
        target.Dispose();
        ms.Dispose();
    }




    //public static void RGBtoBGR(Bitmap bmp)
    //{
    //    BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
    //                                   ImageLockMode.ReadWrite, bmp.PixelFormat);

    //    int length = Math.Abs(data.Stride) * bmp.Height;

    //    unsafe
    //    {
    //        byte* rgbValues = (byte*)data.Scan0.ToPointer();

    //        for (int i = 0; i < length; i += 3)
    //        {
    //            byte dummy = rgbValues[i];
    //            rgbValues[i] = rgbValues[i + 2];
    //            rgbValues[i + 2] = dummy;
    //        }
    //    }

    //    bmp.UnlockBits(data);
    //}

}

