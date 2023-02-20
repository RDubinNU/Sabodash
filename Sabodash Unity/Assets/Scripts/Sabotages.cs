using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sabotages : MonoBehaviour
{
    // Start is called before the first frame update
    
    public static int sabNumber;

    void Start()
    {
        
    }

    public static void ApplySabotage(int n, Player player)
    {
        List<Player> playersCopy = new List<Player>(GameState.players);
        playersCopy.Remove(player);

        // Makes other players bigger
        if (n == 0 && player.bank >= 1)
        {
            player.bank -= 1;
            sabNumber = player.sabSelected;
            foreach (Player Player in playersCopy)
            {
                Player.gameObject.transform.localScale += new Vector3(0.25f, 0.25f, 0f);
            }
        }

        // Makes other players gray
        else if (n == 1 && player.bank >= 1)
        {
            player.bank -= 1;
            sabNumber = player.sabSelected;
            foreach (Player Player in playersCopy)
            {
                Player.sprite.color = Color.gray;
            }

        }

        // Reverse gravity
        else if (n == 2 && player.bank >= 1)
        {
            player.bank -= 1;
            sabNumber = player.sabSelected;
            foreach (Player Player in playersCopy)
            {
                Player.rigbod.gravityScale *= -1;
            }

        }
    }

    public static void ResetPlayers()
    {
        // Reset only applied sabotage

        if(sabNumber == 0)
        {
            foreach (Player player in GameState.players)
            {
                player.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
            }
        }
        else if (sabNumber == 1)
        {
            foreach (Player player in GameState.players)
            {
                player.sprite.color = player.color_copy;
            }
        }
        else if (sabNumber == 2)
        {
            foreach (Player player in GameState.players)
            {
                player.rigbod.gravityScale = 1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
