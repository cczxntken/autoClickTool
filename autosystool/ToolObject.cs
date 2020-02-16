using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Diagnostics;
namespace autosystool
{
    public class ToolObject
    {
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);


        [DllImport("user32.dll", EntryPoint = "mouse_event", SetLastError = true)]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        const int MOUSEEVENTF_MOVE = 0x0001;  // 移动鼠标
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;// 模拟鼠标左键按下
        const int MOUSEEVENTF_LEFTUP = 0x0004; //模拟鼠标左键抬起
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008; //模拟鼠标右键按下
        const int MOUSEEVENTF_RIGHTUP = 0x0010;// 模拟鼠标右键抬起
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //模拟鼠标中键按下
        const int MOUSEEVENTF_MIDDLEUP = 0x0040; //模拟鼠标中键抬起
        const int MOUSEEVENTF_ABSOLUTE = 0x8000; //标示是否采用绝对坐标
        public class QueObject
        {
            public string[] keys = { "", "8", "9" };
            public int ms = 10000;
            public int index = 0;
            public void Run()
            {
                Console.WriteLine("index = " + index);
                for (int i = 0; i < keys.Length; i++)
                {
                    KeyDown(keys[i]);
                    Console.WriteLine(keys[i]);
                    System.Threading.Thread.Sleep(ms);
                }
            }

            void KeyDown(string pKey)
            {
                switch (pKey)
                {
                    case "mr":
                        MouseDown(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP);
                        break;
                    case "ml":
                        MouseDown(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP);
                        break;
                    case "mm":
                        MouseDown(MOUSEEVENTF_MIDDLEDOWN | MOUSEEVENTF_MIDDLEUP);
                        break;
                    default:
                        SendKeys.SendWait(pKey);
                        break;
                }
            }

            void MouseDown(int pdwflag)
            {
                mouse_event(pdwflag, 50, 200, 0, 0);
            }
        }

        List<QueObject> queList = new List<QueObject>();
        int queCount = 1;
        Thread runingThread = null;
        string proName = "WowClassic";
        public ToolObject(string[] args)
        {
            Console.WriteLine("启动成功.");
            
            Console.WriteLine("输入配置加载方式:");
            Console.WriteLine("0.加载配置文件.");
            Console.WriteLine("1.自定义.");
            string ttypestr = Console.ReadLine();
            int ttype = 1;
            if (!int.TryParse(ttypestr, out ttype))
            {
                ttype = 1;
            }
            ttype = ttype > 1 ? 1 : ttype;

            switch (ttype)
            {
                case 0:
                      CreatQueByFile(); 
                    break;
                case 1:
                    CreatCustomQue();
                    break;
            }
        }

        public void Update()
        {
            if (pause)
            {
                return;
            }
            if (runingThread == null || !runingThread.IsAlive)
            {
                CreatThread();
            }
        }
        void CreatThread()
        {
            if (runingThread != null && runingThread.IsAlive) return;
            runingThread = new Thread(SendThread);
            runingThread.IsBackground = true;
            
            Console.WriteLine("5秒后启动按键循环.");
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine("-----------------------------------------------------------");

            runingThread.Start();
        }

        bool CreatQueByFile()
        {
            queList.Clear();
            startTag:
            Console.Write("输入配置文件名:");
            string tfilename = Console.ReadLine();
            string tfilepath = Application.StartupPath + "/" + tfilename;
            if (!File.Exists(tfilepath))
            {
                goto startTag;
            }
            byte[] tarrys = File.ReadAllBytes(tfilepath);

            StreamReader treader = new StreamReader(new MemoryStream(tarrys));

            string tmsstr =  treader.ReadLine();
            Console.WriteLine(string.Format("队列数 {0} ", tmsstr));
            if (!int.TryParse(tmsstr, out queCount))
            {
                queCount = 100;
            }
            queCount = System.Math.Min(100, queCount);

            for (int i = 0; i < queCount; i++)
            {
                QueObject tobj = new QueObject();
                tobj.index = i;
                
                string tms = treader.ReadLine();
                Console.WriteLine(string.Format("队列间隔 {0} ", tms));
                if (!int.TryParse(tms, out tobj.ms))
                {
                    tobj.ms = 1000;
                }
                tobj.ms = System.Math.Max(100, tobj.ms);

                string trdstrs = treader.ReadLine();
                Console.WriteLine(string.Format("队列按键组 {0} ", trdstrs));
                tobj.keys = trdstrs.Split(',');

                queList.Add(tobj);
                Console.WriteLine("-----------------------------------------------------------");
            }


            treader.Close();
            return true;
        }

        void CreatCustomQue()
        {
            queList.Clear();
            MemoryStream mem = new MemoryStream();
            StreamWriter twriter = new StreamWriter(mem);
            Console.WriteLine("输入队列数.上限100次");
            string tmsstr = Console.ReadLine();

            if (!int.TryParse(tmsstr, out queCount))
            {
                queCount = 100;
            }
            queCount = System.Math.Min(100, queCount);

            twriter.WriteLine(queCount);

            for (int i = 0; i < queCount; i++)
            {
                QueObject tobj = new QueObject();
                tobj.index = i;
                Console.WriteLine(string.Format("输入队列 {0} 间隔.", i));
                string tms = Console.ReadLine();
                if (!int.TryParse(tms, out tobj.ms))
                {
                    tobj.ms = 1000;
                }
                tobj.ms = System.Math.Max(100, tobj.ms);

                Console.WriteLine(string.Format("输入队列 {0} 按键组,以 , 号分隔.", i));
                string trdstrs = Console.ReadLine();
                tobj.keys = trdstrs.Split(',');

                queList.Add(tobj);

                twriter.WriteLine(tobj.ms);
                twriter.WriteLine(trdstrs);
                Console.WriteLine("-----------------------------------------------------------");
            }


            twriter.Flush();
            twriter.Close();
            byte[] tarys = mem.ToArray();
            Console.Write("是否保存为配置文件? y/n : ");
            string yorn = Console.ReadLine();
            switch (yorn)
            {
                case "y":
                    {
                        Console.Write("输入文件名: ");
                        string tfilename = Console.ReadLine();
                        string tfilepath = Application.StartupPath + "/" + tfilename;
                        Console.WriteLine(tfilepath);
                        File.WriteAllBytes(tfilepath, tarys);
                    }
                    break;
                case "n":
                    break;
            }

            mem.Close();
        }

        public void Pause()
        {
            pause = true;
        }
        bool pause = true;
        public void Start()
        {
            pause = false;
            CreatThread();
        }

        void SendThread()
        {
            while (true)
            {
                if (pause)
                {
                    System.Threading.Thread.Sleep(1000);
                    continue;
                }
                try
                {
                    Process[] tpros = RuningInstance(proName);
                    if( tpros.Length > 0)
                    {
                        foreach (var item in tpros)
                        {
                            HandleRunningInstance(item);
                            GoQue();
                        }
                        
                    }
                    System.Threading.Thread.Sleep(2000);
                }
                catch (Exception pE)
                {
                    Console.WriteLine(pE.ToString());
                    System.Threading.Thread.Sleep(2000);
                }

            }
        }

        void GoQue()
        {
            for (int i = 0; i < queCount; i++)
            {
                if (pause) break;
                queList[i].Run();
                Console.WriteLine("=======================================================");
            }
        }

        private static void HandleRunningInstance(Process pIns)
        {
            ShowWindowAsync(pIns.MainWindowHandle, 1);//显示
            SetForegroundWindow(pIns.MainWindowHandle);//当到最前端
        }

        private static Process[] RuningInstance(string pName)
        {
            Process[] Processes = Process.GetProcessesByName(pName);
            return Processes;
        }
    }
}
