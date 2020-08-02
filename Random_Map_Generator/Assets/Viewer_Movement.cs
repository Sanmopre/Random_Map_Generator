using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer_Movement : MonoBehaviour
{

    public float mouseSensitibvity = 100f;

    public float speed = 120f;

    public Transform viewer;
    public CharacterController character;


    void Start() {
        Cursor.visible = false;
    }


    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitibvity * Time.deltaTime;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        Vector3 move = transform.right * x + transform.forward * z;


        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        viewer.Rotate(Vector3.up * mouseX);

        character.Move(move * speed * Time.deltaTime);
    }
}
