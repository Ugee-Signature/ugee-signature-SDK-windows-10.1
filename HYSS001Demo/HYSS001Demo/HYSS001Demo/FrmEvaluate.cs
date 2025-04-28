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
    public partial class FrmEvaluate : Form
    {
        public FrmEvaluate()
        {
            InitializeComponent();

            //防止闪烁
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        private void FrmEvaluate_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;

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

            pb_EV1.Location = new Point(260, 280);
            pb_EV2.Location = new Point(450, 280);
            pb_EV3.Location = new Point(640, 280);
            pb_EV4.Location = new Point(830, 280);

            pb_E1.Location = new Point(260, 490);
            pb_E2.Location = new Point(450, 490);
            pb_E3.Location = new Point(640, 490);
            pb_E4.Location = new Point(830, 490);

            pb_EV1.Tag = 10;
            pb_EV2.Tag = 11;
            pb_EV3.Tag = 12;
            pb_EV4.Tag = 13;

            pb_E1.Tag = 14;
            pb_E2.Tag = 15;
            pb_E3.Tag = 16;
            pb_E4.Tag = 17;

            Ugee.UgeeRegisterBtnPosInfo(BtnPostionInfo(pb_EV1) + BtnPostionInfo(pb_EV2) + BtnPostionInfo(pb_EV3) + BtnPostionInfo(pb_EV4) + BtnPostionInfo(pb_E1) + BtnPostionInfo(pb_E2) + BtnPostionInfo(pb_E3) + BtnPostionInfo(pb_E4));
        }
        /// <summary>
        /// 按钮位置信息
        /// </summary>
        /// <param name="btn"></param>
        /// <returns></returns>
        private string BtnPostionInfo(PictureBox btn)
        {
            return btn.Top + "@" + btn.Left + "@" + btn.Bottom + "@" + btn.Right + "@" + btn.Tag.ToString() + "@";
        }
        public void EvaluateOK(int value)
        {
            switch (value)
            {
                case 10:
                    pb_E1.BackgroundImage = HYSS001Demo.Properties.Resources.evaluate_AA;
                    break;
                case 11:
                    pb_E2.BackgroundImage = HYSS001Demo.Properties.Resources.evaluate_BB;
                    break;
                case 12:
                    pb_E3.BackgroundImage = HYSS001Demo.Properties.Resources.evaluate_CC;
                    break;
                case 13:
                    pb_E4.BackgroundImage = HYSS001Demo.Properties.Resources.evaluate_DD;
                    break;
                case 14:
                    pb_E1.BackgroundImage = HYSS001Demo.Properties.Resources.evaluate_AA;
                    break;
                case 15:
                    pb_E2.BackgroundImage = HYSS001Demo.Properties.Resources.evaluate_BB;
                    break;
                case 16:
                    pb_E3.BackgroundImage = HYSS001Demo.Properties.Resources.evaluate_CC;
                    break;
                case 17:
                    pb_E4.BackgroundImage = HYSS001Demo.Properties.Resources.evaluate_DD;
                    break;
                default:
                    break;
            }
        }
    }
}