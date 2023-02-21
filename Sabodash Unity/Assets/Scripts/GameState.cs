using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using Unity.VisualScripting;

public class GameState : MonoBehaviour
{
    // State Variables
    public static float gameSpeed = 1;
    public static bool gameStarted = false;

    // Player Tracking
    public static List<Player> alivePlayers = new List<Player>();
    public static List<Player> deadPlayers = new List<Player>();
    private static int nextPlayerID = 0;
    public static List<Color> possibleColours = new List<Color>();
    private static List<int> coloursInUse = new List<int>();

    // Camera
    private Camera mainCamera;
    private Vector3 cameraStartingPos;

    private bool resetting = false;
    private Vector3 cameraResetIncr;
    private const int cameraResetTime = 4;

    // Generator
    [SerializeField] private Generator generator;

    // Start is called before the first frame update
    void Start()
    {

        mainCamera = FindObjectOfType<Camera>();
        cameraStartingPos = mainCamera.transform.position;

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
    void LateUpdate()
    {

        if (!resetting) {

            checkForGameStart();
            releasePlayers();
            checkForReset();
            
        } else
        {
            checkResetDone();
        }

        

    }

    static public void AddPlayer(Player player)
    {
        // Player ID Handling
        player.playerID = nextPlayerID;
        nextPlayerID++;

        alivePlayers.Add(player);

    }

    static public void RemovePlayer(Player player)
    {
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            if (alivePlayers[i] == player)
            {
                alivePlayers.RemoveAt(i);
            }
        }

        deadPlayers.Add(player);
        resetPlayerToLobby(player);
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


    void CameraResetTick()
    {
        mainCamera.transform.position -= cameraResetIncr;
    }

    void checkForGameStart()
    {
        // Game Start Behaviour
        bool readyToStart = true;
        foreach (Player p in alivePlayers)
        {
            if (!p.ready)
            {
                readyToStart = false;
            }
        }

        if (readyToStart == true && alivePlayers.Count > 1)
        {
            gameStarted = true;
        }
    }
    

    void releasePlayers()
    {
        // Release players
        if ((CompareTag("LobbyOnly")) && gameStarted)
        {
            Destroy(this.gameObject);
        }
    }

    void checkForReset()
    {
        if (alivePlayers.Count <= 1 && gameStarted)
        {
            prepForReset();
        }
    }

    void prepForReset()
    {

        // Credit winning player (foreach in case of 0)
        foreach (Player p in alivePlayers)
        {
            p.playerWins += 1;
            resetPlayerToLobby(p);
            p.transform.position = p.spawnPoint;
        }

        // Trigger resetting state
        PlayerReset();
        resetting = true;
        cameraResetIncr = (mainCamera.transform.position - cameraStartingPos) / (50 * cameraResetTime);

    }

    void PlayerReset()
    {
        // Reset players
        gameStarted = false;
        foreach (Player p in deadPlayers)
        {
            alivePlayers.Add(p);
        }

        deadPlayers.Clear();

    }
    
   

    static void resetPlayerToLobby(Player player)
    {
        player.rigbod.position = player.spawnPoint;
        player.ready = false;
        player.bank = 0;
    }

    void ResetLevel()
    {
        // Reset level
        foreach (Transform tf in generator.renderedSections)
        {
            Destroy(tf.GameObject());
        }

        generator.renderedSections.Clear();

        // Update for new section
        generator.latestSectionEndPos = generator.Lobby.Find("SectionEnd").position;
        generator.SpawnLevelSection();
    }

    void checkResetDone()
    {
        // Do camera resetting
        CameraResetTick();
        // Ticked reset with tolerance of one incr
        if ((mainCamera.transform.position - cameraStartingPos).magnitude <= cameraResetIncr.magnitude)
        {
            mainCamera.transform.position = cameraStartingPos;
            resetting = false;

            // After camera do level
            ResetLevel();
        }
    }
}
