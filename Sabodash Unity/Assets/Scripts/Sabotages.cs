using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sabotages : MonoBehaviour
{
    // Start is called before the first frame update

    // Sabotage costs and durations

    public static int sabotageCount = 3;

    // Sabotage overlapping control
    private static bool[] overlappingAllowed = new bool[sabotageCount];
    private static bool[] sabotageInUse = new bool[sabotageCount];

    // Embiggening (0)
    private static float embiggenScale = 1.5f;
    private static float embiggenDuration = 5;
    private static int embiggenCost = 1;
    private static float embiggenCD = 5;

    // Greyscale (1)
    private static float greyscaleDur = 5;
    private static int greyScaleCost = 1;
    private static float greyscaleCD = 5;

    // Gravity (2)
    private static float gravityDur = 5f;
    private static int gravityCost = 1;
    private static float gravityCD = 5;


    static int[] sabotageCosts = {embiggenCost, greyScaleCost, gravityCost};
    public static float[] sabotageCDs = {embiggenCD, greyscaleCD, gravityCD};
    public static float[] sabotageDurs = { embiggenDuration, greyscaleDur, gravityDur};
    

    void Start()
    {
        // Set overlapping behaviour
        overlappingAllowed[0] = false;
        overlappingAllowed[1] = false;
        overlappingAllowed[2] = false;

    }

    public static bool ApplySabotage(int sabTriggered, Player callingPlayer)
    {

        

        if (callingPlayer.bank >= sabotageCosts[sabTriggered] && (overlappingAllowed[sabTriggered] || !sabotageInUse[sabTriggered]))
        {
            // Makes other players bigger 
            if (sabTriggered == 0)
            {
                callingPlayer.bank -= embiggenCost;
                foreach (Player p in GameState.players)
                {
                    if (p != callingPlayer)
                    {
                        p.gameObject.transform.localScale *= embiggenScale;
                    }
                }

            }

            // Makes other players gray
            else if (sabTriggered == 1)
            {
                callingPlayer.bank -= greyScaleCost;
                foreach (Player p in GameState.players)
                {
                    if (p != callingPlayer)
                        p.sprite.color = Color.gray;
                }
                sabotageInUse[sabTriggered] = true;
                return true;
            }

            // Reverse gravity
            else if (sabTriggered == 2)
            {
                callingPlayer.bank -= gravityCost;
                foreach (Player p in GameState.players)
                {
                    if (p != callingPlayer)
                    {
                        p.rigbod.gravityScale *= -1;
                    }
                }
                sabotageInUse[sabTriggered] = true;
                return true;
            }

            // Sabotage succesfully used
            sabotageInUse[sabTriggered] = true;
            return true;


        } else
        {
            // Sabotage failed to use
            return false;
        }
    }

    public static void ResetSabotage(int sabNumber, Player player)
    {
        // Reset only applied sabotage

        if(sabNumber == 0)
        {
            foreach (Player p in GameState.players)
            {
                if (p != player)
                {
                    p.gameObject.transform.localScale *= (1 / embiggenScale);
                }
            }
        }
        else if (sabNumber == 1)
        {
            foreach (Player p in GameState.players)
            {
                if (p != player)
                {
                    p.sprite.color = GameState.possibleColours[p.colourIndex];
                }
            }
        }
        else if (sabNumber == 2)
        {
            foreach (Player p in GameState.players)
            {
                if (p != player)
                {
                    p.rigbod.gravityScale *= -1;
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