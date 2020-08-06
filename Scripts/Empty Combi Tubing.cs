#region Header
// ** Copyright St Jude Children's Research Hospital 2019 **
// 
//    File:      EmptyCombiTubing.cs 
//    Project:   StJudeLibrary
//    Solution:  CellarioWPF
#endregion

using System;
using System.Linq;
using HRB.Cellario.Scripting.API;

namespace StJude.Scripting
{
    public class EmptyCombiTubing : AbstractScript
    {
    	// Set up the Combi Resources
    	private readonly string[] ResourceNames = {"Combi 1", "Combi 2", "Combi 3"};

        /// <summary>
        /// Method called ahead of execution to optionally allocate resources.
        /// </summary>
        /// <param name="api">Use to claim device resources that will be used in the script.</param>
        public override void AllocateResources(IScriptingApiAllocation api)
        {
        	foreach (var ResourceName in ResourceNames)
            {
            	if ( api.CurrentPlate.PlateNumber != (api.GetPlatesForCurrentThread().Count()) ) return;
            	if ( api.Resources.ContainsKey(ResourceName) ) api.Resources[ResourceName].Allocate();
            }
        }

        /// <summary>
        /// Method called during Cellario protocol execution when a sample arrives at the scripting step.
        /// </summary>
        /// <remarks>Executes synchronously with the run scheduler. Device operation results not available
        /// until <see cref="ReleaseResources"/> method is called.</remarks>
        /// <param name="api">Access to properties and methods for interacting with the Cellario run-time scheduler, 
        /// samples, resources and devices operations.</param>
        public override void Execute(IScriptingApi api)
        {
            foreach (var ResourceName in ResourceNames)
            {
	            if ( api.CurrentPlate.PlateNumber != (api.GetPlatesForCurrentThread().Count()) ) return;
	            if ( !(api.Resources.ContainsKey(ResourceName)) ) return;  
	            
	            // Log the Empty Operation
	            api.Messaging.WriteDiagnostic(ScriptLogLevel.Normal, "Empty {0}.", ResourceName);   
	            
	            // Do the Empty Operation
	            var operation = api.Resources[ResourceName].Operations["Empty"];
				operation.OperationParameters["Volume"] = 1200;
				operation.Execute();

				 
			}				 
        }

        /// <summary>
        /// Method called during Cellario protocol execution when any device operations for the sample are complete,
        /// allowing access to their results.
        /// </summary>
        /// <remarks>If a resource is allocated but not released, it will remain unavailable to the Cellario run-time scheduler.</remarks>
        /// <param name="api">Access to properties and methods for interacting with the Cellario run-time scheduler, 
        /// samples, resources and device operations, including releasing allocated resources.</param>
        public override void ReleaseResources(IScriptingApiPostExecute api)
        {
            foreach (var ResourceName in ResourceNames)
            {
            	if ( api.CurrentPlate.PlateNumber != (api.GetPlatesForCurrentThread().Count()) ) return;
            	if ( api.Resources.ContainsKey(ResourceName) ) api.Resources[ResourceName].Release();
            }
        }
    }
}
