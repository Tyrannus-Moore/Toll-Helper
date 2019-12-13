using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TollAssistComm;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ID:");
            int id = int.Parse(Console.ReadLine());
            string path=Helper.GetProcessPath(id);
            Console.WriteLine("PATH={0}",path);
            Console.ReadLine();
        }
    }
}
