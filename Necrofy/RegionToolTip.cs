using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Necrofy
{
    class RegionToolTip
    {
        private const int ToolTipVerticalOffset = 20;

        private readonly IClient client;
        private readonly ToolTip toolTip;
        private readonly Control control;

        private readonly Timer timer = new Timer();

        private Point mousePos;
        private string currentText;

        public RegionToolTip(IClient client, ToolTip toolTip, Control control) {
            this.client = client;
            this.toolTip = toolTip;
            this.control = control;
            
            control.MouseMove += Control_MouseMove;
            control.MouseLeave += Control_MouseLeave;
            timer.Tick += Timer_Tick;

            timer.Interval = toolTip.InitialDelay;
        }
        
        private void Control_MouseMove(object sender, MouseEventArgs e) {
            if (e.Location != mousePos) {
                mousePos = e.Location;
                string newText = client.GetToolTipAtPoint(mousePos);
                if (newText != currentText) {
                    timer.Stop();
                    timer.Start();
                    toolTip.Hide(control);
                }
            }
        }

        private void Control_MouseLeave(object sender, EventArgs e) {
            timer.Stop();
            toolTip.Hide(control);
        }

        private void Timer_Tick(object sender, EventArgs e) {
            timer.Stop();
            currentText = client.GetToolTipAtPoint(mousePos);
            toolTip.Show(currentText, control, mousePos.X, mousePos.Y + ToolTipVerticalOffset);
        }
        
        public interface IClient
        {
            string GetToolTipAtPoint(Point p);
        }
    }
}
