#region Header
// ** Copyright St Jude Children's Research Hospital 2020 **
// 
//    Project:   St Jude ScriptLibrary
//    
#endregion

using System;
using System.Linq;
using HRB.Cellario.Scripting.API;

namespace StJude.Scripting
{
    public class ValidateEchoSourcePlate : AbstractScript
    {
        /// <summary>
        /// Look at the script name in the Echo transfer step for this thread and verify that the plate type matches.
        /// </summary>
        /// <remarks>This script will need to be edited if any Echo Source plate types are added or names changed</remarks>
        public override void Execute(IScriptingApi api)
        {
        	// Only execute this script once -- when the first plate in the thread is introduced
        	if (api.CurrentPlate.PlateNumber != 1) return;
        	
        	// Get Current Thread Plates
        	var plates = api.GetPlatesForCurrentThread();
            foreach (var plate in plates)
            {
            	// Get Steps remaining in thread
            	var steps = plate.RemainingSteps.ToList();
            	foreach (var step in steps)
            	{
            		// Check parameters for LIQUIDTRANSFER steps
            		if (step.StepName.ToUpper().Contains("LIQUIDTRANSFER"))
            		{
            			// Check Script Name matches Labware
            			// PP = LABCYTE_POLYPROPYLENE
	            		if (step.OperationParameters["Script Name"].ToString().Contains("PP") && !plate.Labware.Name.ToUpper().Contains("LABCYTE_POLYPROPYLENE"))
	            		{
	            			api.Messaging.WriteError(ScriptErrorSeverity.Error, 
	            				string.Format("Stopping system due to labware mismatch.  Echo script indicates PP source ({0}), but labware supplied is {1}.", 
	            								step.OperationParameters["Script Name"].ToString(), plate.Labware.Name) );
                        	api.System.Stop(); 
	            		}
	            		// LDV = LABCYTE_DIAMOND
	            		if (step.OperationParameters["Script Name"].ToString().Contains("LDV") && !plate.Labware.Name.ToUpper().Contains("LABCYTE_DIAMOND"))
	            		{
	            			api.Messaging.WriteError(ScriptErrorSeverity.Error, 
	            				string.Format("Stopping system due to labware mismatch.  Echo script indicates LDV source ({0}), but labware supplied is {1}.", 
	            								step.OperationParameters["Script Name"].ToString(), plate.Labware.Name) );
                        	api.System.Stop(); 
	            		}
	            	}
            	}
            }
         }   	

    }
}
