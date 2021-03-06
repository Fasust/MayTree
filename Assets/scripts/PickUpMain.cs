﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpMain : MonoBehaviour {

	[Header("General")]
	public TimerMain timer;
	public Fireworks fireworks;

	[Header("Detection")]
	public float zoneRange;
	public Transform zonePosition;
	public LayerMask playerMask;

	[Header("Visuals")]
	public Animator carAnimator;


	
	// Update is called once per frame
	void FixedUpdate () {
		//Detection ---------------------------------
		Collider2D[] playerColliders = Physics2D.OverlapCircleAll(zonePosition.position, zoneRange, playerMask);

		foreach (Collider2D col in playerColliders)
		{
			PlayerState playerState = col.GetComponent<PlayerState>();
			Movement movement = col.GetComponent<Movement>();

			if (playerState.isCarrying() && playerState.enabled)
			{
				fireworks.play();
				playerState.enabled = false;
				movement.setBlocked(true);
				timer.stop();
			}

		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(zonePosition.position, zoneRange);
	}
}
