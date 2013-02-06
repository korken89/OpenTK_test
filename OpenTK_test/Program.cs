using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO.Ports;
using KFlySerialStateMachine;

namespace OpenTK_test
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            List<byte> test = new List<byte>();
            /*test.Add(0xA6);
            test.Add(0x19);
            test.Add(0x10);
            test.Add(0xAF);
            test.Add(0xC9);
            test.Add(0xCB);
            test.Add(0x4D);
            test.Add(0x3F);
            test.Add(0x51);
            test.Add(0x65);
            test.Add(0x80);
            test.Add(0x3D);
            test.Add(0x37);
            test.Add(0x8C);
            test.Add(0x82);
            test.Add(0x3E );
            test.Add(0x86);
            test.Add(0xCF);
            test.Add(0x07);
            test.Add(0x3F);
            test.Add(0xF3);
            test.Add(0x98);*/
            test.Add(0xA6);
            test.Add(0x19);
            test.Add(0x10);
            test.Add(0xAF);
            test.Add(0x60);
            test.Add(0x74);
            test.Add(0x6C);
            test.Add(0x3F);
            test.Add(0xA6);
            test.Add(0xA6);
            test.Add(0x96);
            test.Add(0x65);
            test.Add(0x3E);
            test.Add(0x09);
            test.Add(0x5C);
            test.Add(0xD7);
            test.Add(0x3D);
            test.Add(0x95);
            test.Add(0xC3);
            test.Add(0x92);
            test.Add(0x3E);
            test.Add(0x50);
            test.Add(0xE2); 

            StateMachine state = new StateMachine();

            //for (int i = 0; i < 10; i++)
               // foreach (byte b in test)
                    //state.SerialManager(b);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new OpenTK_test());
        }
    }
}
