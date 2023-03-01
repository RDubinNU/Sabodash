using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sabotages : MonoBehaviour
{
    // Start is called before the first frame update

    public struct Sabotage
    {
        public String name;
        public int dur;
        public int cost;
        public bool overlappable;
        public int relProb;

        public Sabotage(String newname, int duration, int mycost, bool overlap, int relativeProb)
        {
            name = newname;
            dur = duration;
            cost = mycost;
            overlappable = overlap;
            relProb = relativeProb;
        }
    }

    // Sabotage overlapping control
    private static List<bool> sabotageInUse = new List<bool>();

    private static float embiggenScale = 1.5f;
    public static List<String> sabNamesList = new List<String>(); 

    // Sabotage costs and durations
    // Sabotage(name, duration, cost, cd, overlappable)
    public static List<Sabotage> sabVars = new List<Sabotage>();

    // Sabotage probabilites
    private static List<int> sabProbs = new List<int>();

    void Start()
    {

        sabVars.Add(new Sabotage("Big ", 5, 1, false, 1));
        sabVars.Add(new Sabotage("Grey", 5, 1, false, 1));
        sabVars.Add(new Sabotage("Grav", 5, 1, false, 1));
        sabVars.Add(new Sabotage("Ctrl", 5, 1, false, 1));
        sabVars.Add(new Sabotage("Bncy", 5, 1, false, 1));
        sabVars.Add(new Sabotage("Stop", 3, 1, false, 1));
        sabVars.Add(new Sabotage("Frwd", 3, 1, false, 1));

        // Initialize control lists
        for (int i = 0; i < sabVars.Count; i++)
        {
            // Add to names
            sabNamesList.Add(sabVars[i].name);

            // Usage
            sabotageInUse.Add(false);

            // Probabilities
            for (int j = 0; j < sabVars[i].relProb; j++)
            {
                sabProbs.Add(i);
            }
        }

    }

    public static int GrantSabotage()
    {
        int sabotageClaimedIndex = UnityEngine.Random.Range((int)0, (int)sabProbs.Count);
        return sabProbs[sabotageClaimedIndex];
    }

    public static void ApplySabotage(int sabTriggered, Player callingPlayer)
    {

        if ((sabVars[sabTriggered].overlappable || !sabotageInUse[sabTriggered]))
        {

            // Makes other players bigger and slower
            if (sabTriggered == 0)
            {
                foreach (Player p in GameState.alivePlayers)
                {
                    if (p != callingPlayer)
                    {
                        p.gameObject.transform.localScale *= embiggenScale;
                        p.sab_vel_percent = 0.5f;
                        p.outline.color = callingPlayer.sprite.color;
                    }
                }
            }
            // Makes other players gray
            else if (sabTriggered == 1)
            {
                foreach (Player p in GameState.alivePlayers)
                {
                    if (p != callingPlayer)
                    {
                        p.sprite.color = Color.gray;
                        p.outline.color = callingPlayer.sprite.color;
                    }
                }
            }
            // Reverse gravity
            else if (sabTriggered == 2)
            {
                foreach (Player p in GameState.alivePlayers)
                {
                    if (p != callingPlayer)
                    {
                        p.rigbod.gravityScale *= -1;
                        p.outline.color = callingPlayer.sprite.color;
                    }
                }
            }
            // Reverse controls
            else if (sabTriggered == 3)
            {
                foreach (Player p in GameState.alivePlayers)
                {
                    if (p != callingPlayer)
                    {
                        p.directionScale *= -1;
                        p.outline.color = callingPlayer.sprite.color;
                    }
                }
            }
            // Bouncy
            else if (sabTriggered == 4)
            {
                foreach (Player p in GameState.alivePlayers)
                {
                    if (p != callingPlayer)
                    {
                        p.boxcollider.sharedMaterial = p.mat_bouncy;
                        p.outline.color = callingPlayer.sprite.color;
                    }
                }
            }

            // Stop
            else if (sabTriggered == 5)
            {
                foreach (Player p in GameState.alivePlayers)
                {
                    if (p != callingPlayer)
                    {
                        p.sab_vel_percent = 0f;
                        p.outline.color = callingPlayer.sprite.color;
                    }
                }
            }
            else if (sabTriggered == 6)
            {
                Player furthest = callingPlayer;
                foreach (Player p in GameState.alivePlayers)
                {
                    if (p.transform.position.x > furthest.transform.position.x) furthest = p;
                }
                callingPlayer.transform.position = furthest.transform.position;
            }


            // Sabotage succesfully used
            sabotageInUse[sabTriggered] = true;
        }

        else
        {
            // Sabotage failed to use
        }
    }

    public static void ResetSabotage(int sabNumber, Player player)
    {
        // Reset only applied sabotage

        if (sabNumber == 0)
        {
            foreach (Player p in GameState.alivePlayers)
            {
                if (p != player)
                {
                    p.gameObject.transform.localScale *= (1 / embiggenScale);
                    p.sab_vel_percent = 1f;
                    p.outline.color = Color.black;
                }
            }
        }
        else if (sabNumber == 1)
        {
            foreach (Player p in GameState.alivePlayers)
            {
                if (p != player)
                {
                    p.sprite.color = GameState.possibleColours[p.colourIndex];
                    p.outline.color = Color.black;
                }
            }
        }
        else if (sabNumber == 2)
        {
            foreach (Player p in GameState.alivePlayers)
            {
                if (p != player)
                {
                    p.rigbod.gravityScale *= -1;
                    p.outline.color = Color.black;
                }
            }
        }
        else if (sabNumber == 3)
        {
            foreach (Player p in GameState.alivePlayers)
            {
                if (p != player)
                {
                    p.directionScale *= -1;
                    p.outline.color = Color.black;
                }
            }
        }

        else if (sabNumber == 4)
        {
            foreach (Player p in GameState.alivePlayers)
            {
                if (p != player)
                {
                    p.boxcollider.sharedMaterial = p.mat_normal;
                    p.outline.color = Color.black;
                }
            }
        }

        else if (sabNumber == 5)
        {
            foreach (Player p in GameState.alivePlayers)
            {
                if (p != player)
                {
                    p.sab_vel_percent = 1f;
                    p.outline.color = Color.black;
                }
            }
        }
        else if (sabNumber == 6)
        {
            foreach (Player p in GameState.alivePlayers)
            {
                if (p != player)
                {
                    p.outline.color = Color.black;
                }
            }
        }

        else if (sabNumber == 7)
        {
            foreach (Player p in GameState.alivePlayers)
            {
                if (p != player)
                {
                    // Reset handled by sabotage ticking
                    p.outline.color = Color.black;
                }
            }
        }

        sabotageInUse[sabNumber] = false;

    }

    // Update is called once per frame
    void Update()
    {

    }
}