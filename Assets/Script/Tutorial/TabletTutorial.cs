using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class TabletTutorial : MonoBehaviour
{
    public static TabletTutorial Instance;

    public GameObject[] interactionPages;
    public TMP_Text[] textMeshPro;
    [TextArea] public string[] fullText;

    public float typingSpeed = 0.05f;

    private bool[] hasTyped;
    private bool isTyping = false;

     
     public bool isTabletTutorialComplete = false;

    public int currentPage = 0;
    private bool isTableOpen;
    private bool FirstPageComplete;
    private bool endDialog;

    public Canvas canvas;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        hasTyped = new bool[textMeshPro.Length];
        canvas.sortingOrder = -1;

    }

  
    public void PageHandler()
    {
        
         if (FirstPageComplete && !isTyping)
        {
            if (FirstPageComplete && Input.GetMouseButtonDown(0))
            {
                ShowCurrentPage();

                return;
            }
        }
        if (!FirstPageComplete)
        {
            if (!isTableOpen && !isTyping)
            {
                FirstPage();
                return;
            }
            else if (isTableOpen && Input.GetKeyDown(KeyCode.Tab))
            {
                foreach (var page in interactionPages)
                    page.SetActive(false);

                canvas.sortingOrder = 100;
                currentPage = 1;
                FirstPageComplete = true;

            }
        }

    }
    private void ShowCurrentPage()
    {
        if (endDialog)
        {
            EndTutorial();
            return;
        }

        ShowPanel(currentPage);
        endDialog = true;
    }

    private void EndTutorial()
    {
        foreach (var page in interactionPages)
            page.SetActive(false);

        isTabletTutorialComplete = true;
        canvas.sortingOrder = -1;

        if (PlayerInteract.Instance != null && PointAndMovement.instance != null)
        {
            PlayerInteract.Instance.enabled = true;
            PointAndMovement.instance.enabled = true;
        }
        
    }

    public void FirstPage()
    {
       
        if (!TabletManager.isTabletOpen)
        {
            if (PlayerInteract.Instance != null && PointAndMovement.instance != null)
            {
                PlayerInteract.Instance.enabled = false;
                PointAndMovement.instance.enabled = false;
            }
            ShowPanel(currentPage);
            isTableOpen = true;

        }
    }

    public void ShowPanel(int index)
    {
        // Hide all pages
        foreach (var page in interactionPages)
            page.SetActive(false);

        // Show the requested page
        interactionPages[index].SetActive(true);

        // Cancel previous typing coroutine
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // Start typing new text if not already typed
        if (!hasTyped[index])
            typingCoroutine = StartCoroutine(TypeText(textMeshPro[index], fullText[index], index));
    }

    private IEnumerator TypeText(TMP_Text text, string message, int index)
    {

        isTyping = true;
        text.text = "";

        foreach (char c in message)
        {
            text.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
        hasTyped[index] = true;
    }
}
