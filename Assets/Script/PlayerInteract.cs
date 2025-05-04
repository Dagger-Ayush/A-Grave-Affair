using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    
    void Update()
    {
        GetObjectInteract();
    }
    public ObjectInteract GetObjectInteract()
    {
        //checking if the object is in range
        float interactRange = 3f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out ObjectInteract objectInteract))
            {
                return objectInteract;
            }

        }
        return null;
    }
    public ObjectPickHandler GetObjectPickHandler()
    {
        //checking if the object is in range
        float interactRange = 3f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out ObjectPickHandler objectPickHandler))
            {
                return objectPickHandler;
            }

        }
        return null;
    }
}
