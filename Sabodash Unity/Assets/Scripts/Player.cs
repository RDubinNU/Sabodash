using System;
using Unity.VisualScripting;
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

    float max_vel_y = 6f;
    float fly_accel = 0.05f;
    float jumpstrength = 8f;
    float last_jump = 0f;
    float fly_delay = 0.25f;
    float jump_cd = 0.1f;

    // Variables
    public Rigidbody2D rigbod;
    public BoxCollider2D boxcollider;
    public SpriteRenderer sprite;
    private bool grounded;

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
        // Temp starting behaviour
        if (Input.GetKey("p"))
        {
            GameState.gameStarted = true;
        }

         grounded = IsGrounded();
         MoveAnywhere();

    }

    bool IsGrounded()
    {
        float tolerance = 0.05f;

        Vector3 raycast_origin = boxcollider.bounds.center + (Vector3)Vector2.down * boxcollider.bounds.extents.y;
        RaycastHit2D ground_raycast = Physics2D.Raycast(raycast_origin, Vector2.down, tolerance);

        bool on_ground = false;
        if (ground_raycast.collider != null && rigbod.velocity.y <= 0.05)
        {
            on_ground = true;
        }

        return (on_ground);
    }

    void MoveAnywhere(){
        // Horizontal acceleration control
        float accel_x = lr.ReadValue<float>();
        accel_x = accel_x * horizontal_accel_speed;

        if (Math.Abs(rigbod.velocity.x + accel_x) > maxvel_x)
        {
            accel_x = 0;
        }

        // Flying and jump control
        float accel_y = 0;

        if (jump.ReadValue<float>() > 0)
        {
            if (grounded & Time.time > last_jump + jump_cd)
            {
                accel_y = jumpstrength;
                rigbod.angularVelocity = 300f * Math.Sign(rigbod.velocity.x);
                last_jump = Time.time;
            }
            else if ((Time.time > last_jump + fly_delay) && rigbod.velocity.y + fly_accel <= max_vel_y)
            {
                accel_y = fly_accel;
            }
        }

        // Acceleration Application
        Vector2 acceleration = new Vector2(accel_x, accel_y);
        rigbod.velocity = rigbod.velocity + acceleration;


        // Frictional Slowing
        if(grounded && accel_x == 0){
            rigbod.velocity = new Vector2(rigbod.velocity.x * 0.95f, rigbod.velocity.y);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Kill")
        {
            Destroy(this.gameObject);
            Debug.Log("player died");
        }
    }

}