﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFTViewer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            FFTWLoader.Test();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormFourier());
        }
    }
}
