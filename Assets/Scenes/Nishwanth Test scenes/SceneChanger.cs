using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public bool istrigger = false;
    [SerializeField] private CanvasGroup interactiImage;
    [SerializeField] private PlayerInteract playerInteract;
    
    void Update()
    {
        if (playerInteract.SceneChangerHandler() == this && !istrigger)
        {
            if (interactiImage != null)
            {
                interactiImage.alpha = 1;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(LevelLoader.Instance.ChangeLevel());
                if (interactiImage != null)
                {
                    interactiImage.alpha = 0;
                }


            }
        }
        else
        {
            if (interactiImage != null)
            {
                //interactiImage.alpha = 0;
            }
        }
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")&& istrigger)
        {
            StartCoroutine(LevelLoader.Instance.ChangeLevel());
        }
    }
}
