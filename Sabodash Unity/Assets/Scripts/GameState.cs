using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{

    public static float gameSpeed = 1;

    public static bool gameStarted;

    private Transform cameraStartingPos;

    // Start is called before the first frame update
    void Start()
    {
        gameStarted = false;
        cameraStartingPos = FindObjectOfType<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if ((CompareTag("LobbyOnly")) && gameStarted)
        {
            Destroy(this.gameObject);
        }
    }
}
