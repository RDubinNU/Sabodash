using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Player : MonoBehaviour
{

    // Public attributes
    public int playerID;
    public int colourIndex = -1;
    public int playerWins = 0;

    // Ready mechanics
    public bool ready = false;
    private bool readyReleased = true;

    // Controls
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


    //Variables for Bouncy Sabotage (4)
    public PhysicsMaterial2D mat_normal;
    public PhysicsMaterial2D mat_bouncy;
    //Variables for Pullback Sabotage (5)
    public float sab_vel_percent = 1;


    // Variables
    public Rigidbody2D rigbod;
    public BoxCollider2D boxcollider;
    public SpriteRenderer sprite;
    private bool grounded;
    public int bank = 0;
    public GameObject textPrefab;
    public GameObject bank_txt;
    public GameObject sab_txt;

    public Vector3 spawnPoint = new Vector3(0, 0, 0);

    private const int numSabotages = 8;
    private String[] sabNames = new String[numSabotages] {"Big", "Grey", "Grav", "Ctrl",
                                                            "Bncy", "Slow", "Stop", "Frwd"};

    private int sabSelected = 0;
    private bool triggerDown = false;

    public List<float> playerSabotageCooldowns = new List<float>();
    public List<float> playerSabotageDurs = new List<float>();
    private const int GENERAL_SABOTAGE_CD_DUR = 3;
    public float playerGeneralSabCD = 0;

    public int directionScale = 1;

    void Start() {

        // Physics Instantiation
        rigbod = GetComponent<Rigidbody2D>();
        boxcollider = GetComponent<BoxCollider2D>();

        // Input Instantiation
        input = GetComponent<PlayerInput>();
        lr = input.actions["LR"];
        jump = input.actions["Jump"];
        start = input.actions["Start"];
        triggers = input.actions["Triggers"];
        sabotage = input.actions["Sabotage"];

        // Sprite Instantiation
        sprite = GetComponent<SpriteRenderer>();
        updatePlayerColour(1);

        // HUD Instantiation
        bank_txt = Instantiate(textPrefab, transform.position, Quaternion.identity);
        bank_txt.GetComponent<TextMeshPro>().text = "";
        sab_txt = Instantiate(textPrefab, transform.position, Quaternion.identity);
        sab_txt.GetComponent<TextMeshPro>().text = "";

        // Sabotage instantiation
        for (int i = 0; i < Sabotages.sabotageCount; i++)
        {
            playerSabotageCooldowns.Add(0);
            playerSabotageDurs.Add(-1);
        }

        // Game state control
        GameState.AddPlayer(this);
        
    }

    private void Update()
    {
        updateHUD();
        parseTriggers();
        checkReady();

    }

    void FixedUpdate()
    {
        

        // Physics updates
        grounded = IsGrounded();
        MoveAnywhere();

        // Sabotage usage
        tickSabotageTimers();
        CheckForSabotageUse();

    }
    void parseTriggers()
    {
        if (GameState.gameStarted)
        {
            // Sabotage Handling
            if (triggers.ReadValue<float>() > 0 && !triggerDown){
                sabSelected++;
                triggerDown = true;
            } else if (triggers.ReadValue<float>() < 0 && !triggerDown)
            {
                sabSelected--;
                triggerDown = true;
            } else if (triggers.ReadValue<float>() == 0)
            {
                triggerDown = false;
            }
            if (sabSelected > numSabotages - 1) sabSelected = 0;
            if (sabSelected < 0) sabSelected = numSabotages - 1;
        } else
        {
            // Colour Selection
            if (triggers.ReadValue<float>() > 0 && !triggerDown)
            {
                triggerDown = true;
                updatePlayerColour(1);
            }
            else if (triggers.ReadValue<float>() < 0 && !triggerDown)
            {
                triggerDown = true;
                updatePlayerColour(-1);
            }
            else if (triggers.ReadValue<float>() == 0)
            {
                triggerDown = false;
            }
        }
        
    }

    void checkReady()
    {
        if (!GameState.gameStarted)
        {
            if (sabotage.ReadValue<float>() > 0 && readyReleased)
            {
                ready = !ready;
                readyReleased = false;
                spawnPoint = rigbod.transform.position;
            } else if (sabotage.ReadValue<float>() == 0)
            {
                readyReleased = true;
            }
        }
    }

    bool IsGrounded(){
        // Grounding check for jumps

        float tolerance = 0.025f;
        Vector3 raycast_origin = boxcollider.bounds.center + (Vector3)Vector2.down * boxcollider.bounds.extents.y;
        RaycastHit2D ground_raycast = Physics2D.Raycast(raycast_origin, Math.Sign(rigbod.gravityScale) * Vector2.down, tolerance);
        bool on_ground = false;
        if (ground_raycast.collider != null && rigbod.velocity.y <= 0.05) {
            on_ground = true;
        } 
        return (on_ground);
    }
    void MoveAnywhere(){
        // Horizontal acceleration control
        float accel_x = lr.ReadValue<float>();

        accel_x = accel_x * horizontal_accel_speed * directionScale;
        if (Math.Abs(rigbod.velocity.x + accel_x) > maxvel_x * sab_vel_percent)
        {
            accel_x = 0; // Clipping acceleration vs velocity allows fun side sliding on slanted surfaces
        }

        // Flying and jump control
        float accel_y = 0;

        if (jump.ReadValue<float>() > 0)
        {
            if (grounded & Time.time > last_jump + jump_cd)
            {
                accel_y = jumpstrength * Math.Sign(rigbod.gravityScale);
                last_jump = Time.time;
            }
            else if ((Time.time > last_jump + fly_delay))
            {
                accel_y = fly_accel * Math.Sign(rigbod.gravityScale);
            }
        }

        // Acceleration Application
        Vector2 acceleration = new Vector2(accel_x, accel_y);
        rigbod.velocity = rigbod.velocity + acceleration;


        // Fly Capping

        if (rigbod.velocity.y >= maxvel_y && rigbod.gravityScale > 0)
        {
            rigbod.velocity = new Vector2(rigbod.velocity.x,
                                            maxvel_y);
        } else if ((rigbod.velocity.y <= -maxvel_y) && rigbod.gravityScale < 0)
        {
            rigbod.velocity = new Vector2(rigbod.velocity.x,
                                            -maxvel_y);
        }


        // Frictional Slowing
        if(grounded && accel_x == 0){
            rigbod.velocity = new Vector2(rigbod.velocity.x * 0.95f, rigbod.velocity.y);
        }
    }

    void CheckForSabotageUse()
    {
        if (GameState.gameStarted && sabotage.ReadValue<float>() > 0)
        {
            AttemptSabotageUse();
        }
    }

    void AttemptSabotageUse()
    {
        if (playerSabotageCooldowns[sabSelected] == 0 && playerGeneralSabCD == 0)
        {
            bool applied = Sabotages.ApplySabotage(sabSelected, this);
            
            if (applied) {
                // Update CDs
                playerSabotageCooldowns[sabSelected] = Sabotages.sabVars[sabSelected].CD;
                playerSabotageDurs[sabSelected] = Sabotages.sabVars[sabSelected].dur;
                playerGeneralSabCD = GENERAL_SABOTAGE_CD_DUR;
            }
        }
    }

    private void OnBecameInvisible()
    {
        if (GameState.gameStarted)
        {
            GameState.RemovePlayer(this);
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(col.gameObject);
        bank++;
    }

    void updateHUD() {

        if (GameState.gameStarted)
        {
            // Display Updates
            bank_txt.transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
            bank_txt.GetComponent<TextMeshPro>().text = bank.ToString();
            sab_txt.transform.position = new Vector2(transform.position.x, transform.position.y + 0.75f);
            sab_txt.GetComponent<TextMeshPro>().text = sabNames[sabSelected];
        } else
        {
            bank_txt.transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
            if (ready) bank_txt.GetComponent<TextMeshPro>().text = "Ready!";
            else bank_txt.GetComponent<TextMeshPro>().text = "";
            sab_txt.GetComponent<TextMeshPro>().text = "";
        }
    }

    void updatePlayerColour(int direction)
    {
        GameState.GetNextColour(this, direction);
        sprite.color = GameState.possibleColours[colourIndex];
    }

    public void tickSabotageTimers()
    {

        // Update CDs

        // Update Individual Timers
        for (int i = 0; i < playerSabotageDurs.Count; i++) 
        {
            // Tick timers
            playerSabotageCooldowns[i] -= Time.fixedDeltaTime;
            playerSabotageDurs[i] -= Time.fixedDeltaTime;

            // Update Cooldowns
            if (playerSabotageCooldowns[i] < 0)
            {
                playerSabotageCooldowns[i] = 0;
            }

            // Update durations and reset
            if (playerSabotageDurs[i] < 0 && playerSabotageDurs[i] > -1)
            { 
                Sabotages.ResetSabotage(i, this);
                playerSabotageDurs[i] = -1;
            } else if (playerSabotageDurs[i] < -1)
            {
                playerSabotageDurs[i] = -1;
            }
        }

        // Update general timer
        playerGeneralSabCD -= Time.fixedDeltaTime;
        if (playerGeneralSabCD < 0) playerGeneralSabCD = 0;


        // Update Durations

    }
}