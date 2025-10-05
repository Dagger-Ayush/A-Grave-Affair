using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class InspectionClueFeedBack : MonoBehaviour
{
    public static InspectionClueFeedBack Instance;

    public TMP_Text[] textMeshPro;

    public float typingSpeed = 0.02f;



    public GameObject arrow;
    public GameObject background;

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator clueSet(string clues_1, string clues_2)
    {
        background.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(TypeText(textMeshPro[0], clues_1));
        yield return new WaitForSeconds(0.5f);
        arrow.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(TypeText(textMeshPro[1], clues_2));
        yield return new WaitForSeconds(0.5f);
        background.SetActive(false);
        arrow.SetActive(false);
        // Disable all TMP_Texts
        foreach (TMP_Text txt in textMeshPro)
        {
            if (txt != null)
                txt.gameObject.SetActive(false);
        }
    }
    private IEnumerator TypeText(TMP_Text text, string message)
    {
        text.gameObject.SetActive(true);
        text.text = "";

        foreach (char c in message)
        {
            text.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

    }
}
