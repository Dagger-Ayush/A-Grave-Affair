using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ObjectHovering : MonoBehaviour
{
    float radius = 0.1f;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private Texture2D cursorTextureInRange;
    [SerializeField] private Texture2D cursorTextureOutRange;

    [SerializeField] private AudioSource cursorAudioClip;
    private bool isSoundPlayed = false;

    private Vector2 cursorHotspot;
  
    void Update()
    {
        if (playerInteract != null && playerInteract.isPointAndMovementEnabled)
        {
            ObjectDectecting();
        }
    }
    void ObjectDectecting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit))
        {
            float minRange = 0.09f;
            float maxRange = 0.1f; 

            Collider[] colliderArray = Physics.OverlapSphere(hit.point, radius);

            bool objectInRange = false;

            foreach (Collider collider in colliderArray)
            {
                if (collider.gameObject.CompareTag("TriggerObject"))
                {
                    float distance = Vector3.Distance(hit.point, collider.transform.position);

                    if (distance >= minRange && distance <= maxRange )
                    {
                        objectInRange = true;
                        break; 
                    }
                }
            }

            cursorHotspot = new Vector2(cursorTextureInRange.width / 2, cursorTextureInRange.height / 2);

            if (objectInRange)
            {
                if (!isSoundPlayed)
                { 
                cursorAudioClip.Play();
                isSoundPlayed = true;
                }
                //Cursor.SetCursor(cursorTextureInRange, cursorHotspot, CursorMode.Auto);
            }
            else
            {
                isSoundPlayed = false;
                //Cursor.SetCursor(cursorTextureOutRange, cursorHotspot, CursorMode.Auto);
            }
        }
    }
   
}
