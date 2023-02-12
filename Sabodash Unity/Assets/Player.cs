using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //Controls
    private PlayerInput input;
    private InputAction lr;
    private InputAction jump;
    // Parameters
    float horizontal_accel_speed = 0.05f;
    float maxvel_x = 6f;

    float fly_cap = 6f;
    float fly_accel = 0.3f;
    float jumpstrength = 2.5f;

    // Variables
    public Rigidbody2D rigbod;
    public BoxCollider2D boxcollider;
    public SpriteRenderer sprite;

    void Start() {
        rigbod = GetComponent<Rigidbody2D>();
        boxcollider = GetComponent<BoxCollider2D>();
        input = GetComponent<PlayerInput>();
        lr = input.actions["LR"];
        jump = input.actions["Jump"];
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }
    void Update()
    {
        MoveAnywhere();
    }

    bool IsGrounded()
    {
        float tolerance = 0.05f;

        Vector3 raycast_origin = boxcollider.bounds.center + (Vector3)Vector2.down * boxcollider.bounds.extents.y;
        RaycastHit2D ground_raycast = Physics2D.Raycast(raycast_origin, Vector2.down, tolerance);

        Color rayColor;

        bool on_ground = false;
        if (ground_raycast.collider != null)  on_ground = true;

        if (on_ground) rayColor = Color.green;
        else  rayColor = Color.red;

        UnityEngine.Debug.DrawRay(raycast_origin, Vector2.down * tolerance, rayColor);
        return (on_ground);
    }

    void MoveAnywhere(){
        // Horizontal acceleration control
        float accel_x = lr.ReadValue<float>();
        accel_x = accel_x * horizontal_accel_speed;

        // Flying and jump control
        bool grounded = IsGrounded();
        float accel_y = 0;

        if (jump.ReadValue<float>() > 0){
            if (grounded) accel_y = jumpstrength;
            else accel_y = fly_accel;
        }

        // Acceleration Application
        // Horizontal velocity squash
        if (Math.Abs(rigbod.velocity.x) > maxvel_x){
            accel_x = 0;
        }
        // Vertical velocity squash
        if (Math.Abs(rigbod.velocity.y) > fly_cap){
            accel_y = 0;
        }

        Vector2 acceleration = new Vector2(accel_x, accel_y);
        rigbod.velocity = rigbod.velocity + acceleration;

        if(grounded && accel_x == 0){
            rigbod.velocity = new Vector2(rigbod.velocity.x * 0.95f, rigbod.velocity.y);
        }
    }

}