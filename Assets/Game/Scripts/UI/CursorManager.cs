using UnityEngine;
using Zenject;

public class CursorManager : MonoBehaviour
{
    public GameObject SelectedObject { get; set; }
    [Inject] private GameManager _manager;
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    private Unit _unit;

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        _unit = FindObjectOfType<Unit>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;

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
}