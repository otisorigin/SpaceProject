
using UnityEngine;

namespace Game.Scripts.UI
{
    public class PathIndicator : MonoBehaviour
    {
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
            if (mm.GetSelectedUnit() != null && mm.selectedObject != null && !mm.selectedObject.CompareTag("Unit") &&
                !mm.selectedObject.CompareTag("Barrier") && !mm.GetSelectedUnit().isPathSet)
            {
                transform.position = mm.selectedObject.transform.position;
            }
        }

        protected void OnClick()
        {
        }
    }
}