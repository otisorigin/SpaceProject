using UnityEngine;
using Zenject;

public class CameraManager : MonoBehaviour
{
    [Inject] private TileMap _tileMap;
    public float panSpeed = 0.20f;
    public float panBorderThickness = 10f;

    public float scrollSpeed = 20f;

    public float minSize = 5;

    public float maxSize = 8;
//    private readonly Vector2 _panLimit;
//
    public void InitCamera()
    {
       // transform.position = new Vector3(7.0f, _tileMap.mapSizeX / 2.0f, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        
        var cam = Camera.main;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minSize, maxSize);

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
            cam.orthographicSize -= scrollSpeed * 100f * Time.deltaTime; 
        }
        if (Input.GetKey("e") /*|| Input.GetAxis("Mouse ScrollWheel")  < 0f*/)
        {
            cam.orthographicSize += scrollSpeed * 100f * Time.deltaTime; 
        }


        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minSize, maxSize);
        //pos.z = Mathf.Clamp(pos.z, -minZ, -maxZ);
//        float scroll = Input.GetAxis("Mouse ScrollWheel");
//        cam.orthographicSize -= scroll * scrollSpeed * 100f * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, 12, 22);
        pos.y = Mathf.Clamp(pos.y, 4, 30);
       
        transform.position = pos;

    }
}
