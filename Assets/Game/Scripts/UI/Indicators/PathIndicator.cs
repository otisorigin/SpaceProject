using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Indicators
{
    public class PathIndicator : MonoBehaviour
    {
        [Inject] 
        private CursorManager cursorManager;

        // Start is called before the first frame update
        void Start()
        {
            // mm = FindObjectOfType<MouseManager>();
            //transform.localScale = mm.selectedObject.gameObject.transform.localScale;
        }

        void Update()
        {
            OnCover();
            OnClick();
        }

        protected void OnCover()
        {
            if (cursorManager.GetSelectedUnit() != null && cursorManager.SelectedObject != null && !cursorManager.SelectedObject.CompareTag("Unit") &&
                !cursorManager.SelectedObject.CompareTag("Barrier") && !cursorManager.GetSelectedUnit().isPathSet)
            {
                transform.position = cursorManager.SelectedObject.transform.position;
            }
        }

        protected void OnClick()
        {
        }
    }
}