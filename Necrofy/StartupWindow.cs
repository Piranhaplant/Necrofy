﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Necrofy
{
    partial class StartupWindow : EditorWindow
    {
        public StartupWindow(string statusText) {
            InitializeComponent();

            webBrowser.DocumentText = Properties.Resources.StartPage;
            Status = statusText;
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (e.Url.ToString().StartsWith("http")) {
                e.Cancel = true;
                Process.Start(e.Url.ToString());
            }
        }
    }
}
