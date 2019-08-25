using UnityEngine;
using Zenject;

public class PathIndicator : MonoBehaviour
{
    [Inject] 
    private MouseManager mm;

    // Start is called before the first frame update
    void Start()
    {
        mm = FindObjectOfType<MouseManager>();
        //transform.localScale = mm.selectedObject.gameObject.transform.localScale;
    }

    void Update()
    {
        OnCover();
        OnClick();
    }

    protected void OnCover()
    {
        if (mm.GetSelectedUnit() != null && mm.SelectedObject != null && !mm.SelectedObject.CompareTag("Unit") &&
            !mm.SelectedObject.CompareTag("Barrier") && !mm.GetSelectedUnit().isPathSet)
        {
            transform.position = mm.SelectedObject.transform.position;
        }
    }

    protected void OnClick()
    {
    }
}