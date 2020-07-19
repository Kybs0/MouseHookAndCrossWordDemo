using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace MouseHookDemo
{
    public class MouseHookManager
    {
        private IKeyboardMouseEvents globalMouseHook;
        private bool isMouseDown;
        private Point mouseSecondPoint;
        private Point mouseFirstPoint;

        public void Init()
        {
            globalMouseHook = Hook.GlobalEvents();
            globalMouseHook.MouseDoubleClick += async (o, args) => await this.MouseDoubleClicked(o, args);
            globalMouseHook.MouseDown += async (o, args) => await this.MouseDown(o, args);
            globalMouseHook.MouseUp += async (o, args) => await this.MouseUp(o, args);
        }

        private async Task MouseUp(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine($"MouseUp:{e.Location}");
            this.mouseSecondPoint = e.Location;

            if (this.isMouseDown && !this.mouseSecondPoint.Equals(this.mouseFirstPoint))
            {
                await Task.Run(() =>
                {
                    SendKeys.SendWait("^c");
                    //Debug.WriteLine($"MouseUp:SendKeys.SendC;");
                });
                this.isMouseDown = false;
            }
            this.isMouseDown = false;
        }

        private async Task MouseDown(object sender, MouseEventArgs e)
        {
            await Task.Run(() =>
            {
                this.mouseFirstPoint = e.Location;
                this.isMouseDown = true;
                //Debug.WriteLine($"MouseDown:{e.Location}");
            });
        }

        private async Task MouseDoubleClicked(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine($"MouseDoubleClicked");
            this.isMouseDown = false;
            await Task.Run(() =>
            {
                SendKeys.SendWait("^c");
                //Debug.WriteLine($"MouseDoubleClicked:SendKeys.SendC");
            });
        }
    }
}
