using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HYSS001Demo
{
    public partial class FrmShow : Form
    {
        int ScrollStep = 300;

        int CurScroll = 0;

        int ScrollMin = 0;

        int ScrollMax = 0;

        public FrmShow()
        {
            //防止闪烁
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲

            InitializeComponent();
        }
        private void FrmShow_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;

            this.FormBorderStyle = 0;

            this.ShowInTaskbar = false;///使窗体不显示在任务栏

            if (Screen.AllScreens[0] == Screen.PrimaryScreen)
            {
                this.Size = new System.Drawing.Size(Screen.AllScreens[1].Bounds.Width, Screen.AllScreens[1].Bounds.Height);

                this.Location = new Point(Screen.AllScreens[1].Bounds.Left, Screen.AllScreens[1].Bounds.Top);
            }
            else
            {
                this.Size = new System.Drawing.Size(Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height);

                this.Location = new Point(Screen.AllScreens[0].Bounds.Left, Screen.AllScreens[0].Bounds.Top);
            }

            for (int i = 0; i < Screen.AllScreens.Count(); i++)
            {
                if (Screen.AllScreens[i].Bounds.Height == 800 && Screen.AllScreens[i].Bounds.Width == 1280)
                {
                    this.Size = new System.Drawing.Size(Screen.AllScreens[i].Bounds.Width, Screen.AllScreens[i].Bounds.Height);
                    this.Location = new Point(Screen.AllScreens[i].Bounds.Left, Screen.AllScreens[i].Bounds.Top);
                }
            }

            Ugee.GetScrollRange(panel1.Handle, 1, out ScrollMin, out ScrollMax);

            ScrollMax = panel1.DisplayRectangle.Height;

            //设置隐藏按钮ID
            btnPrePage.Tag = 21;
            //设置隐藏按钮ID
            btnNextPage.Tag = 22;

            //StartSign();
        }
        public void StartSign()
        {
            try
            {
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");

                Ugee.SignImgFullPath = Ugee.SignImgPath + "\\" + fileName + ".png";

                Ugee.FingerImgFullPath = Ugee.FingerImgPath + "\\" + fileName + ".png";

                Ugee.SignFingerImgFullPath = Ugee.SignFingerImgPath + "\\" + fileName + ".png";

                Ugee.MergeFullPath = Ugee.MergePath + "\\" + fileName + ".png";

                Ugee.UgeeRegisterBtnPosInfo(BtnPostionInfo(btnPrePage) + BtnPostionInfo(btnNextPage));

                int ret = Ugee.UgeeStartSign(820, 280, 420, 210, 8, Ugee.SignImgFullPath);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "HWUGMultiFunPdf", MessageBoxButtons.OK, MessageBoxIcon.Information);
                throw;
            }
        }
        public void StartSignNew()
        {
            try
            {

                int ret = Ugee.UgeeStartSign(820, 280, 420, 210, 4, Ugee.SignImgFullPath);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "HWUGMultiFunPdf", MessageBoxButtons.OK, MessageBoxIcon.Information);
                throw;
            }
        }
        /// <summary>
        /// 按钮位置信息
        /// </summary>
        /// <param name="btn"></param>
        /// <returns></returns>
        private string BtnPostionInfo(Button btn)
        {
            return btn.Top + "@" + btn.Left + "@" + btn.Bottom + "@" + btn.Right + "@" + btn.Tag.ToString() + "@";
        }
        //上翻
        public void PrePage()
        {
            if (CurScroll > 0)
            {
                CurScroll -= ScrollStep;

                Ugee.SetScrollPos(panel1.Handle, 1, CurScroll, true);

                Ugee.PostMessage(panel1.Handle, Ugee.WM_VSCROLL, Ugee.SB_THUMBPOSITION, 0);
            }
        }
        //下翻
        public void NextPage()
        {
            if (CurScroll <= ScrollMax)
            {
                CurScroll += ScrollStep;

                Ugee.SetScrollPos(panel1.Handle, 1, CurScroll, true);

                Ugee.PostMessage(panel1.Handle, Ugee.WM_VSCROLL, Ugee.SB_THUMBPOSITION, 0);
            }
        }
        private void btnNextPage_Click(object sender, EventArgs e)
        {
            NextPage();
        }

        private void btnPrePage_Click(object sender, EventArgs e)
        {
            PrePage();
        }
        public void SignOK()
        {
            pbSignImg.BackgroundImage = Image.FromFile(Ugee.MergeFullPath);
        }

        private void FrmShow_Activated(object sender, EventArgs e)
        {
            StartSign();

            //if (!Ugee.HasSign)
            //{
            //    Ugee.HasSign = true;
            //    pbSignImg.BackgroundImage = null;
            //    StartSign();
            //}
        }
    }
}