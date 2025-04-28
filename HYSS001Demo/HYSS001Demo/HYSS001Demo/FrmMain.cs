using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace HYSS001Demo
{
    public partial class FrmMain : Form
    {
        FrmShow frmShow;
        FrmADV frmADV;
        FrmEvaluate frmEvaluate;

        Ugee.UgeeRegisterBtnCallBack_delegate Callback;
        Ugee.UgeeRegisterBtnCallBack_delegate CallbackEva;
        Ugee.UgeeUnregisterBtnCallBack_delegate UnCallback;

        //Camera correlation
        System.Timers.Timer TimerCamera; //获取图像帧定时器

        bool CameraOpen;//Is the camera on

        string FramePath;
        string PhotoPath;

        ArrayList alCompletedPdfList = new ArrayList();//存放已签字pdf文件路径

        public FrmMain()
        {
            InitializeComponent();

            //防止闪烁
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
        }
        #region 加载/初始化
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            Ugee.SavePath = Application.StartupPath + "\\file";
            Ugee.SignImgPath = Ugee.SavePath + "\\sign";
            Ugee.FingerImgPath = Ugee.SavePath + "\\finger";
            Ugee.SignFingerImgPath = Ugee.SavePath + "\\signFinger";

            Ugee.MergePath = Ugee.SavePath + "\\merge";

            if (!Directory.Exists(Ugee.SignImgPath))
            {
                Directory.CreateDirectory(Ugee.SignImgPath);
            }
            if (!Directory.Exists(Ugee.FingerImgPath))
            {
                Directory.CreateDirectory(Ugee.FingerImgPath);
            }
            if (!Directory.Exists(Ugee.SignFingerImgPath))
            {
                Directory.CreateDirectory(Ugee.SignFingerImgPath);
            }
            if (!Directory.Exists(Ugee.MergePath))
            {
                Directory.CreateDirectory(Ugee.MergePath);
            }

            if (Screen.AllScreens.Count() > 1)
            {
                frmADV = new FrmADV();
                frmADV.Show();
            }
            ClearData();

            this.tlp.Controls.Add(plR1C1, 0, 0);
            plR1C1.BackColor = Color.FromArgb(214, 221, 227);
            plR1C1.Dock = DockStyle.Fill;

            this.tlp.Controls.Add(plR1C2, 1, 0);
            plR1C2.BackColor = Color.FromArgb(214, 221, 227);
            plR1C2.Dock = DockStyle.Fill;

            this.tlp.Controls.Add(plR2C1, 0, 1);
            plR2C1.BackColor = Color.FromArgb(81, 94, 111);
            plR2C1.Dock = DockStyle.Fill;

            this.tlp.Controls.Add(plR2C2, 1, 1);
            plR2C2.BackColor = Color.FromArgb(241, 245, 248);
            plR2C2.Dock = DockStyle.Fill;

            plCard.Dock = DockStyle.Fill;
            plCamera.Dock = DockStyle.Fill;
            plSign.Dock = DockStyle.Fill;
            plEvaluate.Dock = DockStyle.Fill;

            plCard.Visible = false;
            plCamera.Visible = true;
            plSign.Visible = false;
            plEvaluate.Visible = false;

            plCardTit.BackColor = Color.FromArgb(65, 78, 95);
            plCameraTit.BackColor = Color.FromArgb(56, 66, 75);
            plSignTit.BackColor = Color.FromArgb(56, 66, 75);
            plEvaluateTit.BackColor = Color.FromArgb(56, 66, 75);

            //Camera correlation
            FramePath = Application.StartupPath + "\\" + "frame.jpg";

            PhotoPath = Application.StartupPath + "\\" + "photo.jpg";

            LoadSignImgs();


            #region 签字相关
            plSignContent.BackColor = Color.FromArgb(121, 119, 120);
            #endregion
        }
        #endregion

        #region 选项卡切换
        //点击身份证
        private void lblCardTit_Click(object sender, EventArgs e)
        {
            plCard.Visible = true;
            plCamera.Visible = false;
            plSign.Visible = false;
            plEvaluate.Visible = false;

            plCardTit.BackColor = Color.FromArgb(107, 120, 138);
            plCameraTit.BackColor = Color.FromArgb(56, 66, 75);
            plSignTit.BackColor = Color.FromArgb(56, 66, 75);
            plEvaluateTit.BackColor = Color.FromArgb(56, 66, 75);
        }
        //摄像头拍照
        private void lblCameraTit_Click(object sender, EventArgs e)
        {
            plCard.Visible = false;
            plCamera.Visible = true;
            plSign.Visible = false;
            plEvaluate.Visible = false;

            plCardTit.BackColor = Color.FromArgb(56, 66, 75);
            plCameraTit.BackColor = Color.FromArgb(107, 120, 138);
            plSignTit.BackColor = Color.FromArgb(56, 66, 75);
            plEvaluateTit.BackColor = Color.FromArgb(56, 66, 75);

        }
        //pdf签字
        private void lblSignTit_Click(object sender, EventArgs e)
        {
            plCard.Visible = false;
            plCamera.Visible = false;
            plSign.Visible = true;
            plEvaluate.Visible = false;

            plCardTit.BackColor = Color.FromArgb(56, 66, 75);
            plCameraTit.BackColor = Color.FromArgb(56, 66, 75);
            plSignTit.BackColor = Color.FromArgb(107, 120, 138);
            plEvaluateTit.BackColor = Color.FromArgb(56, 66, 75);
        }
        //评价
        private void lblEvaluateTit_Click(object sender, EventArgs e)
        {
            plCard.Visible = false;
            plCamera.Visible = false;
            plSign.Visible = false;
            plEvaluate.Visible = true;

            plCardTit.BackColor = Color.FromArgb(56, 66, 75);
            plCameraTit.BackColor = Color.FromArgb(56, 66, 75);
            plSignTit.BackColor = Color.FromArgb(56, 66, 75);
            plEvaluateTit.BackColor = Color.FromArgb(107, 120, 138);
        }
        #endregion

        #region Signature related
        //End/cancel signature
        private void btnCancelSign_Click(object sender, EventArgs e)
        {
            if (Ugee.HasSign)
            {
                Ugee.HasSign = false;
                ReleaseDevice();
            }
            if (!(frmShow == null || frmShow.IsDisposed))
            {
                Thread.Sleep(300);//跟随签字框的延迟(500)
                frmShow.Close();
            }
            btnStartSign.Enabled = true;
            this.pbSignImg.BackgroundImage = global::HYSS001Demo.Properties.Resources.signDemo;
        }
        //Start signing
        private void btnStartSign_Click(object sender, EventArgs e)
        {
            if (Ugee.HasEvaluate)
            {
                MessageBox.Show("Please finish the satisfaction rating first", "Ugee HYSS001Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Ugee.HasSign)
            {
                MessageBox.Show("Please complete the signature operation first", "Ugee HYSS001Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!(frmEvaluate == null || frmEvaluate.IsDisposed))
            {
                UnCallback = new Ugee.UgeeUnregisterBtnCallBack_delegate(RESPONSE_MSG_FUNC);
                Ugee.UgeeUnregisterBtnCallBack(UnCallback);

                frmEvaluate.Close();
            }

            if (Screen.AllScreens.Count() > 1)//Presence secondary screen
            {
                //Turn on the device
                int ret = Ugee.UgeeOpenDevice();
                if (ret == 0)
                {
                    HWUGUtil.Ugee sound = new HWUGUtil.Ugee();
                    sound.SoundFileName = System.Windows.Forms.Application.StartupPath + "\\res\\tip_Sign.mp3";
                    sound.SoundPlay();

                    //Register callback functions
                    Callback = new Ugee.UgeeRegisterBtnCallBack_delegate(RESPONSE_MSG_FUNC);
                    Ugee.UgeeRegisterBtnCallBack(Callback);
                    GC.KeepAlive(Callback);

                    Ugee.HasSign = true;

                    if (frmShow == null || frmShow.IsDisposed)
                    {
                        frmShow = new FrmShow();
                        frmShow.Show();//Open secondary screen
                    }
                    else
                    {
                        frmShow.Activate();
                        //frmShow.StartSign();
                    }
                }
                else
                {
                    MessageBox.Show("Device failed to open", "Ugee HYSS001Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("No device detected", "Ugee HYSS001Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //File preview
        private void btnPreview_Click(object sender, EventArgs e)
        {
            Ugee.UgeeShowWindow(1);
           /* if (cbFileList.SelectedIndex > -1)
            {
                pbSignImg.BackgroundImage = Image.FromFile(alCompletedPdfList[cbFileList.SelectedIndex].ToString());
                btnStartSign.Enabled = false;
            }*/
        }
        //Signature fingerprint completed
        private void SignFingerOK()
        {
            HWUGUtil.Ugee ugee = new HWUGUtil.Ugee();
            ugee.HWUG_imageMergeByPos(System.Windows.Forms.Application.StartupPath + "\\res\\sign_bg.jpg", Ugee.SignFingerImgFullPath, Ugee.MergeFullPath, 280, 140, 464, 1195);
            pbSignImg.BackgroundImage = Image.FromFile(Ugee.MergeFullPath);

            ReleaseDevice();
            if (!(frmShow == null || frmShow.IsDisposed))
            {
                Thread.Sleep(300);//跟随签字框的延迟(500)
                frmShow.Close();
            }
            LoadSignImgs();
        }
        private void LoadSignImgs()
        {
            cbFileList.Items.Clear();
            alCompletedPdfList = new ArrayList();

            string[] files = Directory.GetFiles(Ugee.MergePath, "*" + Ugee.MergeFileType, SearchOption.TopDirectoryOnly);

            foreach (string fileName in files)
            {
                string name = Ugee.MergePath + "\\";
                string fname = fileName.Substring(name.Length);

                cbFileList.Items.Add(fname.Substring(0, fname.Length - 4));

                alCompletedPdfList.Add(fileName);
            }
            if (cbFileList.Items.Count > 0)
            {
                cbFileList.SelectedIndex = 0;
            }
        }
        //Release device
        [HandleProcessCorruptedStateExceptions]
        private void ReleaseDevice()
        {
            try
            {
                UnCallback = new Ugee.UgeeUnregisterBtnCallBack_delegate(RESPONSE_MSG_FUNC);
                Ugee.UgeeUnregisterBtnCallBack(UnCallback);

                Ugee.UgeeCloseWindow();
                Ugee.UgeeCloseDevice();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 摄像头相关
        //Turn on the camera
        [HandleProcessCorruptedStateExceptions]
        private void btnOpenCamera_Click(object sender, EventArgs e)
        {
            int ret = Ugee.UgeeOpenCamera();
            if (ret == 0)
            {
                CameraOpen = true;
                TimerCamera = new System.Timers.Timer(100);//实例化Timer类，设置间隔时间为100毫秒；
                TimerCamera.Elapsed += new System.Timers.ElapsedEventHandler(theout);//到达时间的时候执行事件；
                TimerCamera.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
                TimerCamera.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            }
            else
            {
                CameraOpen = false;
                MessageBox.Show("Failed to open the camera", "Ugee HYSS001Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        [HandleProcessCorruptedStateExceptions]
        public void theout(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Ugee.UgeeGetImgFrame("");

                if (File.Exists(FramePath))
                {
                    FileStream fileStream = new FileStream(FramePath, FileMode.Open, FileAccess.Read);
                    int byteLength = (int)fileStream.Length;
                    byte[] fileBytes = new byte[byteLength];
                    fileStream.Read(fileBytes, 0, byteLength);
                    //文件流关闭,文件解除锁定
                    fileStream.Close();
                    pbCamera.BackgroundImage = Image.FromStream(new MemoryStream(fileBytes));
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.ToString());
            }
        }
        //Get camera resolution
        [HandleProcessCorruptedStateExceptions]
        private void pbGetResolution_Click(object sender, EventArgs e)
        {
            if (CameraOpen)
            {
                string res = Marshal.PtrToStringAnsi(Ugee.UgeeGetResolution());
                string[] ResolutionList = res.Split('@');
                cbResolution.Items.Clear();
                foreach (var item in ResolutionList)
                {
                    cbResolution.Items.Add(item);
                }
                if (ResolutionList.Length > 0)
                {
                    cbResolution.SelectedIndex = 0;
                }
            }
        }
        //Set camera resolution
        [HandleProcessCorruptedStateExceptions]
        private void pbSetResolution_Click(object sender, EventArgs e)
        {
            if (CameraOpen)
            {
                if (cbResolution.Items.Count > 0)
                {
                    //"640*480@800*600@1024*768@1280*720@1600*1200@1920*1080@2048*1536@2560*1440@2592*1944";
                    Ugee.UgeeSetResolution(cbResolution.SelectedItem.ToString());
                }
                else
                {
                    Ugee.UgeeSetResolution("1920*1080");
                }
            }
        }
        //Mirror image
        [HandleProcessCorruptedStateExceptions]
        private void pbSetLRMirror_Click(object sender, EventArgs e)
        {
            try
            {
                if (CameraOpen)
                {
                    Ugee.UgeeSetLRMirror();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Take a picture
        [HandleProcessCorruptedStateExceptions]
        private void btnPhoto_Click(object sender, EventArgs e)
        {
            try
            {
                if (CameraOpen)
                {
                    Ugee.UgeeTakePhoto("");

                    if (File.Exists(PhotoPath))
                    {
                        FileStream fileStream = new FileStream(PhotoPath, FileMode.Open, FileAccess.Read);
                        int byteLength = (int)fileStream.Length;
                        byte[] fileBytes = new byte[byteLength];
                        fileStream.Read(fileBytes, 0, byteLength);
                        //文件流关闭,文件解除锁定
                        fileStream.Close();
                        pbPhoto.BackgroundImage = Image.FromStream(new MemoryStream(fileBytes));
                    }
                }
            }
            catch (Exception ex)
            {
                Ugee.WriteLogs("err4", ex.ToString());
            }
        }
        //Turn off the camera
        private void btnCloseCamera_Click(object sender, EventArgs e)
        {
            try
            {
                if (CameraOpen)
                {
                    CameraOpen = false;
                    Ugee.UgeeCloseCamera();
                    if (TimerCamera != null)
                    {
                        TimerCamera.Enabled = false;
                    }
                    pbCamera.BackgroundImage = HYSS001Demo.Properties.Resources.camera;
                    pbPhoto.BackgroundImage = HYSS001Demo.Properties.Resources.photo;
                    cbResolution.Items.Clear();
                }
            }
            catch (Exception ex)
            {
                Ugee.WriteLogs("err5", ex.ToString());
            }
        }
        #endregion

        #region 满意度评价
        //evaluate
        private void pbStartEvaluate_Click(object sender, EventArgs e)
        {
            try
            {
                if (Ugee.HasSign)
                {
                    MessageBox.Show("Please complete the signature operation first", "Ugee HYSS001Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!(frmEvaluate == null || frmEvaluate.IsDisposed))
                {
                    frmEvaluate.Close();
                }
                if (Screen.AllScreens.Count() > 1)
                {
                    int ret = Ugee.UgeeOpenDevice();
                    if (ret == 0)
                    {

                        HWUGUtil.Ugee sound = new HWUGUtil.Ugee();
                        sound.SoundFileName = System.Windows.Forms.Application.StartupPath + "\\res\\tip_Evaluate.mp3";
                        sound.SoundPlay();

                        CallbackEva = new Ugee.UgeeRegisterBtnCallBack_delegate(RESPONSE_MSG_FUNC);
                        Ugee.UgeeRegisterBtnCallBack(CallbackEva);
                        GC.KeepAlive(CallbackEva);

                        Ugee.HasEvaluate = true;
                        lblEvaluate.Visible = true;
                        lblEvaluate.Text = "Evaluation in progress";
                        frmEvaluate = new FrmEvaluate();
                        frmEvaluate.Show();

                        timer1.Start();
                        pbEvaluateStop.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Device failed to open", "Ugee HYSS001Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("No device detected", "Ugee HYSS001Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }
        //Evaluation completion
        private void EvaluateOK(int flag)
        {
            HWUGUtil.Ugee sound = new HWUGUtil.Ugee();
            sound.SoundFileName = System.Windows.Forms.Application.StartupPath + "\\res\\tip_Thanks.mp3";
            sound.SoundPlay();

            frmEvaluate.EvaluateOK(flag);
            switch (flag)
            {
                case 10:
                    lblEvaluate.Text = "Satisfaction rating: Very satisfied";
                    break;
                case 11:
                    lblEvaluate.Text = "Satisfaction rating: Satisfactory";
                    break;
                case 12:
                    lblEvaluate.Text = "Satisfaction rating: average";
                    break;
                case 13:
                    lblEvaluate.Text = "Satisfaction rating: Not satisfied";
                    break;
                case 14:
                    lblEvaluate.Text = "Satisfaction rating: Very satisfied";
                    break;
                case 15:
                    lblEvaluate.Text = "Satisfaction rating: Satisfactory";
                    break;
                case 16:
                    lblEvaluate.Text = "Satisfaction rating: average";
                    break;
                case 17:
                    lblEvaluate.Text = "Satisfaction rating: Not satisfied";
                    break;
                default:
                    break;
            }

            Ugee.HasEvaluate = false;
            ReleaseDevice();
            if (!(frmEvaluate == null || frmEvaluate.IsDisposed))
            {
                frmEvaluate.Close();
            }
        }
        //End evaluation
        private void pbEvaluateStop_Click(object sender, EventArgs e)
        {
            try
            {
                lblEvaluate.Visible = false;
                if (!(frmEvaluate == null || frmEvaluate.IsDisposed))
                {
                    frmEvaluate.Close();
                }

                if (Ugee.HasEvaluate)
                {
                    Ugee.HasEvaluate = false;
                    ReleaseDevice();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pbEvaluateStop.Enabled = true;
            timer1.Stop();

            pbStartEvaluate.Enabled = false;
            timer2.Start();
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            pbStartEvaluate.Enabled = true;
            timer2.Stop();
        }

        #endregion

        #region Identification card

        public static Boolean IsConnected = false;
        public static Boolean IsAuthenticate = false;
        public static Boolean IsRead_Content = false;
        public static int Port = 0;
        public static int ComPort = 0;
        public const int cbDataSize = 128;
        public const int GphotoSize = 256 * 1024;

        //获取身份证信息
        private void btnGetIDCard_Click(object sender, EventArgs e)
        {
            int AutoSearchReader = IDCard.InitCommExt();
            if (AutoSearchReader > 0)
            {
                HWUGUtil.Ugee sound = new HWUGUtil.Ugee();
                sound.SoundFileName = System.Windows.Forms.Application.StartupPath + "\\res\\IDCard.mp3";
                sound.SoundPlay();

                Thread thCard = new Thread(ThreadGetCard);
                thCard.Start();
            }
            else
            {
                MessageBox.Show("打开设备失败", "Ugee HYSS001Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ThreadGetCard()
        {
            try
            {
                int ret = 0;
                while (ret == 0)
                {
                    Thread.Sleep(10);
                    ret = GetCardInfo();
                    if (ret > 0)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Ugee.WriteLogs("err6", ex.ToString());
            }
        }
        //读取身份证信息
        private int GetCardInfo()
        {

            //Port = AutoSearchReader;
            //IsConnected = true;
            //lblName.Text = AutoSearchReader.ToString();

            StringBuilder sb = new StringBuilder(cbDataSize);
            IDCard.GetSAMID(sb);
            //MessageBox.Show("连接身份证阅读器成功,SAM模块编号:" + sb);

            //卡认证
            int FindCard = IDCard.Authenticate();
            //MessageBox.Show("卡认证" + FindCard);
            //if (FindCard != 1)
            //{
            //    ClearData();
            //    return;
            //}

            //读卡
            int rs = IDCard.Read_Content(1);
            //MessageBox.Show("Read_Content:" + rs);
            if (rs != 1 && rs != 2 && rs != 3)
            {
                ClearData();
                return 0;
            }

            //读卡成功
            //姓名
            //StringBuilder sb = new StringBuilder(cbDataSize);
            IDCard.getName(sb, cbDataSize);
            lblName.Text = sb.ToString();

            //民族/国家
            IDCard.getNation(sb, cbDataSize);
            lblNation.Text = sb.ToString();

            //性别 
            IDCard.getSex(sb, cbDataSize);
            lblSex.Text = sb.ToString();

            //出生 
            IDCard.getBirthdate(sb, cbDataSize);

            string birthdate = sb.ToString();
            DateTime dt = Convert.ToDateTime(birthdate.Substring(0, 4) + "-" + birthdate.Substring(4, 2) + "-" + birthdate.Substring(6, 2));
            lblBirthYear.Text = dt.Year.ToString();
            lblBirthMonth.Text = dt.Month.ToString();
            lblBirthDay.Text = dt.Day.ToString();

            //地址 
            IDCard.getAddress(sb, cbDataSize);
            string ad = sb.ToString();
            lblAdd.Text = ad;

            //号码 
            IDCard.getIDNum(sb, cbDataSize);
            lblIDNum.Text = sb.ToString();

            //机关 
            IDCard.getIssue(sb, cbDataSize);
            lblIssue.Text = sb.ToString();

            //有效期 
            IDCard.getEffectedDate(sb, cbDataSize);
            DateTime dt1 = Convert.ToDateTime(sb.ToString().Substring(0, 4) + "-" + sb.ToString().Substring(4, 2) + "-" + sb.ToString().Substring(6, 2));
            lblEffectedDate.Text = string.Format("{0:yyyy.MM.dd}", dt1);

            IDCard.getExpiredDate(sb, cbDataSize);
            DateTime dt2 = Convert.ToDateTime(sb.ToString().Substring(0, 4) + "-" + sb.ToString().Substring(4, 2) + "-" + sb.ToString().Substring(6, 2));
            lblExpiredDate.Text = string.Format("{0:yyyy.MM.dd}", dt2);

            //显示头像
            IDCard.GetBmpPhotoExt();
            int cbPhoto = 256 * 1024;
            StringBuilder sbPhoto = new StringBuilder(cbPhoto);
            int nRet = IDCard.getBMPPhotoBase64(sbPhoto, cbPhoto);
            byte[] byPhoto = Convert.FromBase64String(sbPhoto.ToString());

            if (nRet == 1)
            {
                MemoryStream ms = new MemoryStream(byPhoto);
                Image a = Image.FromStream(ms);

                Bitmap srcbmp1 = new Bitmap(a);

                a.Dispose();

                srcbmp1.MakeTransparent(Color.FromArgb(255, 255, 255));

                pbIDPhoto.BackgroundImage = srcbmp1;
            }

            int retc = IDCard.CloseComm();

            return 1;
        }

        public void ClearData()
        {
            lblName.Text = "";
            lblNation.Text = "";
            lblSex.Text = "";

            lblBirthYear.Text = "";
            lblBirthMonth.Text = "";
            lblBirthDay.Text = "";

            lblAdd.Text = "";
            lblIDNum.Text = "";

            lblIssue.Text = "";
            lblEffectedDate.Text = "";
            lblExpiredDate.Text = "";
            pbIDPhoto.BackgroundImage = null;
        }
        #endregion

        #region 响应函数
        private int RESPONSE_MSG_FUNC(int status)
        {
            try
            {
                if (status > 9 && status < 18)  //10-17
                {
                    EvaluateOK(status);
                }
                switch (status)
                {
                    case 0://Equipment available
                        break;
                    case -1://Device unavailable
                        ReleaseDevice();
                        if (!(frmShow == null || frmShow.IsDisposed))
                        {
                            frmShow.Close();
                        }
                        break;
                    case 1://Signature completed
                        Ugee.UgeeStartFinger(150, Ugee.FingerImgFullPath, Ugee.SignFingerImgFullPath);
                        break;
                    case 2://Cancel signature
                        ReleaseDevice();
                        if (!(frmShow == null || frmShow.IsDisposed))
                        {
                            Thread.Sleep(300);//跟随签字框的延迟(500)
                            frmShow.Close();
                        }
                        btnStartSign.Enabled = true;
                        this.pbSignImg.BackgroundImage = global::HYSS001Demo.Properties.Resources.signDemo;
                        Ugee.HasSign = false;
                        break;
                    case 3://Clear handwriting
                        break;
                    case 4://Click to confirm but no handwriting 
                        break;
                    case 5://End of fingerprint
                        SignFingerOK();
                        Ugee.HasSign = false;
                        break;
                    case 21:
                        frmShow.PrePage();
                        break;
                    case 22:
                        frmShow.NextPage();
                        break;
                    default:
                        break;
                }
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ugee HYSS001Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return -1;
            }
        }
        #endregion

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
           // Application.Exit();
            Ugee.StopProcess("HYSS001Demo");
            Ugee.StopProcess("FrmMain");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Ugee.UgeeShowWindow(0);
           /* if (frmShow != null )
            {
                frmShow.StartSignNew();
            }*/
        }

        private void pbSignImg_Click(object sender, EventArgs e)
        {

        }

        private void tlpCamera_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
