using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Parameters
    float horizontal_accel_speed = 0.05f;
    float maxvel_x = 6f;

    float fly_cap = 2f;
    float fly_accel = 0.05f;
    float jumpstrength = 4f;

    // Variables
    public Rigidbody2D rigbod;
    public BoxCollider2D boxcollider;

    void Start()
    {
        rigbod = GetComponent<Rigidbody2D>();
        boxcollider = GetComponent<BoxCollider2D>();
    }


    bool IsGrounded()
    {
        float tolerance = 0.01f;
        RaycastHit2D ground_raycast = Physics2D.Raycast(boxcollider.bounds.center, Vector2.down, boxcollider.bounds.extents.y + tolerance);
        Debug.Log(boxcollider.bounds.extents.y);

        Color rayColor;
        if (ground_raycast.collider != null)
        {
           rayColor = Color.green;
        } else
        {
           rayColor = Color.red;
        }

        Debug.DrawRay(boxcollider.bounds.center, Vector2.down * (boxcollider.bounds.extents.y*2 + tolerance), rayColor);
        return (ground_raycast.collider != null);
    }

    void MoveAnywhere()
    {

        // Horizontal acceleration control
        float accel_x = Convert.ToInt32(Input.GetKey("right")) - Convert.ToInt32(Input.GetKey("left"));
        accel_x = accel_x * horizontal_accel_speed;


        // Flying and jump control

        bool grounded = IsGrounded();
        float accel_y = 0;

        if (Input.GetKey("up"))
        {
            
            if (grounded)
            {
                accel_y = jumpstrength;
            }
            else
            {
                accel_y = fly_accel;
            }
        }


        // Acceleration Application

        // Horizontal velocity squash
        if (Math.Abs(rigbod.velocity.x) > maxvel_x)
        {
            accel_x = 0;
        }

        // Vertical velocity squash
        if (Math.Abs(rigbod.velocity.y) > fly_cap)
        {
            accel_y = 0;
        }


        Vector2 acceleration = new Vector2(accel_x,
                                           accel_y);


        rigbod.velocity = rigbod.velocity + acceleration;


    }


    void Update()
    {
        MoveAnywhere();
    }

}