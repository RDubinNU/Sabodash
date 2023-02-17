using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideScroll : MonoBehaviour
{
    // Start is called before the first frame update
    private float scrollAmount = .05f;
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (GameState.gameStarted)
        {
            transform.position = new Vector3(transform.position.x + scrollAmount, transform.position.y, transform.position.z);

        }
    }


}
