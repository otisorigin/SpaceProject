using UnityEngine;
using Zenject;

public class MouseManager : MonoBehaviour
{
    public GameObject SelectedObject { get; set; }
    [Inject] private GameController _controller;
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    // Start is called before the first frame update
    private void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;

//        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (Physics.Raycast(ray, out hitInfo))
        {
//            Debug.Log("Mouse is over: " + hitInfo.collider.name + " " + hitInfo.transform.root.gameObject.name);
            GameObject hitObject = hitInfo.transform.root.gameObject;

            SelectObject(hitObject);
        }
        else
        {
            ClearSelection();
        }
    }

    void SelectObject(GameObject obj)
    {
        if (SelectedObject != null)
        {
            if (obj == SelectedObject)
            {
                return;
            }

            ClearSelection();
        }

        SelectedObject = obj;
    }

    void ClearSelection()
    {
        SelectedObject = null;
    }

    public Unit GetSelectedUnit()
    {
        return _controller.SelectedUnit;
    }
}