using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer_Movement : MonoBehaviour
{

    public float mouseSensitibvity = 100f;

    public float speed = 120f;

    public Transform viewer;
    public CharacterController character;
    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitibvity * Time.deltaTime;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        Vector3 move = transform.right * x + transform.forward * z;



        viewer.Rotate(Vector3.up * mouseX);

        character.Move(move * speed * Time.deltaTime);
    }
}
