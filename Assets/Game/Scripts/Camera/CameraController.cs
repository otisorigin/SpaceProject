using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private UnityEngine.Camera _camera;

    public float panSpeed = 0.20f;
    public float panBorderThickness = 10f;

    public float scrollSpeed = 20f;

    public float minSize = 5;

    public float maxSize = 8;

    void Update()
    {
        Vector3 pos = _camera.transform.position;

        //var cam = UnityEngine.Camera.main;
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minSize, maxSize);

        if (Input.GetKey("w") /*|| Input.mousePosition.y >= Screen.height - panBorderThickness*/)
        {
            pos.y += panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s") /*|| Input.mousePosition.y <= panBorderThickness*/)
        {
            pos.y -= panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("d") /*|| Input.mousePosition.x >= Screen.width - panBorderThickness*/)
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("a") /*|| Input.mousePosition.x <= panBorderThickness*/)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("q") /*|| Input.GetAxis("Mouse ScrollWheel")  > 0f*/)
        {
            _camera.orthographicSize -= scrollSpeed * 100f * Time.deltaTime;
        }

        if (Input.GetKey("e") /*|| Input.GetAxis("Mouse ScrollWheel")  < 0f*/)
        {
            _camera.orthographicSize += scrollSpeed * 100f * Time.deltaTime;
        }


        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minSize, maxSize);
        //pos.z = Mathf.Clamp(pos.z, -minZ, -maxZ);
//        float scroll = Input.GetAxis("Mouse ScrollWheel");
//        cam.orthographicSize -= scroll * scrollSpeed * 100f * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, 12, 22);
        pos.y = Mathf.Clamp(pos.y, 4, 30);

        _camera.transform.position = pos;
    }

    public void MoveCameraTo(Vector3 target)
    {
        var position = _camera.transform.position;
        position.x = Mathf.Clamp(position.x, 12, 22);
        position.y = Mathf.Clamp(position.y, 4, 30);
        _camera.transform.position = Vector3.Lerp(position, new Vector3(target.x, target.y, position.z), panSpeed);
    }

    public UnityEngine.Camera GetMainCamera()
    {
        return _camera.GetComponent<UnityEngine.Camera>();
    }
}