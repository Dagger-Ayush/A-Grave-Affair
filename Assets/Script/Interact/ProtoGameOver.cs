using UnityEngine;

public class ProtoGameOver : MonoBehaviour
{
    [SerializeField]private ObjectMoving objectMoving;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
         {
            StartCoroutine(objectMoving.FadeInAndOut());
         }
    }
}
