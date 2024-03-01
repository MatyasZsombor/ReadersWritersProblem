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
            BankAccounts.Add(new BankAccount(Threads[i].Name!, Random.NextDouble() * 10000));
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
                ReadBalance();
                continue;
            }
            Transfer();
        }
    }

    protected abstract void ReadBalance();
    protected abstract void Transfer();

    private void WriteStatistics()
    {
        string str = $"\nNumber of Transfers: {NumOfTransfers}\nNumber of Readings: {NumOfReadings}\n\n";
        if (Id == 2) { str += $"Number of Transfers timeouts: {NumOfTransferTimeouts}\nNumber of Reading Timeouts: {NumOfReaderTimeouts}\n\n"; }

        foreach (BankAccount bankAccount in BankAccounts)
        {
            str += $"History of Bank Account {bankAccount.AccountOwner}\n";
            str = bankAccount.TransferHistory.Aggregate(str, (current, transfer) => current + transfer + "\n");
            str += "\n";
        }
        File.AppendAllText($"../../../transferHistory{Id}.txt", str);
    }

    protected void ReadBalanceCriticalRegion()
    {
        Console.WriteLine($"Current Balance of {Thread.CurrentThread.Name} is {BankAccounts.Where(x => x.AccountOwner == Thread.CurrentThread.Name).ToArray()[0].CurBalance.ToString("C2", CultureInfo.CreateSpecificCulture("de-AT"))}");
        File.AppendAllText($"../../../transferHistory{Id}.txt", $"Current Balance of {Thread.CurrentThread.Name} is {BankAccounts.Where(x => x.AccountOwner == Thread.CurrentThread.Name).ToArray()[0].CurBalance.ToString("C2", CultureInfo.CreateSpecificCulture("de-AT"))}\n");
        NumOfReadings++;
    }

    protected void TransferCriticalRegion()
    {
        BankAccount curAccount = BankAccounts.Where(x => x.AccountOwner == Thread.CurrentThread.Name).ToArray()[0];
        BankAccount target = BankAccounts[Random.Next(0, 9)];
        double amount = Random.NextDouble() * (int)(curAccount.CurBalance - 1);
        
        while (target.AccountOwner == curAccount.AccountOwner)
        {
            target = BankAccounts[Random.Next(0, 9)];
        }
        
        Console.WriteLine($"Transferring {amount.ToString("C2", CultureInfo.CreateSpecificCulture("de-AT"))} from {curAccount.AccountOwner} to Thread {target.AccountOwner}.");
        File.AppendAllText($"../../../transferHistory{Id}.txt", $"Transferring {amount.ToString("C2", CultureInfo.CreateSpecificCulture("de-AT"))} from {curAccount.AccountOwner} to Thread {target.AccountOwner}.\n");
        curAccount.OutgoingTransfer(amount, target);
        
        Console.WriteLine("The transfer was successful.");
        File.AppendAllText($"../../../transferHistory{Id}.txt", "The transfer was successful.\n");

        NumOfTransfers++;
    }

    #region Variables
    private int Id { get; }
    private int NumOfTransfers { get; set; }
    private int NumOfReadings { get; set; }
    protected int NumOfReaderTimeouts { get; set; } = 0;
    protected int NumOfTransferTimeouts { get; set; } = 0;
    protected int NumIterations { get; } = 3;
    protected static int NumThreads { get; } = 10;
    private List<BankAccount> BankAccounts { get; }= [];
    protected static Random Random { get; } = new();
    private Thread[] Threads { get; } = new Thread[10];
    #endregion
}