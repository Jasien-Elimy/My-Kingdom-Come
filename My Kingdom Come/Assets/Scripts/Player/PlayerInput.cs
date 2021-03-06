﻿using UnityEngine;
using System.Collections;
using GameHelper;

[RequireComponent(typeof (Player))] //Unity specific, forces the object that uses PlayerInput to also 
//attach a Player component (because we need one to have player input)
public class PlayerInput : MonoBehaviour {

	//================================================================================================
	//[PlayerInput Variables]//
	//================================================================================================
	private Player player;
	private Transform plyCamera;
	//================================================================================================



	//================================================================================================
	//[Awake]// --- Called before Start, used to initialise variables before game
	//================================================================================================
	void Awake () {

	}
	//================================================================================================



	//================================================================================================
	//[Start]// --- Called before Update, used to pass any information after all initialisation
	//================================================================================================
	void Start () {
		player = transform.GetComponent<Player> ();
		plyCamera = transform.FindChild ("Main Camera");
	}
	//================================================================================================



	//================================================================================================
	//[Update]// --- Called every frame to implement game behaviour
	//================================================================================================
	void Update () {

		// If game is not paused
		if (!GameHelper.GlobalVariables.GAME_PAUSED) {

			// If player is not an AI
			if (player.isHuman) {
				if (player.isInDirectMode) {
					//DIRECT CONTROL
					DirectModeMouseActivity ();
					DirectModeKeysActivity ();
				} else {
					//RTS CONTROL
					doCameraMovement ();
					doCameraRotation ();
					doCameraZoom ();
					handleMouseClicks ();
				}
			}
		}
	}
	//================================================================================================



	//================================================================================================
	//[RTSModeCameraMovement]// --- Camera movement controls when in RTS mode
	//================================================================================================
	private void doCameraMovement () {
		
		float cameraMovementSpeed;

		//If SHIFT key is down, move camera 4 times quicker with arrow keys, else move it at default speed
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift)) {
			cameraMovementSpeed = GlobalVariables.CAMERA_MOVEMENT_SPEED_SHIFT_MODIFIER;
		} else {
			cameraMovementSpeed = GlobalVariables.CAMERA_MOVEMENT_SPEED;
		}

		//Vertical camera movement
		float moveDirectionHorizontal = Input.GetAxisRaw("Horizontal");
		float moveDirectionVertical = Input.GetAxisRaw("Vertical");

		Vector3 movementDirection = new Vector3 (moveDirectionHorizontal, 0, moveDirectionVertical);
		Vector3 movement = movementDirection * cameraMovementSpeed * Time.deltaTime;
		plyCamera.Translate (movement);

		// If they are not panning with arrow keys.
		if (moveDirectionHorizontal == 0 && moveDirectionVertical == 0) {
			Vector3 middleOfScreen = new Vector3 (Screen.width/2, Screen.height/2, 0);

			if (WorldInteraction.mouseIsOutsideScreen ()) {
				Vector3 camDirection = (Input.mousePosition - middleOfScreen).normalized;

				// For some reason mouse Position uses x and y so you have to switch the y with z.
				camDirection.z = camDirection.y;
				camDirection.y = 0;

				plyCamera.Translate (camDirection * cameraMovementSpeed * Time.deltaTime);
			}

					
		}
	}
		
	//================================================================================================



	//================================================================================================
	//[RTSModeCameraRotation]// --- Camera rotation controls when in RTS mode
	//================================================================================================
	private void doCameraRotation () {

		float cameraRotationSpeed = GlobalVariables.CAMERA_ROTATION_SPEED;

		if (Input.GetKey (KeyCode.Q)) {
			plyCamera.Rotate (Vector3.up * -cameraRotationSpeed * Time.deltaTime, Space.World);
		} else if (Input.GetKey (KeyCode.E)) {
			plyCamera.Rotate (Vector3.up * cameraRotationSpeed * Time.deltaTime, Space.World);
		}

	}
	//================================================================================================



	//================================================================================================
	//[RTSModeCameraZoom]// --- Camera zoom controls when in RTS mode
	//================================================================================================
	private void doCameraZoom () {

		float cameraScrollZoomSpeed = GlobalVariables.CAMERA_SCROLL_ZOOM_SPEED;

		float mouseScrollWheel = Input.GetAxisRaw("Mouse ScrollWheel");

		//zoom in / down
		if (mouseScrollWheel > 0) {
			if(plyCamera.position.y >= GlobalVariables.CAMERA_MIN_HEIGHT){
				plyCamera.Translate(Vector3.up * cameraScrollZoomSpeed * -mouseScrollWheel * Time.deltaTime);
			}
		//zoom out / up
		}else if(mouseScrollWheel < 0){
			if(plyCamera.position.y <= GlobalVariables.CAMERA_MAX_HEIGHT){
				plyCamera.Translate(Vector3.down * cameraScrollZoomSpeed * mouseScrollWheel * Time.deltaTime);
			}
		}
			
	}
	//================================================================================================



	//================================================================================================
	//[RTSModeMouseActivity]// --- Mouse controls when in RTS mode
	//================================================================================================
	private void handleMouseClicks () {

		//LEFT CLICK
		if (Input.GetMouseButtonDown (0)) {
			if (player.hud.IsMouseOutsideHUD ()) {
				
				Vector3 worldPositionClicked = WorldInteraction.Find3DVectorAt2DPoint (Input.mousePosition);
				GameObject objectClicked = WorldInteraction.FindObjectAt2DPoint (Input.mousePosition);

				if (objectClicked && worldPositionClicked != GlobalVariables.UNREACHABLE_VECTOR) {
					
					Entity ent = objectClicked.GetComponentInParent<Entity> ();

					if (ent) {

						if (ent.IsOwnedBy (player)) {
						
							if (Input.GetKey (KeyCode.LeftControl)) {
								
								if (ent.GetComponentInChildren<Structure> () || player.CheckIfAnotherPlayersEntityIsSelected()) {
									player.DeselectAllEntities ();
									ent.ChangeSelection (player, true);
								} else {
									if (player.CheckIfStructureIsSelected ()) {
										player.DeselectAllEntities ();
									}
									if (player.selectedEntities.Count < GlobalVariables.MAX_ENTITY_SELECTION) {
										ent.ChangeSelection (player, true);
									}
								}

							} else {
								
								player.DeselectAllEntities ();
								ent.ChangeSelection (player, true);

							}

						} else {

							player.DeselectAllEntities ();
							ent.ChangeSelection (player, true);

						}

					} else {
						player.DeselectAllEntities ();
					}

				}

			}
		//RIGHT CLICK
		}else if(Input.GetMouseButtonDown (1)) {
			if (player.hud.IsMouseOutsideHUD ()) {

			}
		}
	}

	//================================================================================================
	//[DirectModeMouseActivity]// --- Mouse controls when in direct mode
	//================================================================================================
	private void DirectModeMouseActivity () {

	}
	//================================================================================================



	//================================================================================================
	//[DirectModeKeysActivity]// --- Camera movement controls when in direct mode
	//================================================================================================
	private void DirectModeKeysActivity () {

	}
	//================================================================================================

}
