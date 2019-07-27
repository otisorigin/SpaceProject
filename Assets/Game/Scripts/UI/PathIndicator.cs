using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathIndicator : MonoBehaviour
{
    private MouseManager mm;
    
    // Start is called before the first frame update
    void Start()
    {
        mm = GameObject.FindObjectOfType<MouseManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mm.selectedObject != null && !mm.selectedObject.CompareTag("Unit") && !mm.selectedObject.CompareTag("Barrier"))
        {
            transform.position = mm.selectedObject.transform.position;
        }
    }
}
