using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController Controller;
    private Vector3 playerVelocity;
    public float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   //recive input from input Manager
   public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x  = input.x;
        moveDirection.z = input.y;
        Controller.Move(transform.TransformDirection(moveDirection)* speed * Time.deltaTime);
    }



}
