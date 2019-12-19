using System;
using System.Collections.Concurrent;
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

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        readonly List<Task> _runningTasks = new List<Task>();

        public async Task MakeClientCalls()
        {
            Console.WriteLine("Starting");

            _client.Timeout = TimeSpan.FromSeconds(10);
            var exceptions = new ConcurrentBag<Exception>();

            for (var i = 0; i < 100; i++)
            {
                _runningTasks.Add(Task.Run(async () => await MakeCallWithHttpClient(_cts.Token)));
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //await Task.WhenAll(tasks);

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 2,
                CancellationToken = _cts.Token,
            };

            //            _cts.CancelAfter(5000);
            
            var sw = new Stopwatch();
            sw.Start();
            Parallel.ForEach(_runningTasks, parallelOptions, (task, loopState) =>
            {
                
                if (loopState.IsStopped || loopState.IsExceptional)
                {
                    Console.WriteLine("XXX");
                    return;
                }
                try
                {
                    task.Wait();
                }

                // This should always be a Killer Exception, RunTask should only throw
                // Killer Exceptions
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    loopState.Stop();
                }
            });

            stopWatch.Stop();
            Console.WriteLine("Total Time Elapsed: " + stopWatch.Elapsed);
        }

        private async Task MakeCallWithHttpClient(CancellationToken ct)
        {
            Console.WriteLine($"Starting [{Thread.CurrentThread.ManagedThreadId}]");
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                var result = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, Uri), ct);
                //Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                stopWatch.Stop();
                Console.WriteLine($"ERROR [{Thread.CurrentThread.ManagedThreadId}]\t\tElapsed: {stopWatch.ElapsedMilliseconds}\t\t{ex.Message}");
                throw;
                //return;
            }

            stopWatch.Stop();
            Console.WriteLine($"Finished [{Thread.CurrentThread.ManagedThreadId}]\t\tElapsed: {stopWatch.ElapsedMilliseconds}");
        }
    }
}
