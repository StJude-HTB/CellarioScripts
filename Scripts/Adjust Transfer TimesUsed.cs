#region Header
// ** Copyright St Jude Children's Research Hospital 2019 **
// 
//    File:      SetTransferTimesUsed.cs 
//    Project:   StJudeLibrary
//    Solution:  CellarioWPF
#endregion

using System;
using System.Linq;
using HRB.Cellario.Scripting.API;

namespace StJude.Scripting
{
    public class SetTransferTimesUsed : AbstractScript
    {
    
        /// <summary>
        /// Changes the times used parameter on the transfer step to be the same as the number of plates in the other thread.
        /// Useful for Echo transfers from a single plate to a variable number of destinations.  When using virtual plates the
        /// physical plate is moved in and out of the Echo repeatedly.  This script will use a single labware (no virtual labware)
        /// and it stays in the Echo for the entire run. Script only runs on the first plate.
        /// </summary>        
        public override void Execute(IScriptingApi api)
        {
            // Only execute this script once -- when the first plate in the thread is introduced
        	if (api.CurrentPlate.PlateNumber != 1) return;
        	
        	// Get the number of plates in the other Thread.  Raise error if more than 2 threads exist.
        	var threads = api.CurrentPlate.CurrentProtocol.Threads;
        	// Throw an error if the thread count does not equal 2.
        	if (threads.Count() != 2)
        	{
        		try
        		{
        		 	throw new Exception("This protocol does not have two threads.  Only protocols with exactly two threads are supported.");
				}
				catch (Exception ex)
				{
					// Show exception to the user
					// Message severity is serious => the user may elect to shut down.
					api.Messaging.Notify("Unsupported Thread Count", null, ex, ScriptMessageSeverity.Serious);
				} 
			}

			int otherThreadPlateCount =  api.GetPlates().Count(p => p.CurrentThread.ThreadName != api.CurrentPlate.CurrentThread.ThreadName);

        	// Rebuild the plates array from the current thread
        	var plates = api.GetPlatesForCurrentThread();
        	var cumulativeUsage = 0;
        	
        	// Calculate the new timesused value
        	int newTimesUsed = Convert.ToInt16(Math.Ceiling( ((double) otherThreadPlateCount/(double) plates.Length) ));
            foreach (var plate in plates)
            {
            	
            	// Change the newTimesUsed value if the cumulative usage does not equal the number of plates in the other thread
            	if( (cumulativeUsage + newTimesUsed) > otherThreadPlateCount )
            	{
            		newTimesUsed = otherThreadPlateCount - cumulativeUsage;	
            	}
            	cumulativeUsage = cumulativeUsage + newTimesUsed;
            	
            	// Modify operation parameter TimesUsed in all transfer steps
                var transfer = plate.RemainingSteps.FirstOrDefault(s => s.StepName.Contains("Transfer"));
                if (transfer != null)
                {
                    transfer.TimesUsed = newTimesUsed;
                }  
        	}
        }
    }
}
