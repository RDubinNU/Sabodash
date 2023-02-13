using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{



    public static bool gameStarted;

    // Start is called before the first frame update
    void Start()
    {
        gameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((CompareTag("LobbyOnly")) && gameStarted) {
            Destroy(this.gameObject);       
        }
    }
}
