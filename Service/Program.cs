using Config;
using Service.Publisher;

namespace Service;


class Program
{
    public static async Task<bool> RandomDelayAsync(Func<Task<bool>> func, TimeSpan interval, Task throttle)
    {
        var random = new Random();
        var shiftedInterval = interval.Add(TimeSpan.FromSeconds(-1));
        var randomValue = random.Next(1, shiftedInterval.Seconds);
        var randomInterval = TimeSpan.FromSeconds(randomValue);
        
        await Task.Delay(randomInterval);
        var result = await func();
        await throttle;
        return result;
    }
    
    public static async Task LoopIntervalAsync(Func<Task<bool>> func, TimeSpan interval, CancellationToken ctx)
    {
        while (true)
        {
            Console.WriteLine("Start loop at: {0:T}", DateTime.Now);
            Task throttle = Task.Run(async () => await Task.Delay(interval, ctx), ctx);
            var result = await RandomDelayAsync(func, interval, throttle);
            if (!result)
            {
                break;
            }
            Console.WriteLine("End loop at: {0:T}", DateTime.Now);
        }
    }

    
    static async Task Main(string[] args)
    {
        var configPath = @"c:\Users\acid3\dev\TelegramBot\Resources\config.json";
        var ctx = new CancellationToken();

        IBuilderConfig config = new BuilderConfig(configPath);
        var article = new Article(config, ctx);

        await LoopIntervalAsync(article.Release, TimeSpan.FromSeconds(config.LoopIntervalSeconds), ctx);

        Console.WriteLine("Press any key to exit...");
        Console.ReadLine();
    }
}
