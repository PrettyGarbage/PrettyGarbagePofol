using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastSelector : MonoBehaviour
{
    public LayerMask selectableLayer;  // Layer that contains selectable objects
    public float raycastDistance = 100f;  // Maximum distance for raycast

    private GameObject selectedObject;  // Currently selected object

    void Update()
    {
        // Check if the mouse is over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the camera through the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Perform a raycast and get the hit info
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, raycastDistance, selectableLayer))
            {
                // Check if the hit object is selectable
                if (hitInfo.collider.GetComponent<SelectableObject>() != null)
                {
                    // Deselect the currently selected object
                    if (selectedObject != null)
                    {
                        selectedObject.GetComponent<SelectableObject>().Deselect();
                    }

                    // Select the new object
                    selectedObject = hitInfo.collider.gameObject;
                    selectedObject.GetComponent<SelectableObject>().Select();
                }
            }
            else
            {
                // Deselect the currently selected object
                if (selectedObject != null)
                {
                    selectedObject.GetComponent<SelectableObject>().Deselect();
                    selectedObject = null;
                }
            }
        }
    }
}
