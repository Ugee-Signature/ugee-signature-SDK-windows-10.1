using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HYSS001Demo
{
    public partial class FrmADV : Form
    {
        Thread thADV;
        public FrmADV()
        {
            InitializeComponent();
            //防止闪烁
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        private void FrmADV_Load(object sender, EventArgs e)
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

            thADV = new Thread
                (
                    delegate ()
                    {
                        //3就是要循环轮数了
                        int i = 1;
                        while (true)
                        {
                            if (i > 8)
                            {
                                i = 1;
                            }
                            //调用方法

                            //设置图片的位置和显示时间（1000 为1秒）
                            ChangeImage(Image.FromFile(System.Windows.Forms.Application.StartupPath + "\\res\\" + i + ".jpg"), 2000);
                            i++;
                        }
                    }
                );
            thADV.IsBackground = true;
            thADV.Start();
        }
        /// <summary>
        /// 改变图片
        /// </summary>
        /// <param name="img">图片</param>
        /// <param name="millisecondsTimeOut">切换图片间隔时间</param>

        private void ChangeImage(Image img, int millisecondsTimeOut)
        {
            this.Invoke(new Action(() =>
            {
                if (Screen.AllScreens.Count() > 1)
                {
                    if (!this.Visible)
                    {
                        this.Show();
             
                        if (Screen.AllScreens[0] == Screen.PrimaryScreen)
                            this.Location = new Point(Screen.AllScreens[1].Bounds.Left, Screen.AllScreens[1].Bounds.Top);
                        else
                            this.Location = new Point(Screen.AllScreens[0].Bounds.Left, Screen.AllScreens[0].Bounds.Top);
                        for (int i = 0; i < Screen.AllScreens.Count(); i++)
                        {
                            if (Screen.AllScreens[i].Bounds.Height == 800 && Screen.AllScreens[i].Bounds.Width == 1280)
                                this.Location = new Point(Screen.AllScreens[i].Bounds.Left, Screen.AllScreens[i].Bounds.Top);
                        }
                    }
                    this.BackgroundImage = img;
                }
                else
                {
                    this.Hide();
                }
            })
                );
            Thread.Sleep(millisecondsTimeOut);
        }

        private void FrmADV_FormClosing(object sender, FormClosingEventArgs e)
        {
            thADV.Abort();
        }
    }
}