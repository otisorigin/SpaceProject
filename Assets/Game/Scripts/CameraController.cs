using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float panSpeed = 0.20f;

    public float panBorderThickness = 10f;
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.y += panSpeed * Time.deltaTime;
        }

        transform.position = pos;

    }
}
