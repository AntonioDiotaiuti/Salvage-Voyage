using UnityEngine;

public class FixRigOffset : MonoBehaviour
{
    private Vector3 defaultLocalPos;

    void Awake()
    {
        defaultLocalPos = transform.localPosition;
    }

    void LateUpdate()
    {
        // Blocca solo Y
        Vector3 pos = transform.localPosition;
        pos.y = defaultLocalPos.y;
        transform.localPosition = pos;
    }
}
