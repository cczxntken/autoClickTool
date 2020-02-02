using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
namespace autosystool
{
    class Program
    {
        static Program proobj;
        static ToolObject toolObj;
        static Thread loopThread;
        static void Main(string[] args)
        {
            loopThread = new Thread(LoopFun);
            loopThread.IsBackground = true;
            toolObj = new ToolObject(args);

            loopThread.Start();
            while (true)
            {
                System.Threading.Thread.Sleep(10000);
            }
            //System.Windows.Input.ICommand.
        }

        static void LoopFun()
        {
           
            toolObj.Start();
            while (true)
            {
                toolObj.Update();
                System.Threading.Thread.Sleep(200);
            }
        }
    }
}
