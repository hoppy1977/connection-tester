using System;
using System.Net;

namespace ConnectionTester
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 10;

            var tester = new ConnectionTester();
            tester.MakeClientCalls().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
