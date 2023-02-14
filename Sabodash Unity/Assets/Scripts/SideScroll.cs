using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideScroll : MonoBehaviour
{
    // Start is called before the first frame update
    public float scrollAmount;
    void Start()
    {
        scrollAmount = 0.002f;
    }

    // Update is called once per frame
    void Update()
    {

        if (GameState.gameStarted)
        {
            transform.position = new Vector3(transform.position.x + scrollAmount, transform.position.y, transform.position.z);

        }
    }


}
