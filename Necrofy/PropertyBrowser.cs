﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Necrofy
{
    public partial class PropertyBrowser : DockContent
    {
        private static bool hexPointers = false;

        public event EventHandler<PropertyValueChangedEventArgs> PropertyChanged;

        public PropertyBrowser() {
            InitializeComponent();
            Disposed += PropertyBrowser_Disposed;

            SetHexPointers(Properties.Settings.Default.hexPointers);
            RefreshTimer(timerCancel.Token);
        }

        private void PropertyBrowser_Disposed(object sender, EventArgs e) {
            timerCancel.Cancel();
        }

        public void SetObjects(object[] objects) {
            propertyGrid.SelectedObjects = objects;
        }

        public void RefreshProperties() {
            refreshQueued = true;
        }

        private CancellationTokenSource timerCancel = new CancellationTokenSource();
        private bool refreshQueued = false;

        private async void RefreshTimer(CancellationToken cancellation) {
            while (!cancellation.IsCancellationRequested) {
                await Task.Delay(5);
                if (refreshQueued) {
                    propertyGrid.Refresh();
                    refreshQueued = false;
                }
            }
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
            PropertyChanged?.Invoke(this, e);
        }

        private void snesLoromPointers_Click(object sender, EventArgs e) {
            SetHexPointers(false);
        }

        private void hexadecimalPointers_Click(object sender, EventArgs e) {
            SetHexPointers(true);
        }

        private void SetHexPointers(bool value) {
            hexPointers = value;
            Properties.Settings.Default.hexPointers = value;
            Properties.Settings.Default.Save();

            snesLoromPointers.Checked = !hexPointers;
            hexadecimalPointers.Checked = hexPointers;
            RefreshProperties();
        }

        public static string PointerToString(int pointer) {
            if (hexPointers) {
                return $"0x{pointer:X}";
            } else {
                byte[] pointerBytes = new byte[4];
                pointerBytes.WritePointer(0, pointer);
                return $"${pointerBytes[2]:X2}:{pointerBytes[1]:X2}{pointerBytes[0]:X2}";
            }
        }
    }
}
