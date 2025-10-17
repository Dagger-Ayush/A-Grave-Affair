using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class XrayTutorial : MonoBehaviour
{
    public static XrayTutorial Instance;
    public GameObject[] interactionPages;
    private int count = 0;
    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool shouldShowIcon = false;
    //public GameObject[] blurImage;

    public TMP_Text[] textMeshPro;
    [TextArea] public string[] fullText;
    public float typingSpeed = 0.05f;

    private bool[] hasTyped;
    private bool isTyping = false;

    [HideInInspector] public bool IsXrayTutorialCompleted = false;

    public ObjectPickHandler ObjectPickHandler;

    [Header("Sort Canvas")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas TutorialCanvas;

    public GameObject mouseImage;
    public PuzzleData xrayPuzzle;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        // Initialize the hasTyped array based on text length
        hasTyped = new bool[fullText.Length];

        // Ensure all panels are disabled at the start
        foreach (var page in interactionPages)
            page.SetActive(false);
    }

    void Update()
    {
        if (xrayPuzzle.isCompleted) {
            // Ensure all panels are disabled at the start
            foreach (var page in interactionPages)
                page.SetActive(false);
        }
        if (ObjectPickHandler != null && ObjectPickHandler.isPicked)
        {
            if (count < 1 && !isTyping)
            {
                mouseImage.SetActive(true);
            }
            else
            {
                mouseImage.SetActive(false);
            }
            if (count == 1)
            {
                    if (!isTyping && hasTyped[1] && Input.GetKeyDown(KeyCode.Q))
                    {
                        count = 2;
                        mainCanvas.sortingOrder = 0;
                        TutorialCanvas.sortingOrder = 0;
                        ShowPanel(count);
                    }
                return;
            }
            if (Input.GetMouseButtonDown(0) && !IsXrayTutorialCompleted)
            {
                if (!isTyping && hasTyped[count])
                    count++;
            }


            switch (count)
            {
                case 0:
                    isRunning = true;
                    shouldShowIcon = false;

                    mainCanvas.sortingOrder = 1;
                    TutorialCanvas.sortingOrder = 1;
                    ShowPanel(count);
                    break;
                case 1:
                    shouldShowIcon = true;
                    mainCanvas.sortingOrder = 1;
                    TutorialCanvas.sortingOrder = 1;
                    ShowPanel(count);
                    break;
                case 3:
                    
                    foreach (var page in interactionPages)
                        page.SetActive(false);
                    IsXrayTutorialCompleted = true;
                    isRunning = false;
                    break;
            }
        }

    }
    private void ShowPanel(int index)
    {
        // Hide all first
        foreach (var page in interactionPages)
            page.SetActive(false);

        // Show current
        interactionPages[index].SetActive(true);

        // Start typing if not already done
        if (!hasTyped[index])
            StartCoroutine(TypeText(textMeshPro[index], fullText[index], index));
    }

    private IEnumerator TypeText(TMP_Text text, string message, int index)
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
}
