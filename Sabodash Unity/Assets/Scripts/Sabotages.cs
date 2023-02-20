using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sabotages : MonoBehaviour
{
    // Start is called before the first frame update
    public static Player user;
    public static List<Player> playersCopy;
    public static int sabNumber;

    void Start()
    {
        playersCopy = new List<Player>(GameState.players);
    }

    public static void ApplySabotage(int n, Player player)
    {
        List<Player> Players_list = new List<Player>(FindObjectsOfType<Player>());
        Players_list.Remove(player);

        // Makes other players bigger
        if (n == 0 && player.bank >= 1)
        {
            player.bank -= 1;
            sabNumber = player.sabSelected;
            foreach (Player Player in Players_list)
            {
                Player.gameObject.transform.localScale += new Vector3(0.25f, 0.25f, 0f);
            }
        }

        // Makes other players gray
        else if (n == 1 && player.bank >= 1)
        {
            player.bank -= 1;
            sabNumber = player.sabSelected;
            foreach (Player Player in Players_list)
            {
                Player.sprite.color = Color.gray;
            }

        }

        // Reverse gravity
        else if (n == 2 && player.bank >= 1)
        {
            player.bank -= 1;
            sabNumber = player.sabSelected;
            foreach (Player Player in Players_list)
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
