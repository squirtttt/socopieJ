using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement: MonoBehaviour
{
    public CharacterController controller;
    public CharacterController characterController;
    public float speed = 2f;
    public float gravity = -9.181f;
    public float Gravity = 9.8f;
    private float velocity = 0;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * y;
        controller.Move(move * speed * Time.deltaTime);

        if(characterController.isGrounded)
        {
            velocity = 0;
            velocity -= Gravity * Time.deltaTime;
        }
        else
        {
            characterController.Move(new Vector3(0, velocity, 0));
        }
    }
}
