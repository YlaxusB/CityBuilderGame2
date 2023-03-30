using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows.WebCam;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    public GameObject camera;
    public GameObject player;
    public float moveSpeed = 50;
    public float mouseSensitivity = 500;

    private InputAction kKeyAction;
    void OnEnable()
    {
        kKeyAction = new InputAction(binding: "<Keyboard>/k");
        kKeyAction.Enable();

        kKeyAction.performed += OnKKeyPerformed;
    }

    private void OnKKeyPerformed(InputAction.CallbackContext context)
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Confined ? CursorLockMode.None : CursorLockMode.Confined;
        Cursor.visible = Cursor.lockState == CursorLockMode.Confined ? false : true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Code to execute when right mouse button is clicked
            Debug.Log("Right mouse button clicked");
        }
    }

    // Update is called once per frame
    float cameraX;
    float cameraY;
    void Update()
    {
        // Move
        float horizontalInput = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float verticalInput = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        player.transform.Translate(new Vector3(horizontalInput, 0, verticalInput));


        // Rotate (if right click is being held down)
        if (Input.GetMouseButton(2)) // 2 refers to the middle mouse button
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            cameraX -= mouseY;
            cameraX = Mathf.Clamp(cameraX, -90f, 90f);
            cameraY += mouseX;
            player.transform.localRotation = Quaternion.Euler(cameraX, cameraY, 0f);
        } else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
}
