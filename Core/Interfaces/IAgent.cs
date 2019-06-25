using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
	/// <summary>
	/// Interface for an AI-driven agent 
	/// </summary>
	public interface IAgent
	{
		GameObject Target { get; set; }     // TargetEnt property; TargetEntPos (does this even need to be a property?)
		GameObject LastDmgObj { get; set; } // LastDmgEnt property, who hurt this agent last; including position and stats maybe
		bool CheckTarget();                 // Returns true if the target is viable
		bool CheckLastDmgEnt();             // Returns true if LastDmgEnt is viable
		Vector3 FindRoamPoint();
		/*void LookAtTarget(Vector3 targetPos);*/ // Behaviors??
		/*void LookAtDestination();*/       // Behaviors??
		void UpdateAnim();
		void SetDestination(Vector3 pos);
		/*void CalcApproach();*/            // Inside of Hunt() or Aggro() state?
		void CheckEnv();                    // Consolidate with AreaScan()?
		void EntProxCheck();                // If the closestEnt is closer than proxMod percentage of the targetEnt distance, change target to closestEnt
		void AreaScan();                    // Uses a Physics.OverlapSphere to scan the area up to this entity's Awareness Max Distance and log any GameObjects on Layers specified in the LayerMask
		void ProcessTrackBook();            // Process the entities in the trackBook, finds closest and removes ents beyond MAX_DIST 
		void TriggerSound(AudioClip nType, float nDelay);
	}
}
