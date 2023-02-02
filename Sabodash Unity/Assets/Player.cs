using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Parameters
    float speed = 0.1f;
    float flyspeed = 0.15f;
    float maxvel = 8f;
    float jumpstrength = 10f;

    //Variables
    public Rigidbody2D rigbod;
    private bool jumped = false;

    void Start()
    {
        rigbod = GetComponent<Rigidbody2D>();
    }

    void MoveAnywhere()
    {
        var horizontalInc = Convert.ToInt32(Input.GetKey("right")) - Convert.ToInt32(Input.GetKey("left"));
        var verticalInc = Convert.ToInt32(Input.GetKey("up")) - Convert.ToInt32(Input.GetKey("down"));
        var dir = new Vector2(horizontalInc * speed, verticalInc * flyspeed);
        //Debug.Log(dir);
        rigbod.velocity = rigbod.velocity + dir;
        if(rigbod.velocity.magnitude > maxvel){
            rigbod.velocity = rigbod.velocity / rigbod.velocity.magnitude * maxvel;
        }
        if (Input.GetKey(KeyCode.Space) && !jumped){
            rigbod.velocity = rigbod.velocity + new Vector2(0, jumpstrength);
            jumped = true;
        }
        if (!Input.GetKey(KeyCode.Space)){
            jumped = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveAnywhere();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == ""){}
    }
}