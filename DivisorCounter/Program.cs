// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using Dapper;
using MySqlConnector;
using Polly;
using Polly.CircuitBreaker;
using RestSharp;

public class Program
{
    private static RestClient restClient = new RestClient("http://cache-service/");
    private static Policy retryPolicy = Policy.Handle<Exception>()
        .Retry(4, (exception,retryCount) =>
        {
            Console.WriteLine("Exception" + exception.Message);
            Console.WriteLine("Retry number: "+retryCount);
        });
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
            retryPolicy.Execute(()=> restClient.PostAsync(new RestRequest($"/cache?number={i}&divisorCounter={divisorCounter}")));
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
        int divisorCounter = retryPolicy.Execute(() =>
        {
            var task = restClient.GetAsync<int>(new RestRequest("/cache?number=" + number)).Result;
            return task;
        });

        if(divisorCounter != 0)
        {
            return divisorCounter;
        }
        for(var divisor =1;divisor <= number;divisor++)
        {
            if(number % divisor == 0)
            {
                divisorCounter++;
            }
        }
        return divisorCounter;
    }
}