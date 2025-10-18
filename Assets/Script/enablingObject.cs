using UnityEngine;

public class enablingObject : MonoBehaviour
{
    private ObjectPickHandler pickHandler;

    private void Start()
    {
        pickHandler = GetComponent<ObjectPickHandler>();
    }
    public void enabling()
    {

        gameObject.SetActive(true);
        pickHandler.enabled = true;
        pickHandler.shouldWork = true;
    }
}
