using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{

    public GameObject selectedObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;

//        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.Log("Mouse is over: " + hitInfo.collider.name + " " + hitInfo.transform.root.gameObject.name);
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
        if (selectedObject != null)
        {
            if (obj == selectedObject)
            {
                return;
            }
            ClearSelection();
        }
        selectedObject = obj;
    }

    void ClearSelection()
    {
        selectedObject = null;
    }
}
