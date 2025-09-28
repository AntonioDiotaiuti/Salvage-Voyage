using UnityEngine;

public class PlayerLook : MonoBehaviour
{

    public float xSensitivity = 30f;
    public float ySensitivity = 30f;
    

  public void ProcessLook(Vector2 input)

  {
    float mouseX = input.x;
        float mouseY = input.y;
   //rotate the player to look
   transform.Rotate(Vector3.up *(mouseX*Time.deltaTime)* xSensitivity);
  }
}
