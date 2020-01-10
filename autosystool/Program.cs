using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace autosystool
{
    class Program
    {
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
                    SendKeys.SendWait(keys[i]);
                    Console.WriteLine(keys[i]);
                    System.Threading.Thread.Sleep(ms);
                }
            }
        }

        static List<QueObject> queList = new List<QueObject>();
        static int queCount = 1;
        static void Main(string[] args)
        {

            Console.WriteLine("启动成功.");

            Console.WriteLine("输入队列数.上限100次");
            string tmsstr = Console.ReadLine();

            if (!int.TryParse(tmsstr, out queCount))
            {
                queCount = 100;
            }
            queCount = System.Math.Min(100, queCount);

            for (int i = 0; i < queCount; i++)
            {
                QueObject tobj = new QueObject();
                tobj.index = i;
                Console.WriteLine(string.Format("输入队列 {0} 间隔.",i));
                string tms = Console.ReadLine();
                if (!int.TryParse(tms, out tobj.ms))
                {
                    tobj.ms = 1000;
                }
                tobj.ms = System.Math.Max(1000, tobj.ms);

                Console.WriteLine(string.Format("输入队列 {0} 按键组,以 , 号分隔.", i));
                string trdstrs = Console.ReadLine();
                tobj.keys = trdstrs.Split(',');

                queList.Add(tobj);
            }
           

            Console.WriteLine("5S后启动按键循环.");
            System.Threading.Thread.Sleep(5000);

            Thread t1 = new Thread(SendThread);
            t1.IsBackground = true;
            
            while (true)
            {
                if(!t1.IsAlive)
                {
                    t1.Start();
                }
                System.Threading.Thread.Sleep(1000);
            }    
            
            //System.Windows.Input.ICommand.
        }

        static void SendThread()
        {
            while (true)
            {
                try
                {
                    for (int i = 0; i < queCount; i++)
                    {
                        queList[i].Run();
                    }
                }
                catch (Exception pE)
                {
                    Console.WriteLine(pE.ToString());
                }

            }
        }
    }
}
