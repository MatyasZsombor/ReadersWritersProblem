namespace ReadersWriters;

public sealed class FirstSolution() : ReadersWritersSolution(1)
{
    private readonly Semaphore _mutex = new(1, 1);
    private readonly Semaphore _wrt = new(1, 1);
    private static int _numOfReaders;

    protected override void ReadData()
    {
        //Enter Region
        _mutex.WaitOne();
        _numOfReaders++;
        if (_numOfReaders == 1)
        {
            _wrt.WaitOne();
        }
        _mutex.Release();
        
        //Critical Region
        ReadBalanceCriticalRegion();
        
        //Exit Region
        _mutex.WaitOne();
        _numOfReaders--;
        
        if (_numOfReaders == 0)
        {
            _wrt.Release();
        }
        
        _mutex.Release();
    }

    protected override void TransferData()
    {
        //Enter Region
        _wrt.WaitOne();
        
        //Critical Region
        TransferCriticalRegion();
        
        //Leave Region
        _wrt.Release();
    }
}