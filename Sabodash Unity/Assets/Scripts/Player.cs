using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class Player : MonoBehaviour
{
    //Controls
    private PlayerInput input;
    private InputAction lr;
    private InputAction jump;
    private InputAction start;
    private InputAction triggers;
    private InputAction sabotage;
    // Parameters
    float horizontal_accel_speed = 0.75f;
    float maxvel_x = 6f;

    float maxvel_y = 6f;
    float fly_accel = 1.25f;
    float jumpstrength = 8f;

    float last_jump = 0f;
    float fly_delay = 0.2f;
    float jump_cd = 0.1f;

    // Variables
    public Rigidbody2D rigbod;
    public BoxCollider2D boxcollider;
    public SpriteRenderer sprite;
    private bool grounded;
    public int bank = 0;
    public GameObject textPrefab;
    private GameObject bank_txt;
    private GameObject sab_txt;

    private const int numSabotages = 5;
    private String[] sabNames = new String[numSabotages] { "a", "b", "c", "d", "e"};
    public int sabSelected = 0;
    private bool sabButtonDown = false;

    void Start() {
        rigbod = GetComponent<Rigidbody2D>();
        boxcollider = GetComponent<BoxCollider2D>();
        input = GetComponent<PlayerInput>();
        lr = input.actions["LR"];
        jump = input.actions["Jump"];
        start = input.actions["Start"];
        triggers = input.actions["Triggers"];
        sabotage = input.actions["Sabotage"];
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        bank_txt = Instantiate(textPrefab, transform.position, Quaternion.identity);
        bank_txt.GetComponent<TextMeshPro>().text = "Hello";
        sab_txt = Instantiate(textPrefab, transform.position, Quaternion.identity);
        sab_txt.GetComponent<TextMeshPro>().text = sabNames[sabSelected];
    }
    void FixedUpdate()
    {
        // Temp starting behaviour
        if (start.ReadValue<float>() > 0) GameState.gameStarted = true;

        grounded = IsGrounded();
        MoveAnywhere();

        bank_txt.transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
        bank_txt.GetComponent<TextMeshPro>().text = bank.ToString();
        sab_txt.transform.position = new Vector2(transform.position.x, transform.position.y + 0.75f);
        sab_txt.GetComponent<TextMeshPro>().text = sabNames[sabSelected];

        parseSabButtons();
    }
    void parseSabButtons()
    {
        if (triggers.ReadValue<float>() > 0 && !sabButtonDown){
            sabSelected++;
            sabButtonDown = true;
        }
        if(triggers.ReadValue<float>() < 0 && !sabButtonDown){
            sabSelected--;
            sabButtonDown = true;
        }
        if(triggers.ReadValue<float>() == 0){
            sabButtonDown = false;
        }
        if (sabSelected > numSabotages-1) sabSelected = numSabotages-1;
        if (sabSelected < 0) sabSelected = 0;
    }
    bool IsGrounded(){
        float tolerance = 0.05f;
        Vector3 raycast_origin = boxcollider.bounds.center + (Vector3)Vector2.down * boxcollider.bounds.extents.y;
        RaycastHit2D ground_raycast = Physics2D.Raycast(raycast_origin, Vector2.down, tolerance);
        bool on_ground = false;
        if (ground_raycast.collider != null && rigbod.velocity.y <= 0.05) {
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
            accel_x = 0; // Clipping acceleration vs velocity allows fun side sliding on slanted surfaces
        }

        // Flying and jump control
        float accel_y = 0;

        if (jump.ReadValue<float>() > 0)
        {
            if (grounded & Time.time > last_jump + jump_cd)
            {
                accel_y = jumpstrength;
                //rigbod.angularVelocity = 300f * Math.Sign(rigbod.velocity.x);
                last_jump = Time.time;
            }
            else if ((Time.time > last_jump + fly_delay))
            {
                accel_y = fly_accel;
            }
        }

        // Acceleration Application
        Vector2 acceleration = new Vector2(accel_x, accel_y);
        rigbod.velocity = rigbod.velocity + acceleration;


        // Fly Capping

        if (rigbod.velocity.y >= maxvel_y && (Time.time > last_jump + fly_delay))
        {
            rigbod.velocity = new Vector2(rigbod.velocity.x,
                                            maxvel_y);
        }


        // Frictional Slowing
        if(grounded && accel_x == 0){
            rigbod.velocity = new Vector2(rigbod.velocity.x * 0.95f, rigbod.velocity.y);
        }
    }
    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
        Debug.Log("player died");
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(col.gameObject);
        bank++;
        Debug.Log("bank updated:");
        Debug.Log(bank);
    }
}