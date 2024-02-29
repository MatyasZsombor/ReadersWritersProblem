namespace ReadersWriters;

public class ThirdSolution() : ReadersWritersSolution(3)
{
    private readonly Semaphore _rMutex = new(1, NumThreads);
    private readonly Semaphore _wrMutex = new(1, 1);
    private readonly Semaphore _readTry = new(1, 1);
    private readonly Semaphore _resourceMutex = new(1, 1);
    private int _numOfWriters;
    private int _numOfReaders;

    protected override void ReadBalance()
    {
        //Enter Region
        _readTry.WaitOne();
        _rMutex.WaitOne();
        _numOfReaders++;

        if (_numOfReaders == 1)
        {
            _resourceMutex.WaitOne();
        }

        _rMutex.Release();
        _readTry.Release();
        
        //Critical Region
        ReadBalanceCriticalRegion();
        
        //Exit Region
        _rMutex.WaitOne();
        _numOfReaders--;
        
        if (_numOfReaders == 0)
        {
            _resourceMutex.Release();
        }
        _rMutex.Release();
    }

    protected override void Transfer()
    {
        //Enter Region
        _wrMutex.WaitOne();
        _numOfWriters++;
        if (_numOfWriters == 1)
        {
            _readTry.WaitOne();
        }

        _wrMutex.Release();
        _resourceMutex.WaitOne();

        //Critical Region
        TransferCriticalRegion();
        
        //Leave Region
        _resourceMutex.Release();
        _wrMutex.WaitOne();
        _numOfWriters--;

        if (_numOfWriters == 0)
        {
            _readTry.Release();
        }
        _wrMutex.Release();
    }
}