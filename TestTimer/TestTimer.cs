using System;
using System.Threading;
using ServerTests;
using FtpService;

class TimerExample
{
    //private static bool keepRunning = true;
    static async Task Main()
    {
        await Server.StartServerAsync();
        //var myTimer = new MyTimer();
        //myTimer.SetUpTimer(new TimeSpan(09, 46, 00));
        new TestTimer().Run();
        await Server.StopServerAsync();
        Console.WriteLine("exited");
    }
}

class TestTimer
{
    public bool isRunning = false;
    public void Run()
    {
        int i = 0;
        while (i < 5)
        {
            if (isRunning)
            {
                Console.WriteLine("waiting one to finish...");
            }
            else
            {
                Console.WriteLine("running...");
                Echo();
                i++;
            }
            Thread.Sleep(2000);
        }
    }

    public void Echo()
    {
        isRunning = true;
        Task.Delay(3000);
        isRunning = false;
    }
}

class MyTimer
{
    //private static Timer? timer;
    //private static int Rn = 0;
    //private static object _locker = new object();
    public bool isRunning = false;

    public void SetUpTimer(TimeSpan alertTime)
    {
        Run();

        DateTime current = DateTime.Now;
        TimeSpan timeToGo = alertTime - current.TimeOfDay;
        if (timeToGo < TimeSpan.Zero)
        {
            timeToGo += new TimeSpan(24, 0, 0);
        }

        _ = new Timer(x =>
        {
            Run();
        }, null, timeToGo, Timeout.InfiniteTimeSpan);
    }
    public void Run()
    {
        int i = 0;
        while (i < 3)
        {
            if (isRunning)
            {
                Console.WriteLine("waiting one to finish...");
            }
            else
            {
                Console.WriteLine("running...");
                Echo();
                i++;
            }
            Thread.Sleep(10000);
        }
    }

    public void Echo()
    {
        isRunning = true;
        var data = new Config
        {
            ServerAddress = "127.0.0.1",
            Port = 2222,
            Username = "user",
            Password = "12345",
            ServerRoot = "Upload",
            LocalRoot = @"D:\Data\local",
            HostName = "LocalTest",
            Paths = new List<PathMapping>
                {
                    new PathMapping{ src = "lamp", dst = "lamp_sample" },
                    new PathMapping{ src = "目录1", dst = "目录-1" },
                },
        };
        _ = MyClient.Upload(data);
        isRunning = false;
    }
}