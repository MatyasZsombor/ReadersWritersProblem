namespace ReadersWriters;

public sealed class SecondSolution() : ReadersWritersSolution(2)
{
   private static readonly ReaderWriterLock ReaderWriterLock = new();
   private const int TimeOut = 100;
   
   protected override void ReadBalance()
   {
      try 
      {
         ReaderWriterLock.AcquireReaderLock(TimeOut);
         try 
         {
            ReadBalanceCriticalRegion();
         }
         finally 
         {
            ReaderWriterLock.ReleaseReaderLock();
         }
      }
      catch (ApplicationException)
      {
         NumOfReaderTimeouts++;
      }
   }

   protected override void Transfer()
   {
      try 
      {
         ReaderWriterLock.AcquireWriterLock(TimeOut);
         try 
         {
            TransferCriticalRegion();
         }
         finally 
         {
            ReaderWriterLock.ReleaseWriterLock();
         }
      }
      catch (ApplicationException)
      {
         NumOfTransferTimeouts++;
      }
   }
   
}