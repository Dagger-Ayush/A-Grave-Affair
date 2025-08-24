using System.Collections;
using TMPro;
using UnityEngine;

public class InspectionTutorial : MonoBehaviour
{
    public Animator packanimator;
    public ObjectPickHandler pickHandler;
    public Animator clueanimator;
    public int count;
    [HideInInspector] public bool isRunning;

    public TMP_Text[] textMeshPro;
    [TextArea] public string[] fullText;
    public float typingSpeed = 0.05f;

    private bool[] hasTyped;
    private bool isTyping = false;
    public string clue;

    private bool clueTriggered = false;

    public Transform targetObject; // The object you want to check
    private Quaternion initialRotation;
    private bool hasRotated = false;

    void Start()
    {
        if (targetObject != null)
        {
            initialRotation = targetObject.rotation;
        }
        hasTyped = new bool[textMeshPro.Length];
    }

    void Update()
    {
        if (!clueTriggered && ClueManager.Instance.ClueCheck(clue))
        {
            StartCoroutine(ClueCheck());
          
        }

        if (pickHandler.isPicked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!isTyping)
                    count++;
            }

            switch (count)
            {
                case 0:
                    isRunning = true;
                    if (!hasTyped[0])
                    {
                        StartCoroutine(TypeText(textMeshPro[0], fullText[0], 0));
                    }
                    clueanimator.enabled = true;
                    break;

                case 1:
                   
                    if (!hasTyped[1])
                    {
                        StartCoroutine(TypeText(textMeshPro[1], fullText[1], 1));
                    }
                    clueanimator.SetBool("Empty", true);
                    packanimator.enabled = true;
                    packanimator.SetBool("CluePicking", false);
                    StartCoroutine(RotationCheck());
                    break;
            }
            if (hasRotated && Input.GetMouseButtonUp(0))
            {
                RotationNextPage();
            }
        }
       
    }
   
    IEnumerator TypeText(TMP_Text text, string message, int index)
    {
        if (isTyping) yield break;

        isTyping = true;
        text.text = "";
        foreach (char c in message)
        {
            text.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        hasTyped[index] = true;
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(3.5f);
        isRunning = false;
        packanimator.enabled = false;
    }

    IEnumerator ClueCheck()
    {
        packanimator.enabled = true;
        yield return new WaitForSeconds(0.2f);
        packanimator.SetBool("CluePicking", false);
        yield return new WaitForSeconds(0.2f);
        packanimator.enabled = false;

        clueTriggered = true;
    }
    private IEnumerator RotationCheck()
    {
        yield return new WaitForSeconds(1.5f);
        packanimator.enabled = false;
        if (Quaternion.Angle(initialRotation, targetObject.transform.rotation) > 10f)
        {
          hasRotated = true;  
        }
        else
        {
          hasRotated = false;
        }
    }
    private void RotationNextPage()
    {
        packanimator.enabled = true;
        if (!hasTyped[2])
        {
            StartCoroutine(TypeText(textMeshPro[2], fullText[2], 2));
        }
        packanimator.SetBool("CluePicking", true);
        StartCoroutine(Delay());
    }
}
