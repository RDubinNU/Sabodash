using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    // State Variables
    public static float gameSpeed = 1;
    public static bool gameStarted = false;

    // Player Tracking
    public static List<Player> players = new List<Player>();
    private static int nextPlayerID = 0;
    public static List<Color> possibleColours = new List<Color>();
    private static List<int> coloursInUse = new List<int>(); 

    // Camera
    private Transform cameraStartingPos;

    // Start is called before the first frame update
    void Start()
    {

        cameraStartingPos = FindObjectOfType<Camera>().transform;

        // Add possible colours

        List<Tuple<float, float>> sv = new List<Tuple<float, float>>();
        sv.Add(Tuple.Create(0.5f, 1f));
        sv.Add(Tuple.Create(1f, 1f));
        sv.Add(Tuple.Create(1f, 0.5f));

        foreach (Tuple<float, float> sv_pair in sv)
        {
            for (float i = 0; i < 1; i += 0.1f)
            {
                possibleColours.Add(Color.HSVToRGB(i, sv_pair.Item1, sv_pair.Item2));
            }
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if ((CompareTag("LobbyOnly")) && gameStarted)
        {
            Destroy(this.gameObject);
        }
    }

    static public void AddPlayer(Player player)
    {
        // Player ID Handling
        player.playerID = nextPlayerID;
        nextPlayerID++;

        players.Add(player);

    }

    static public void RemovePlayer(Player player)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == player)
            {
                players.RemoveAt(i);
            }
        }
    }

    static public void GetNextColour(Player player, int direction)
    {
        // Get next index
        int nextIndex = (player.colourIndex + direction) % possibleColours.Count;
        if (nextIndex < 0) nextIndex = possibleColours.Count - 1;

        for (int i = 0; i < coloursInUse.Count; i++)
        {
            if (coloursInUse[i] == player.colourIndex)
            {
                coloursInUse.RemoveAt(i);
                break;
            }
        }

        while (coloursInUse.Contains(nextIndex))
        {
            nextIndex = (nextIndex + direction) % possibleColours.Count;
            if (nextIndex < 0) nextIndex = possibleColours.Count - 1;
        }
        coloursInUse.Add(nextIndex);
        player.colourIndex = nextIndex;
    }
}
