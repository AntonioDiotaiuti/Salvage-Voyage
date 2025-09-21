using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public float amplitude = 1f;
    public float length = 2f;
    public float speed = 1f;
    public float offset = 0f;
    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Istance alrea exists, destroying object!");
        }
    }
    private void Update()
{
        offset += Time.deltaTime * speed;

}
    public float GetwaveHeight(float _x)
    {
      return amplitude * Mathf.Sin(_x /  length + offset);
    }
}