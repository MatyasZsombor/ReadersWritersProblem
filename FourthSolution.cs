namespace ReadersWriters;

public class FourthSolution() : ReadersWritersSolution(4)
{
    private readonly Semaphore _rMutex = new(1, 1);
    private readonly Semaphore _resourceMutex = new(1, 1);
    private readonly FifoSemaphore _queue = new();
    private int _numOfReaders;
    
    protected override void ReadBalance()
    {
        //Enter Region
        _queue.WaitOne();
        _rMutex.WaitOne();
        _numOfReaders++;

        if (_numOfReaders == 1)
        {
            _resourceMutex.WaitOne();
        }
        _queue.Release();
        _rMutex.Release();
        
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
        _queue.WaitOne();
        _resourceMutex.WaitOne();
        _queue.Release();
        
        //Critical Region
        TransferCriticalRegion();
        
        //Leave Region
        _resourceMutex.Release();
    }
}