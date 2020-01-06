using System;
using System.Net;

namespace ConnectionTester
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 1;

            var tester = new ConnectionTester();
            tester.MakeClientCalls().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
