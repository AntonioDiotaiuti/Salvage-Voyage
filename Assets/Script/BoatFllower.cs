using UnityEngine;

public class BoatFollower : MonoBehaviour
{
    public Transform root;       // BoatRoot (galleggiamento)
    public Transform controller; // BoatController (yaw e movimento)

    void LateUpdate()
    {
        // Posizione: dal BoatRoot (include onde)
        transform.position = root.position;

        // Rotazione:
        // - roll/pitch dal BoatRoot
        // - yaw dal BoatController
        Vector3 rootEuler = root.rotation.eulerAngles;
        Vector3 controllerEuler = controller.rotation.eulerAngles;

        transform.rotation = Quaternion.Euler(
            rootEuler.x,        // roll
            controllerEuler.y,  // yaw (timone)
            rootEuler.z         // pitch
        );
    }
}