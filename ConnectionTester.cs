using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectionTester
{
    class ConnectionTester
    {
        private static readonly Uri Uri = new Uri("https://swapi.co/api/people/");
        private readonly HttpClient _client = new HttpClient();

        public async Task MakeClientCalls()
        {
            _client.Timeout = TimeSpan.FromSeconds(10);

            var tasks = new List<Task>();
            for (var i = 0; i < 50; i++)
            {
                tasks.Add(Task.Run(async () => await MakeCallWithHttpClient()));
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            await Task.WhenAll(tasks);

            stopWatch.Stop();
            Console.WriteLine("Elapsed: " + stopWatch.Elapsed);
        }

        private async Task MakeCallWithHttpClient()
        {
            Console.WriteLine($"Starting {Thread.CurrentThread.ManagedThreadId}");
            try
            {
                var result = await _client.GetStringAsync(Uri);
                //Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            Console.WriteLine($"Finished {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
