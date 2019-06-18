using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    public abstract class BNpcBase : IBhvBranch
    {
        public void ChangeBranch(Npc a, IBhvBranch b)
        {
            a.ChangeBranch(b);
        }

        public virtual void Invoke(Npc a)
        {
            // Override in derived state
        }

        public void SetSpeed(Npc a, float nSpd)
        {
            if(a.NavAgent != null)
            {
                if (a.NavAgent.speed != nSpd)
                    a.NavAgent.speed = nSpd;
            }
        }

        public void SetAnimTrigger(Npc a, string nAnim)
        {
            if(a.Anim != null)
            {
                if (nAnim != null && nAnim.Length > 0)
                    a.Anim.SetTrigger(nAnim);
            }
        }

        public void SetAnimBool(Npc a, string nName, bool nValue)
        {
            if(a.Anim != null)
            {
                if (nName != null && nName.Length > 0)
                    a.Anim.SetBool(nName, nValue);
            }
        }

        public void CheckEnv(Npc a)
        {
            AreaScan(a);
            if (a.TrackList != null)
                ProcessTrackBook(a);
        }

        /*public void EntProxCheck(Npc a) // If the closestEnt is closer than proxMod percentage of the targetEnt distance, change target to closestEnt
        {
            targetEntDist = CheckDistSqr(GetTargetEntPos());

            if (targetEntDist <= distAware * distAware)
                canSee = true;
            else
                canSee = false;

            checkCanSee = canSee; // Debugging purposes only, remove when not needed

            if (closestEnt != null)
            {
                if (closestEntDist <= targetEntDist * proxMod * proxMod)
                {
                    RpcSetTargetEnt(closestEnt);
                    RpcClearPath();
                }
                else if (!gameMode) // False = Survive
                {
                    if (targetEnt == rallyPt)
                    {
                        RpcSetTargetEnt(closestEnt);
                        RpcClearPath();
                    }
                }
            }
        }*/

        public void AreaScan(Npc a) // Uses a Physics.OverlapSphere to scan the area up to this entity's Awareness Max Distance and log any GameObjects on Layers specified in the LayerMask
        {
            Collider[] localEnts = Physics.OverlapSphere(a.transform.position, a.Params.AwareMax, a.EntMask);

            if (localEnts != null && localEnts.Length > 0)
            {
                foreach (Collider col in localEnts)
                {
                    GameObject obj = col.transform.root.gameObject;
                    LiveEntity ent = obj.GetComponent<LiveEntity>();

                    if (ent != a)
                    {
                        if (ent != null)
                        {
                            if (!a.TrackList.Contains(ent))
                            {
                                Debug.Log("Found " + ent.gameObject.name);
                                a.TrackList.Add(ent);
                            }

                            /* string tempName = tempStats.GetName();
                            bool tempDead = tempStats.GetDead();
                            bool tempAsleep = tempStats.GetAsleep();

                            if (!tempDead && !tempAsleep)
                            {
                                if (tempName != entStats.GetName()) // If the detected entity is not one of the same species
                                {
                                    if (!trackBook.Contains(obj))
                                    {
                                        // Debug.Log(this.gameObject.name + " AIManager AreaScan(): Adding " + col.gameObject + " to the trackBook");
                                        trackBook.Add(obj);
                                    }
                                }
                                else // If the detected entity is one of the same species
                                {
                                    if (makePack)
                                    {
                                        if (!pack)
                                            SetPack(true);
                                    }

                                    if (gameMode)
                                        ShareState(obj); // Share State with other NPCs of the same type within range; only in Hunt mode
                                }
                            }
                            else
                            {
                                if (tempName == entStats.GetName())
                                {
                                    if (tempDead)
                                        SetStateAll(true, true, true);  // If we see a dead dinosaur, we flip out
                                }
                            }*/
                        }
                        else
                            Debug.LogWarning(a.gameObject.name + " couldn't find a LiveEntity script for a detected object (" + obj.name + "), can't add to trackList");
                    }
                }
            }
        }

        public void ProcessTrackBook(Npc a) // Process the entities in the trackBook, finds closest and removes ents beyond MAX_DIST 
        {
            bool resetTrackList = false;

            if (a.TrackList != null && a.TrackList.Count > 0)
            {
                float closestEntDist = 0.0f;
                LiveEntity closestEnt = null;
                bool fresh = true;

                for (int i = 0; i < a.TrackList.Count; i++)
                {
                    LiveEntity ent = a.TrackList[i];

                    if (ent != null) // If the entity dies and is cleaned up before the trackBook is processed, it will be null
                    {
                        float distCheck = Util.CheckDistSqr(a.transform.position, ent.transform.position);
                        // Stats checkStats = ent.transform.root.GetComponent<Stats>();

                        // Is it still within range
                        if (distCheck <= a.Params.AwareMax * a.Params.AwareMax)
                        {
                            /*if (checkStats != null)
                            {
                                if (!checkStats.GetDead() && !checkStats.GetAsleep())
                                {
                                    if (gameMode)
                                    {*/
                            if (fresh)
                            {
                                closestEntDist = distCheck;
                                closestEnt = ent;
                                fresh = false;
                            }
                            else
                            {
                                if (distCheck < closestEntDist)
                                {
                                    closestEntDist = distCheck;
                                    closestEnt = ent;
                                }
                            }
                            /*}
                            else    // In Survive mode...
                            {
                                if (checkStats.GetName() == Apex.PLAYER)    // ... Dinos only target players
                                {
                                    if (fresh)
                                    {
                                        closestEntDist = distCheck;
                                        closestEnt = ent.transform.root.gameObject;
                                        checkClosestEnt = ent.transform.root.gameObject; // Debugging purposes only, remove when not needed
                                        fresh = false;
                                    }
                                    else
                                    {
                                        if (distCheck < closestEntDist)
                                        {
                                            closestEntDist = distCheck;
                                            closestEnt = ent.transform.root.gameObject;
                                            checkClosestEnt = ent.transform.root.gameObject; // Debugging purposes only, remove when not needed
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            removeBook.Add(ent.transform.root.gameObject);
                            // Debug.Log(this.gameObject.name + " AIManager ProcessTrackBook(): Detected entity is dead");
                        }
                    }
                    else
                    {
                        removeBook.Add(ent.transform.root.gameObject);
                        Debug.LogWarning(this.gameObject.name + " AIManager ProcessTrackBook(): Error fetching the Stats script for a detected entity, can't process");
                    }
                */
                        }
                        else
                        {
                            // No longer within range, remove
                            a.RemoveList.Add(ent);
                            // Debug.Log(this.gameObject.name + " AIManager ProcessTrackBook(): " + ent.gameObject + " moved beyond MAX_DIST, adding to removeBook");
                        }
                    }
                    else
                    {
                        resetTrackList = true;
                        // Debug.Log(this.gameObject.name + " AIManager ProcessTrackBook(): Have an ent which seems to no longer exist, resetting trackBook after this go-round");
                    }
                }

                if (a.TargetObj == null)
                {
                    if (closestEnt != null)
                    {
                        a.SetTargetObj(closestEnt.gameObject);
                    }
                    // else
                    // Debug.Log(this.gameObject + " AIManager ProcessTrackBook(): closestEnt is null");
                }
            }
            else
            {
                a.SetTargetObj(null);
            }

            if (a.RemoveList != null && a.RemoveList.Count > 0)
            {
                for (int i = 0; i < a.RemoveList.Count; i++)
                {
                    LiveEntity ent = a.RemoveList[i];
                    if (ent != null) // If the entity dies and is cleaned up before the removeBook is processed, it will be null
                        Untrack(a, ent);
                }

                a.RemoveList.Clear();
            }

            if (resetTrackList)
            {
                a.TrackList.Clear();
            }
        }

        public void Untrack(Npc a, LiveEntity ent)
        {
            if (ent != null) // If the entity dies and is cleaned up before it can be untracked, it will be null
            {
                if (a.TrackList.Contains(ent))
                {
                    a.TrackList.Remove(ent);
                    Debug.Log(a.gameObject.name + " Untrack(): " + ent.gameObject.name + " removed from trackBook");
                }
            }
        }
    }
}
