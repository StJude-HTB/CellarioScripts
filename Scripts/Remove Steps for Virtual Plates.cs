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
    public class RemoveStepsforEchoSources : AbstractScript
    {
        
        /// <summary>
        /// Removes the seal pierce or spin operations dynamically.  Useful for protocols that will utilize virtual plates and 
        /// it is undesirable for the same physical piece of labware to be sealed and peeled multiple times. 
        /// </summary>
        /// <remarks>Method called during Cellario protocol execution when a sample arrives at the scripting step.</remarks>
        /// <param name="api">Access to properties and methods for interacting with the Cellario run-time scheduler, 
        /// samples, resources and devices operations.</param>
        public override void Execute(IScriptingApi api)
        {
        	//only execute this script once -- when the first plate in the thread is introduced
        	if (api.CurrentPlate.PlateNumber != 1) return;
        	
            var plates = api.GetPlatesForCurrentThread();
            foreach (var plate in plates)
            {
                if (plates.Any(p => p.StartingLocation.ResourceName == plate.StartingLocation.ResourceName &&
                                    p.StartingLocation.Stack == plate.StartingLocation.Stack &&
                                    p.StartingLocation.Position == plate.StartingLocation.Position &&
                                    p.PlateNumber < plate.PlateNumber))
                {
                    RemoveStep(plate, "SPIN");
                    RemoveStep(plate, "PIERCE");
                }
                if (plates.Any(p => p.StartingLocation.ResourceName == plate.StartingLocation.ResourceName &&
                                    p.StartingLocation.Stack == plate.StartingLocation.Stack &&
                                    p.StartingLocation.Position == plate.StartingLocation.Position &&
                                    p.PlateNumber > plate.PlateNumber))
                    RemoveStep(plate, "SEAL");
            }
        }
        
        public void RemoveStep(IScriptingPlate plate, string stepName)
        {
            var ind = plate.RemainingSteps.ToList().FindIndex(x => x.StepName.ToUpper().Contains(stepName));
            var moveStep = plate.RemainingSteps.ElementAt(ind + 1);
            plate.RemainingSteps.Remove(moveStep);
            var step = plate.RemainingSteps.ElementAt(ind);
            plate.RemainingSteps.Remove(step);
        }
    }
}
