using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class InteractionTutorial : MonoBehaviour
{
    public GameObject[] interactionPages;
    private int count = 0;
    [HideInInspector] public bool isRunning;
    public GameObject[] blurImage;

    public ObjectInteract interactHandler;

    public TMP_Text[] textMeshPro;
    [TextArea] public string[] fullText;
    public float typingSpeed = 0.05f;

    private bool[] hasTyped;
    private bool isTyping = false;
    public string clue;

    [HideInInspector] public bool isInteractionComplete = false;

    public CursorHoverOverClue cursor;

    public bool canHover = false;
    public bool isHovered = false;
    public bool isCluePicked = false;
    
    private void Start()
    {
        // Initialize the hasTyped array based on text length
        hasTyped = new bool[fullText.Length];

        // Ensure all panels are disabled at the start
        foreach (var page in interactionPages)
            page.SetActive(false);
    }

    private void Update()
    {
       
        if (canHover)
        {
            if (cursor.isHovered() == true && !isCluePicked)
            {
                ShowPanel(2);
                isHovered = true;
            }
            else if (cursor.isHovered() == false && !isCluePicked)
            {
                ShowPanel(1);
            }
            if (ClueManager.Instance.ClueCheck(clue) && !isCluePicked && isHovered)
            {
                ShowPanel(3);
                isCluePicked = true;
                isInteractionComplete = true;
            }
             if (Input.GetMouseButtonDown(0)  && isCluePicked && hasTyped[3] == true)
            {
                blurImage[1].SetActive(false);
                interactionPages[3].SetActive(false);
                isRunning = false;
            }
            return;
        }
        TutorialHandler();
    }

    private void TutorialHandler()
    {
        if (!interactHandler.interacting) return;

        if (Input.GetMouseButtonDown(0)&& !isInteractionComplete && !canHover)
        {
            if (!isTyping && hasTyped[count])
                count++;
        }
        

        if (count >= interactionPages.Length )
        {
            isInteractionComplete = true;
            return;
        }

        switch (count)
        {
            case 0:
                blurImage[0].SetActive(true);
                isRunning = true;
                ShowPanel(count);
                break;
            case 1:
               
                    blurImage[0].SetActive(false);
                blurImage[1].SetActive(true);
                ShowPanel(count);
                if (hasTyped[count] == true && cursor.isHovered() == true)
                {
                    canHover = true;
                }
                break;
     

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
