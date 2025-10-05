using System.Collections;
using TMPro;
using UnityEngine;

public class InspectionTutorial : MonoBehaviour
{
    public static InspectionTutorial Instance;

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

    [HideInInspector] public bool isInspectionComplete = false;
    [HideInInspector] public bool isRotationComplete = false;


    public GameObject[] interactionPages;

    public GameObject mouseImage;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (targetObject != null)
        {
            initialRotation = targetObject.rotation;
        }
        hasTyped = new bool[textMeshPro.Length];

        hasRotated = false;
    }

    void Update()
    {
        if (!pickHandler.isPicked)
        {
            interactionPages[0].SetActive(false);
        }
       
        if (!clueTriggered && ClueManager.Instance.ClueCheck(clue))
        {
            StartCoroutine(CluePickHandler());
        }
        if (interactionPages[0].activeSelf && Input.GetMouseButton(0) && !isTyping)
        { 
            interactionPages[0].SetActive(false);

            isRunning = false;
            enabled = false;
        }

        if (pickHandler.isPicked)
        {
           
                if (Input.GetMouseButtonDown(0) && !hasRotated)
                {
                    if (!isTyping && hasTyped[count])
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
                    packanimator.enabled = true;
                    if (!hasTyped[1])
                    {
                      
                        StartCoroutine(TypeText(textMeshPro[1], fullText[1], 1));

                    }
                    clueanimator.SetBool("Empty", true);

                    if (!hasRotated)
                        StartCoroutine(RotationCheck());
                    break;
            }
        }
      
    }
    IEnumerator EndPage(float time)
    {
        interactionPages[0].SetActive(false);
        yield return new WaitForSeconds(time);
        interactionPages[0].SetActive(true);
        StartCoroutine(TypeText(textMeshPro[3], fullText[3], 3));
        
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

  
    IEnumerator CluePickHandler()
    {
        packanimator.enabled = true;
        yield return new WaitForSeconds(0.2f);
        packanimator.SetBool("CluePicking", false);
        StartCoroutine(EndPage(2f));
      
        yield return new WaitForSeconds(0.2f);
        packanimator.enabled = false;

        clueTriggered = true;

    }

    private IEnumerator RotationCheck()
    {
        yield return new WaitForSeconds(2f);

      
        while (!hasRotated)
        {
            packanimator.enabled = false;

            if (Quaternion.Angle(initialRotation, targetObject.transform.rotation) > 5f)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    RotationNextPage();
                }

            }
            yield return null;
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
        hasRotated = true;
        StartCoroutine(Delay());

    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(3.5f);

        isRotationComplete = true;

        packanimator.enabled = false;
        isInspectionComplete = true;

    }

}
