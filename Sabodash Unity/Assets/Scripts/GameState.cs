using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    // State Variables
    public static float gameSpeed = 1;
    public static bool gameStarted = false;

    // Player Tracking
    private static List<Player> players = new List<Player>();
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
        possibleColours.Add(new Color(255, 0, 0));
        possibleColours.Add(new Color(0, 255, 0));
        possibleColours.Add(new Color(0, 0, 255));
        possibleColours.Add(new Color(255, 255, 255));
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
