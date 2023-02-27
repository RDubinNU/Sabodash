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
        public int CD;
        public bool overlappable;

        public Sabotage(String newname, int duration, int mycost, int mycd, bool overlap)
        {
            name = newname;
            dur = duration;
            cost = mycost;
            CD = mycd;
            overlappable = overlap;
        }
    }

    // Sabotage costs and durations

    public const int sabotageCount = 7;

    // Sabotage overlapping control
    private static bool[] sabotageInUse = new bool[sabotageCount];

    private static float embiggenScale = 1.5f;
    public static String[] namesList = new String[sabotageCount];

    //Sabotage(name, duration, cost, cd, overlappable)
    public static Sabotage[] sabVars = new Sabotage[sabotageCount]{
        new Sabotage("Big ", 5, 1, 5, false),
        new Sabotage("Grey", 5, 1, 5, false),
        new Sabotage("Grav", 5, 1, 5, false),
        new Sabotage("Ctrl", 5, 1, 5, false),
        new Sabotage("Bncy", 5, 1, 5, false),
        new Sabotage("Stop", 3, 1, 5, false),
        new Sabotage("Frwd", 3, 1, 5, false),
    };

    void Start()
    {
        for (int i = 0; i < sabotageCount; i++)
        {
            namesList[i] = sabVars[i].name;
        }
    }

    public static bool ApplySabotage(int sabTriggered, Player callingPlayer)
    {

        if (callingPlayer.bank >= sabVars[sabTriggered].cost && (sabVars[sabTriggered].overlappable || !sabotageInUse[sabTriggered]))
        {

            callingPlayer.bank -= sabVars[sabTriggered].cost;

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
                Vector3 temp = furthest.transform.position;
                furthest.transform.position = callingPlayer.transform.position;
                callingPlayer.transform.position = temp;
                furthest.outline.color = callingPlayer.sprite.color;
            }

            // Sabotage succesfully used
            sabotageInUse[sabTriggered] = true;
            return true;


        }
        else
        {
            // Sabotage failed to use
            return false;
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

        sabotageInUse[sabNumber] = false;

    }

    // Update is called once per frame
    void Update()
    {

    }
}