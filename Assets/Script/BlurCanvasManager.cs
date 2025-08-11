using UnityEngine;

public class BlurCanvasManager : MonoBehaviour
{
    public GameObject blurBackground; 

    public void OpenDialogue()
    {
        blurBackground.SetActive(true);
        
    }

    public void CloseDialogue()
    {
        blurBackground.SetActive(false);
       
    }
}
