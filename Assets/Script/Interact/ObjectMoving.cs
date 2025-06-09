using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectMoving : MonoBehaviour
{   public enum ObjectType{ DogBed, DogBowl};
   [SerializeField] private ObjectType objectType;

    [SerializeField] private CanvasGroup objectCanvasGroup;
    [SerializeField] private PlayerInteract playerInteract;

    
    [SerializeField] private GameObject foodBowlEmpty, foodBowlFilled;

    private bool isCompleted = false;
  
    void Update()
    {
        if (playerInteract.ObjectMoving() == this)
        {
            if (objectCanvasGroup != null && !isCompleted)
            {
               objectCanvasGroup.alpha = 1; 
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (isCompleted) return;
                switch (objectType)
                {
                    case ObjectType.DogBed:
                        DogBedMoving();
                        break;
                     case ObjectType.DogBowl:
                        DogBowlFilling();
                        break;
                }
                
            }
        }
        else
        {
            if (objectCanvasGroup != null)
            {
                objectCanvasGroup.alpha = 0;
            }

        }
        if (objectCanvasGroup != null && isCompleted)
        {
            objectCanvasGroup.alpha = 0;
        }
    }
    void DogBedMoving()
    {
        isCompleted = true;
        float objectMove = 1;

        float z = transform.position.z ;
        
        transform.position = new Vector3(transform.position.x, transform.position.y, z -= objectMove);
        
    }
    void DogBowlFilling()
    {
        isCompleted = true;

        foodBowlEmpty.SetActive(false);
        foodBowlFilled.SetActive(true);
    }
}
