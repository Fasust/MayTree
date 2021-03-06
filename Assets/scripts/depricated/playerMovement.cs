﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerMovement : MonoBehaviour {
    public CharacterController2D controller;

    [Header("Movement")]
    public float runSpeed = 40f;
    public float carryMultiplier = 1.5f;

    [Header("Visuals")]
    public Animator playerAnimator;
    public GameObject particalEffect;
    public Canvas hiddenDisplay;
    private camShake camShake;

    [Header("Input")]
    public Joystick joystick;
    public KeyboardInput keyboard;
    public Button jumpButton;
    public float moveSensetivety = .2f;
    public float sneakSensetivety = .5f;
    public float jumpSensetivety = .5f;

    [Header("Hiding")]
    public LayerMask hideMask;

    [Header("General")]
    public StateManagerMain StateManager;
    public TimerMain timer;

    private float horizontalMove = 0f;
    private bool sneak = false;
    private bool jump = false;
    private float verticalMove = 0f;
    private bool soundPlaying = false;
    private bool detectedAnimationStarted = false;
    private bool jumpSoundPlaying = false;


    private void Start() {
        particalEffect.GetComponent<ParticleSystem>().Stop();
        camShake = GameObject.FindGameObjectWithTag("CamController").GetComponent<camShake>();

        hiddenDisplay.enabled = false;
        jumpButton.onClick.AddListener(jumpUp);

    }

    //REMOVE: Change keyboard to joystick
    void Update() {
        // Movement -------------------------------------------------------
        horizontalMove = keyboard.Horizontal;
        if (Mathf.Abs(keyboard.Horizontal) >= moveSensetivety) {
            if (keyboard.Horizontal > 0) {
                horizontalMove = runSpeed;
            } else {
                horizontalMove = -runSpeed;
            }
        } else {
            horizontalMove = 0;
        }
        //Jump --------------------------------------------------------------
        bool jumping = playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_idel_land") ||
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_right_land") ||
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_jump") ||
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_carrying_land") ||
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_carrying_jump");

        if (keyboard.Vertical >= jumpSensetivety && !jumping) {
            verticalMove = keyboard.Vertical;
            jump = true;
        }

        //Sneak -------------------------------------------------------------
        if (keyboard.Vertical <= sneakSensetivety && !jumping) {
            verticalMove = keyboard.Vertical;
            sneak = true;
        } else {
            verticalMove = keyboard.Vertical;
            sneak = false;
        }

        //Display------------------------------------------------------

        hiddenDisplay.enabled = isHidden();

        //Update Animator-----------------------------------------------------
        playerAnimator.SetFloat("speed", Mathf.Abs(horizontalMove));
        playerAnimator.SetBool("sneak", sneak);
        playerAnimator.SetBool("jump", jump);
    }


    void FixedUpdate() {

        //Get Animation States
        bool chopping = playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_chop");
        bool beingATree = playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_carrying_sneak");
        bool detected = playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_detected");

        //Special cases ----------------------------------------------

        if (chopping || beingATree || detected) {
            horizontalMove = 0;
        }

        if (detected) {
            jump = false;
            verticalMove = 0;
            sneak = false;
        }

        //Movement -----------------------------------------------------

        controller.Move(horizontalMove * runSpeed * Time.fixedDeltaTime, sneak, verticalMove);

        //Sound --------------------------------------------------------
        if (horizontalMove != 0) {
            if (!soundPlaying) {
                soundPlaying = true;
                FindObjectOfType<AudioManager>().play("Step");
            }

        } else {
            soundPlaying = false;
            FindObjectOfType<AudioManager>().stop("Step");
        }

        if (jump && !jumpSoundPlaying) {
            jumpSoundPlaying = true;
            FindObjectOfType<AudioManager>().play("Jump");
        }

        if (sneak || jump) {
            soundPlaying = false;
            FindObjectOfType<AudioManager>().stop("Step");
        }
    }

    public void onLand() {
        jump = false;
        verticalMove = 0;
        jumpSoundPlaying = false;
        playerAnimator.SetBool("jump", jump);

        //screen shake
        camShake.shakeTiny();

        //sound
        FindObjectOfType<AudioManager>().play("Land");
    }

    public void makeCarry() {
        runSpeed *= carryMultiplier;
        playerAnimator.SetTrigger("carrying");
        FindObjectOfType<AudioManager>().play("Pickup");

        //Particels
        particalEffect.GetComponent<ParticleSystem>().Play();
    }

    public bool isSneaking() {
        return sneak;
    }

    public bool isHidden() {
        return isSneaking() && GetComponent<Collider2D>().IsTouchingLayers(hideMask);
    }

    public void detect() {
        if (!detectedAnimationStarted) {
            playerAnimator.SetTrigger("detect");
            FindObjectOfType<AudioManager>().play("Gameover");
            particalEffect.GetComponent<ParticleSystem>().Stop();
            detectedAnimationStarted = true;
        }
        StateManager.showReplay();
        timer.stop();
    }
    public bool hasTree() {
        return playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_carrying") ||
        playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_carrying_idel") ||
        playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_carrying_land") ||
        playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_carrying_jump") ||
        playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_carrying_sneak");
    }
    private void jumpUp() {
        bool jumping = playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_idel_land") ||
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_right_land") ||
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_jump") ||
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_carrying_land") ||
            playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("german_carrying_jump");

        if (!jumping) {
            jump = true;
            verticalMove = 1;
        }

    }
}
