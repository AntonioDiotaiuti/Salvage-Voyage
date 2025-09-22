using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    private PlayerInput PlayerInput;
    private PlayerInput.OnFootActions OnFoot;


    private PlayerMotor motor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        PlayerInput = new PlayerInput();
        OnFoot = PlayerInput.OnFoot;
        motor = GetComponent<PlayerMotor>();
    }
    
        
 

    // Update is called once per frame
     private void FixedUpdate  ()
    {
      motor.ProcessMove(OnFoot.Movement.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        OnFoot.Enable();
    }

    private void OnDisable()
    {
      OnFoot.Disable();
    }

}

