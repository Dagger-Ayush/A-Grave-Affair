using System.Collections;
using UnityEditor;
using UnityEngine;

public class ProtoGameOver : MonoBehaviour
{
    [SerializeField]private ObjectMoving objectMoving;
    [SerializeField]private GameObject EndImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
         {
            StartCoroutine(objectMoving.FadeInAndOut());
            
            StartCoroutine(EndGame());
        }
    }
    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(0.2f);
        EndImage.SetActive(true);
        yield return new WaitForSeconds(2f);
#if UNITY_EDITOR
        // If in the editor, stop play mode
        EditorApplication.isPlaying = false;
#else
            // If a build, quit the application
            Application.Quit();
#endif
        
    }
}
