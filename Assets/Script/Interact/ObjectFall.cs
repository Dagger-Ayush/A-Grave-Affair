using System.Collections;
using UnityEngine;

public class ObjectFall : MonoBehaviour
{
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            StartCoroutine(Delay());
            
        }
    }
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(1);
        rb.isKinematic = true;
    }
}
