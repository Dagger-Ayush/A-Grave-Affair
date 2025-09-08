using UnityEngine;
using UnityEngine.UI;

public class Visionhover : MonoBehaviour
{
    [Header("Hover")]
    public float radius = 0.2f;
    public Color hoverColor = Color.white;
    public Color unHoverColor = Color.blue;
    [Header("Vision")]
    public Color visionClue = Color.white;
    public Color visionUnClue = Color.blue;

    public GameObject vision;
    public Image visionBorder;
    
    void Update()
    {
        bool found = false;

        
        Ray ray = Camera.main.ScreenPointToRay(vision.transform.position);
       
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (!vision.activeSelf) return;

            Collider[] colliderArray = Physics.OverlapSphere(hit.point, radius);

           
            foreach (Collider collider in colliderArray)
            {
                if (collider.CompareTag("Object"))
                {
                    SetColor(collider.gameObject, hoverColor, visionClue);
                    found = true;
                    break;
                }
            }

        }
        if (!found)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Object"))
            {
                SetColor(obj, unHoverColor, visionUnClue);
            }
        }
    }
    void SetColor(GameObject obj, Color objectColor,Color visionColor)
    {
        if (obj != null)
        {
            obj.GetComponent<MeshRenderer>().material.color = objectColor;
            visionBorder.color = visionColor;
        }
    }
    
}
