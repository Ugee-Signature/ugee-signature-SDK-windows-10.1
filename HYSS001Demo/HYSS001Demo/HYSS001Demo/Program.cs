﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HYSS001Demo
{
    static class Program
    {
        /// <summary>
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
