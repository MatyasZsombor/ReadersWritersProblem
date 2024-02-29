namespace ReadersWriters;

public class FifoSemaphore
{
    private readonly object _lockObj = new();

    private readonly List<Semaphore> _waitingQueue = [];


    private Semaphore RequestNewSemaphore()
    {
        lock (_lockObj)
        {
            Semaphore newSemaphore = new(1, 1);
            newSemaphore.WaitOne();
            return newSemaphore;
        }
    }
    
    public void Release()
    {
        lock (_lockObj)
        {
            _waitingQueue.RemoveAt(0);
            if (_waitingQueue.Count > 0)
            {
                _waitingQueue[0].Release();
            }
        }
    }

    public void WaitOne()
    {
        Semaphore semaphore = RequestNewSemaphore();
        lock (_lockObj)
        {
            _waitingQueue.Add(semaphore);
            semaphore.Release();

            if(_waitingQueue.Count > 1)
            {
                semaphore.WaitOne();
            }
        }
        semaphore.WaitOne();
    }
}