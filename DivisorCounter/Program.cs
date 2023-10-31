// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Diagnostics;
using System.Net;
using Dapper;
using MySqlConnector;
using Polly;
using RestSharp;

public class Program
{
    private static RestClient restClient = new RestClient("http://cache-service/");
    //private static IDbConnection divisorCache = new MySqlConnection("Server=cache-db;Database=cache-database;Uid=div-cache;Pwd=C@ch3d1v;");
    
    public static void Main()
    {
        long first = 1_000_000_000;
        long last = 1_000_000_020;

        var numberWithMostDivisors = first;
        var result = 0;

        var watch = Stopwatch.StartNew();
        for (var i = first; i <= last; i++)
        {
            var innerWatch = Stopwatch.StartNew();
            var divisorCounter = CountDivisors(i);
            
            // divisorCache.Execute("INSERT INTO counters (number, divisors) VALUES (@number, @divisors)", new { number = i, divisors = divisorCounter });
            restClient.PostAsync(new RestRequest($"/cache?number={i}&divisorCounter={divisorCounter}"));

            innerWatch.Stop();
            Console.WriteLine("Counted " + divisorCounter + " divisors for " + i + " in " + innerWatch.ElapsedMilliseconds + "ms");

            if (divisorCounter > result)
            {
                numberWithMostDivisors = i;
                result = divisorCounter;
            }
        }
        watch.Stop();
        
        Console.WriteLine("The number with most divisors inside range is: " + numberWithMostDivisors + " with " + result + " divisors.");
        Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds + "ms");
        Console.ReadLine();
    }

    private static int CountDivisors(long number)
    {
        var divisorCounter = -1; //int.Parse(divisorResult);
        // var divisorCounter = divisorCache.QueryFirstOrDefault<int>("SELECT divisors FROM counters WHERE number = @number", new { number = i });
        var policy = Policy.Handle<Exception>()
        .CircuitBreaker(3, TimeSpan.FromSeconds(30),
        (exception, duration) =>
        {
            divisorCounter = 0;
            Console.WriteLine("Circuit breaker tripped");
            for (var divisor = 1; divisor <= number; divisor++)
            {
                //if (task?.Status == TaskStatus.RanToCompletion)
                //{
                //    var cachedResult = task.Result;
                //    if (cachedResult != 0)
                //    {
                //        return cachedResult;
                //    }

                //    task = null;
                //}

                if (number % divisor == 0)
                {
                    divisorCounter++;
                }
            }
            
        },
        () => Console.WriteLine("Circuit breaker reset"));

        policy.Execute(() =>
        {
            Console.WriteLine("Executing operation");
            var task = restClient.GetAsync<int>(new RestRequest("/cache?number=" + number));
            divisorCounter = task.Result;
        });

        //Thread.Sleep(9000);
        //var divisorResult = task.Content.ReadAsStringAsync().Result;


        //if (divisorCounter == 0)



        return divisorCounter;
    }
}