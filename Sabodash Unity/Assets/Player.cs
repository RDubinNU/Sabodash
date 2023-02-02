using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rigbod;

    void Start()
    {
        rigbod = GetComponent<Rigidbody2D>();
    }

    private bool jumped = false;
    void MoveAnywhere()
    {
        var speed = 0.1f;
        var flyspeed = 0.15f;
        var maxvel = 8f;
        var jumpstrength = 10f;

        var horizontalInc = Convert.ToInt32(Input.GetKey("right")) - Convert.ToInt32(Input.GetKey("left"));
        var verticalInc = Convert.ToInt32(Input.GetKey("up")) - Convert.ToInt32(Input.GetKey("down"));
        var dir = new Vector2(horizontalInc * speed, verticalInc * flyspeed);
        //UnityEngine.Debug.Log(dir);
        rigbod.velocity = rigbod.velocity + dir;
        if(rigbod.velocity.magnitude > maxvel){
            rigbod.velocity = rigbod.velocity / rigbod.velocity.magnitude * maxvel;
        }

        if (Input.GetKey(KeyCode.Space) && !jumped)
        {
            rigbod.velocity = rigbod.velocity + new Vector2(0, jumpstrength);
            jumped = true;
        }
        if (!Input.GetKey(KeyCode.Space))
        {
            jumped = false;
        }
        
    }

    private bool buttonWasDown = false;
    void parseButtons(){ 
        bool buttonDown = Input.GetAxis("Fire1s") > 0;
        if(buttonWasDown){
            buttonWasDown = buttonDown;
            return;
        }
        buttonWasDown = buttonDown;

        var inputs = new string[]{"Fire1", "Fire2", "Fire3", "Fire4"};

        //if (Input.GetAxis("Fire1") > 0) Debug.Log("fire");
        for(int i = 1; i < 4; i++){
            if(Input.GetAxis(inputs[i]) > 0){
                
            }
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        MoveAnywhere();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Enemy")
        {

        }
    }
}
