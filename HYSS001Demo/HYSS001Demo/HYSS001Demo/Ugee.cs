using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HYSS001Demo
{
    public class Ugee
    {
        public static string SavePath = "";

        public static string SignImgPath = "";
        public static string FingerImgPath = "";
        public static string SignFingerImgPath = "";
        public static string MergePath = "";

        public static string SignImgFullPath = "";
        public static string FingerImgFullPath = "";
        public static string SignFingerImgFullPath = "";
        public static string MergeFullPath = "";

        public static string MergeFileType = ".png";

        public static bool HasSign = false;//Be signing
        public static bool HasEvaluate = false;//Under evaluation


        //Gets the position of the scroll button for the specified scroll bar
        [DllImport("user32.dll", EntryPoint = "GetScrollPos")]
        public static extern int GetScrollPos(IntPtr hwnd, int nBar);

        //:Sets the position of the scroll button in the specified scroll bar
        [DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        //Gets the maximum and minimum position of the scroll button for the specified scroll bar
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetScrollRange")]
        public static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

        //这个函数是关键中的关键,它负责向Windows控件发送相应的消息,以真正执行相应的操作。
        //一些网友实现了滚动条中滑块位置的移动,但却没有引起控件中内容的移动,
        //其原因就是因为没有调用这个函数,没有把移动内容的消息发送给控件。

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "PostMessage")]
        public static extern bool PostMessage(IntPtr hWnd, int nBar, int wParam, int lParam);

        public const int SB_HORZ = 0x0;
        public const int SB_VERT = 0x1;
        public const int WM_HSCROLL = 0x114;
        public const int WM_VSCROLL = 0x115;
        public const int SB_THUMBPOSITION = 4;

        /// <summary>
        /// PDF坐标信息
        /// </summary>
        public class PositionInfo
        {
            public int pageNO;
            public int posX;
            public int posY;
        }

        /// <summary>
        /// Turn on the device
        /// </summary>
        /// <returns>返回值：0：Open successfully；Other values: Failed to open</returns>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeOpenDevice")]
        public extern static int UgeeOpenDevice();

        /// <summary>
        /// Register button location, up to 32 buttons can be registered
        /// </summary>
        /// <param name="szBtnPos">Parameter: Button position information，top@left@bottom@right@btn_id@。It is recommended that btn_id start from 10</param>
        /// <returns>Returned value：0：Success；Other values: failed</returns>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeRegisterBtnPosInfo")]
        public extern static int UgeeRegisterBtnPosInfo(string szBtnPos);

        /// <summary>
        /// 注册按钮响应回调函数
        /// </summary>
        /// <param name="status">参数：回调函数指针</param>
        /// <returns>Return value: 0: success; Other values: failed</returns>
        public delegate int UgeeRegisterBtnCallBack_delegate(int status);

        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "UgeeRegisterBtnCallBack")]
        public extern static int UgeeRegisterBtnCallBack(UgeeRegisterBtnCallBack_delegate callback);

        /// <summary>
        /// 注销按钮响应回调函数
        /// </summary>
        /// <param name="status">返回值：0：成功；其他值：失败</param>
        /// <returns>参数：回调函数指针</returns>
        public delegate int UgeeUnregisterBtnCallBack_delegate(int status);

        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "UgeeUnregisterBtnCallBack")]
        public extern static int UgeeUnregisterBtnCallBack(UgeeUnregisterBtnCallBack_delegate callback);

        /// <summary>
        /// Start signing
        /// </summary>
        /// <param name="left">Parameter 1: Coordinates to the left of the signature box, based on client coordinates</param>
        /// <param name="top">Parameter 2: Coordinates of the top side of the signature box</param>
        /// <param name="width">Parameter 3: Width of the signature box</param>
        /// <param name="height">Parameter 4: Height of signature box</param>
        /// <param name="penWidth">Parameter 5: Pen width</param>
        /// <param name="szSignPath">Parameter 6: Signature picture path (do not put the system disk root path)</param>
        /// <returns>Return value: 0: success; Other values: failed</returns>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeStartSign")]
        public extern static int UgeeStartSign(int left, int top, int width, int height, int penWidth, string szSignPath);

        /// <summary>
        /// Take fingerprints
        /// </summary>
        /// <param name="szFingerPath">Parameter 1: Signature picture path</param>
        /// <param name="szSignFPath">Parameter 2: Signature + fingerprint path</param>
        /// <returns>Return value: 0: success; Other values: failed</returns>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeStartFinger")]
        public extern static int UgeeStartFinger(int quality, string szFingerPath, string szSignFPath);

        /// <summary>
        /// Close Window
        /// </summary>
        /// <returns>Return value: 0: success; Other values: failed</returns>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeCloseWindow")]
        public extern static int UgeeCloseWindow();

        /// <summary>
        /// Close Device
        /// </summary>
        /// <returns>Return value: 0: success; Other values: failed</returns>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeCloseDevice")]
        public extern static int UgeeCloseDevice();
        /// <summary>
        /// Turn on the camera
        /// </summary>
        /// <returns>Return value: 0: success; Other values: failed</returns>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeOpenCamera")]
        public extern static int UgeeOpenCamera();

        /// <summary>
        /// Gets the resolution supported by the camera
        /// </summary>
        /// <returns>Return value: resolution, such as"640*480@800*600”</returns>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeGetResolution")]
        public extern static IntPtr UgeeGetResolution();

        /// <summary>
        /// Set camera resolution
        /// </summary>
        /// <param name="width">Parameter: Resolution</param>
        /// <param name="height">Parameter: Resolution</param>
        /// <returns>Return value: 0: success; Other values: failed</returns>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeSetResolution")]
        public extern static int UgeeSetResolution(string Resolution);

        /// <summary>
        /// Left and right mirror image
        /// </summary>
        /// <returns>Return value: 0: success; Other values: failed</returns>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeSetLRMirror")]
        public extern static int UgeeSetLRMirror();

        /// <summary>
        /// Get image frame
        /// </summary>
        /// <returns>Returned value: Image frame base64</returns>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeGetImgFrame")]
        public extern static int UgeeGetImgFrame(string framePath);

        /// <summary>
        /// Take a picture
        /// </summary>
        /// <returns>Returned value: Photo base64</returns>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeTakePhoto")]
        public extern static int UgeeTakePhoto(string photoPath);

        /// <summary>
        /// off the camera
        /// </summary>
        /// <returns>Return value: 0: success; Other values: failed</returns>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeCloseCamera")]
        public extern static int UgeeCloseCamera();

        /// <summary>
        /// Show or hide the signature window
        /// 0 hidden, 1 Show
        /// </summary>
        [DllImport("UgeeSignFIDCam.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UgeeShowWindow")]
        public extern static void UgeeShowWindow(int nCmdShow);

        /// <summary>
        /// Log section
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        public static void WriteLogs(string type, string content)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "ugee.log";
                if (!File.Exists(path))
                {
                    FileStream fs = File.Create(path);
                    fs.Close();
                }
                if (File.Exists(path))
                {
                    StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default);
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff > ") + type + " > " + content);
                    sw.Close();
                }
            }
            catch
            {
            }
        }
        //End process
        public static void StopProcess(string processName)
        {
            try
            {
                System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName(processName);
                foreach (System.Diagnostics.Process p in ps)
                {
                    WriteLogs("tip", "Force exit process~");

                    p.Kill();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}