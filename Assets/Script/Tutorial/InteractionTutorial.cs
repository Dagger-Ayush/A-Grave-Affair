using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static ObjectInteract;

public class InteractionTutorial : MonoBehaviour
{
    public static InteractionTutorial Instance;

    [Header("UI References")]
    public GameObject[] interactionPages;
    public GameObject[] blurImage;
    public GameObject mouseImage;

    [Header("Typing Settings")]
    public TMP_Text[] textMeshPro;
    [TextArea] public string[] fullText;
    public float typingSpeed = 0.05f;

    [Header("Handlers")]
    public ObjectInteract interactHandler;
    public CursorHoverOverClue cursor;
    public string clue;

    private bool[] hasTyped;
    private bool isTyping = false;
    private int count = 0;

    [HideInInspector] public bool isRunning;
    [HideInInspector] public bool isInteractionComplete = false;

    public bool canHover = false;
    public bool isHovered = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        isHovered = false;
        isInteractionComplete = false;
        isTyping = false;
        isRunning = false;

        hasTyped = new bool[fullText.Length];

        foreach (var page in interactionPages)
            page.SetActive(false);

        foreach (var blur in blurImage)
            blur.SetActive(false);

        if (mouseImage != null)
            mouseImage.SetActive(false);
    }

    private void Update()
    {
        // 🧩 STEP 3: When clue is found
        if (ClueManager.Instance.ClueCheck(clue) && !isInteractionComplete)
        {
            ShowPanel(3);
            if (hasTyped[3])
            {
                mouseImage.SetActive(true);
                isInteractionComplete = true;
            }
            else
            {
                mouseImage.SetActive(false);
            }
            return;
        }

        // 🧩 Close tutorial when interaction ends
        if (!interactHandler.isInteracted && isRunning)
        {
            mouseImage.SetActive(false);
            foreach (var blur in blurImage) blur.SetActive(false);
            foreach (var page in interactionPages) page.SetActive(false);
            isRunning = false;
        }

        // 🧩 Left click after interaction complete = close panels
        if (Input.GetMouseButtonDown(0) && isInteractionComplete)
        {
            if (interactionPages[3].activeSelf)
            {
                mouseImage.SetActive(false);
                foreach (var blur in blurImage) blur.SetActive(false);
                interactionPages[3].SetActive(false);
            }
            isRunning = false;
        }

        // 🧩 Interaction active
        if (!isInteractionComplete && interactHandler.isInteracted)
        {
            if (canHover)
            {
                HandleHoverPhase();
            }
            else
            {
                TutorialHandler();
            }
        }
    }

    private void HandleHoverPhase()
    {
        if (cursor.isHovered())
        {
            mouseImage.SetActive(true);
            ShowPanel(2); // Hover panel
            isHovered = true;
        }
        else
        {
            mouseImage.SetActive(false);
            ShowPanel(1); // Previous instruction
        }

        blurImage[0].SetActive(false);
    }

    private void TutorialHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isTyping && hasTyped[count])
                count++;
        }

        if (count >= interactionPages.Length)
        {
            isInteractionComplete = true;
            return;
        }

        switch (count)
        {
            case 0: // 🧭 First panel
                mouseImage.SetActive(true);
                blurImage[0].SetActive(true);
                isRunning = true;
                ShowPanel(count);
                break;

            case 1: // 🧭 Explain hover
                mouseImage.SetActive(false);
                blurImage[0].SetActive(false);
                blurImage[1].SetActive(true);
                ShowPanel(count);
                canHover = true;
                break;
        }
    }

    private void ShowPanel(int index)
    {
        foreach (var page in interactionPages)
            page.SetActive(false);

        interactionPages[index].SetActive(true);

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
