using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iniloaf
{
    class Program
    {
        static string user, pass;
        static void Main(string[] args)
        {
            ReadIni();
            Console.WriteLine(user);
            Console.WriteLine(pass);
            Console.ReadLine();
        }

        static void ReadIni()
        {
            var ini = new ini("config.ini");
            string userP = ini.GetValue("steamLoginDetails", "steamuser");
            string passP = ini.GetValue("steamLoginDetails", "steampassword");
            user = userP;
            pass = passP;
        }
    }
}
