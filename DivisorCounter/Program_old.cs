//// See https://aka.ms/new-console-template for more information

//using System.Data;
//using System.Diagnostics;
//using System.Net;
//using Dapper;
//using MySqlConnector;
//using RestSharp;
//using Polly;
//using System.Numerics;
//using Polly.Retry;

//public class Program_old
//{
//    private static RestClient restClient = new RestClient("http://cache-service/");
//    //private static IDbConnection divisorCache = new MySqlConnection("Server=cache-db;Database=cache-database;Uid=div-cache;Pwd=C@ch3d1v;");

//    public static void Main()
//    {
//        var _retryPolicy = Policy
//        .Handle<HttpRequestException>()
//        .WaitAndRetryAsync(3, retryAttempt =>
//             TimeSpan.FromSeconds(3), onRetry: (exception, timeSpan, retryCount, context) =>
//             {
//                 Console.WriteLine($"Retry {retryCount}. Attempt failed, trying again...");
//             }
//        );

//        long first = 1_000_000_000;
//        long last = 1_000_000_020;

//        var numberWithMostDivisors = first;
//        var result = 0;

//        var watch = Stopwatch.StartNew();
//        for (var i = first; i <= last; i++)
//        {
//            var innerWatch = Stopwatch.StartNew();
//            var divisorCounter = CountDivisors(i);

//            // divisorCache.Execute("INSERT INTO counters (number, divisors) VALUES (@number, @divisors)", new { number = i, divisors = divisorCounter });
//            //var policy = Policy.Handle<Exception>()
//            //.Retry(3, (exception, retryCount) =>
//            //{
//            //    Console.WriteLine($"Retrying Post due to {exception.GetType().Name}... Attempt {retryCount}");
//            //});

//            //policy.Execute(async () =>
//            //{
//            _retryPolicy.ExecuteAsync(async () =>
//            {
//                Console.WriteLine("Executing Post operation i: " + i);
//                var response = await restClient.PostAsync(new RestRequest($"/cache?number={i}&divisorCounter={divisorCounter}"));
//                Console.WriteLine("Finished Post operation i: " + i);

//            });

//            //});

//            innerWatch.Stop();
//            Console.WriteLine("Counted " + divisorCounter + " divisors for " + i + " in " + innerWatch.ElapsedMilliseconds + "ms");

//            if (divisorCounter > result)
//            {
//                numberWithMostDivisors = i;
//                result = divisorCounter;
//            }
//        }
//        watch.Stop();

//        Console.WriteLine("The number with most divisors inside range is: " + numberWithMostDivisors + " with " + result + " divisors.");
//        Console.WriteLine("Elapsed time: " + watch.ElapsedMilliseconds + "ms");
//        Console.ReadLine();
//    }

//    private static int CountDivisors(long number)
//    {
//        var result = 0;
//        //var policy = Policy.Handle<Exception>()
//        //.CircuitBreaker(3, TimeSpan.FromSeconds(30),
//        //(exception, duration) =>
//        //{
//        //    Console.WriteLine("Circuit breaker tripped");


//        //},
//        //() => Console.WriteLine("Circuit breaker reset"));

//        //policy.Execute(() =>
//        //{
//        //    Console.WriteLine("Executing operation");
//        //    try
//        //    {
//        var task = restClient.GetAsync<int>(new RestRequest("/cache?number=" + number));
//        var divisorCounter = 0;
//        for (var divisor = 1; divisor <= number; divisor++)
//        {

//            if (number % divisor == 0)
//            {
//                divisorCounter++;
//            }
//        }
//        result = divisorCounter;
//        //    }
//        //    catch (Exception)
//        //    {

//        //        throw;
//        //    }
//        //    //calculation

//        //});
//        return result;
//        //    var policy = Policy.Handle<Exception>()
//        //.Retry(3, (exception, retryCount) =>
//        //{
//        //    Console.WriteLine($"Retrying due to {exception.GetType().Name}... Attempt {retryCount}");
//        //    Console.WriteLine("Calculating for: " + number);



//        //});

//        //    policy.Execute(() =>
//        //    {
//        //        Console.WriteLine("Executing Get operation");
//        //        //get
//        //        var task = restClient.GetAsync<int>(new RestRequest("/cache?number=" + number));
//        //        result = task.Result;
//        //        //calculation
//        //        var divisorCounter = 0;
//        //        if (result == 0)
//        //        {
//        //            for (var divisor = 1; divisor <= number; divisor++)
//        //            {
//        //                if (number % divisor == 0)
//        //                {
//        //                    divisorCounter++;
//        //                }
//        //            }
//        //            result = divisorCounter;
//        //        }
//        //    });
//        //    return result;
//        //   var policy = Policy.Handle<Exception>()
//        //.CircuitBreaker(3, TimeSpan.FromSeconds(30),
//        //    (exception, duration) =>
//        //    {
//        //        Console.WriteLine("Circuit breaker tripped");
//        //    },
//        //    () => Console.WriteLine("Circuit breaker reset"));

//        //   policy.Execute(() =>
//        //   {
//        //       try
//        //       {
//        //           Console.WriteLine("Executing operation");
//        //           throw new Exception();
//        //       }
//        //       catch (Exception ex)
//        //       {
//        //           // Re-throw the exception as an AggregateException
//        //           throw new AggregateException(ex);
//        //       }
//        //   });


//    }
//}