using System.Globalization;

namespace ReadersWriters;

public abstract class ReadersWritersSolution
{
    protected ReadersWritersSolution(int id)
    {
        Id = id;
        
        using(FileStream fs = File.Open($"../../../transferHistory{id}.txt",FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            lock(fs)
            {
                fs.SetLength(0);
            }
        }

        for (int i = 0; i < NumThreads; i++)
        {
            Threads[i] = new Thread(ThreadProc)
            {
                Name = $"Thread{i + 1}"
            };
            TraficSensors.Add(new TraficSensor(Threads[i].Name!, Random.NextDouble() * 10000));
        }

        foreach (Thread thread in Threads)
        {
            thread.Start();
        }
        
        for (int i = 0; i < NumThreads; i++)
        {
            Threads[i].Join();
        }
        WriteStatistics();
    }

    private void ThreadProc()
    {
        for(int i = 0; i < NumIterations; i++)
        {
            if (Random.Next(2) == 0)
            {
                ReadData();
                continue;
            }
            TransferData();
        }
    }

    protected abstract void ReadData();
    protected abstract void TransferData();

    private void WriteStatistics()
    {
        string str = $"\nNumber of Transfers: {NumOfTransfers}\nNumber of Readings: {NumOfReadings}\n\n";
        if (Id == 2) { str += $"Number of Transfers timeouts: {NumOfTransferTimeouts}\nNumber of Reading Timeouts: {NumOfReaderTimeouts}\n\n"; }

        foreach (TraficSensor traficSensor in TraficSensors)
        {
            str += $"History of Sensor {traficSensor.SensorOwner}\n";
            str = traficSensor.SensorHistory.Aggregate(str, (current, transfer) => current + transfer + "\n");
            str += "\n";
        }
        File.AppendAllText($"../../../transferHistory{Id}.txt", str);
    }

    protected void ReadBalanceCriticalRegion()
    {
        Console.WriteLine($"Current Data of {Thread.CurrentThread.Name} is {TraficSensors.Where(x => x.SensorOwner == Thread.CurrentThread.Name).ToArray()[0].CurData.ToString(CultureInfo.CreateSpecificCulture("de-AT"))}");
        File.AppendAllText($"../../../transferHistory{Id}.txt", $"Current Data of {Thread.CurrentThread.Name} is {TraficSensors.Where(x => x.SensorOwner == Thread.CurrentThread.Name).ToArray()[0].CurData.ToString(CultureInfo.CreateSpecificCulture("de-AT"))}\n");
        NumOfReadings++;
    }

    protected void TransferCriticalRegion()
    {
        TraficSensor curAccount = TraficSensors.Where(x => x.SensorOwner == Thread.CurrentThread.Name).ToArray()[0];
        TraficSensor target = TraficSensors[Random.Next(0, 9)];
        double amount = Random.NextDouble() * (int)(curAccount.CurData - 1);
        
        while (target.SensorOwner == curAccount.SensorOwner)
        {
            target = TraficSensors[Random.Next(0, 9)];
        }
        
        Console.WriteLine($"Transferring {amount.ToString(CultureInfo.CreateSpecificCulture("de-AT"))} from {curAccount.SensorOwner} to Thread {target.SensorOwner}.");
        File.AppendAllText($"../../../transferHistory{Id}.txt", $"Transferring {amount.ToString(CultureInfo.CreateSpecificCulture("de-AT"))} from {curAccount.SensorOwner} to Thread {target.SensorOwner}.\n");
        curAccount.OutgoingData(amount, target);
        
        Console.WriteLine("The transfer was successful.");
        File.AppendAllText($"../../../transferHistory{Id}.txt", "The transfer was successful.\n");

        NumOfTransfers++;
    }

    #region Variables
    private int Id { get; }
    private int NumOfTransfers { get; set; }
    private int NumOfReadings { get; set; }
    protected int NumOfReaderTimeouts { get; set; }
    protected int NumOfTransferTimeouts { get; set; }
    private static int NumIterations => 3;
    protected static int NumThreads => 10;
    private List<TraficSensor> TraficSensors { get; }= [];
    private static Random Random { get; } = new();
    private Thread[] Threads { get; } = new Thread[10];
    #endregion
}