using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TabletManager : MonoBehaviour
{
    public static TabletManager Instance;

    public RectTransform tabletPanel;
    public float slideDuration = 0.3f;
    public float hiddenY = -450f;
    public float visibleY = 50f;

    private bool isOpen = false;
    public bool canOpenTablet = false;

    public GameObject puzzlePanel;
    public Transform sentenceContainer;
    public Transform clueContainer;
    public GameObject clueBox;
    public GameObject dropZonePrefab;
    public GameObject wordPrefab;
    public GameObject correctWordPrefab;
    public PuzzleValidator validator;
    //public List<PuzzleData> puzzles;
    public TMP_Text sentenceText;
    public RectTransform sentencePanel;
    public float maxRowWidth = 250f;

    public GameObject blurPanel;
    public static bool isTabletOpen;
    public PointAndMovement pointAndMovement;
    public PlayerInteract playerInteract;
    private PuzzleData currentDisplayedPuzzle = null;
    public PuzzleProgression puzzleProgression;

    public GameObject[] tabs;
    private int currentTab = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        SetY(hiddenY);
        ShowTab(currentTab);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && ObjectInteract.activeInteraction == null && ObjectPickHandler.activePickup==null)
        {
            if (!canOpenTablet)
            {
                Debug.Log("Tablet is locked. Interact with the object first!");
                return;
            }

            isOpen = !isOpen;
            StopAllCoroutines();
            StartCoroutine(SlideTablet(isOpen));
        }

        if(isTabletOpen && Input.GetKeyDown(KeyCode.Q) && puzzleProgression.InfoPanelEnable())
        {
            NextTab();
        }
    }

    public void NextTab()
    {
        tabs[currentTab].SetActive(false); // hide current tab

        currentTab = (currentTab + 1) % tabs.Length;

        tabs[currentTab].SetActive(true); // show new tab
    }

    private void ShowTab(int index)
    {
        foreach (var tab in tabs)
        {
            tab.SetActive(false);
        }

        tabs[index].SetActive(true);
    }
    public void ToggleClueContainer()
    {
        if(clueBox != null)
            clueBox.SetActive(!clueBox.activeSelf);
    }
    public IEnumerator SlideTablet(bool show)
    {
        isTabletOpen = show;
        float startY = tabletPanel.anchoredPosition.y;
        float endY = show ? visibleY : hiddenY;
        float elapsed = 0f;

        if(blurPanel != null)
            blurPanel.SetActive(show);

        if (show && pointAndMovement != null)
        {
            pointAndMovement.enabled = false;
            playerInteract.enabled = false;
            Debug.Log("Disabled player movement");
        }
        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float newY = Mathf.Lerp(startY, endY, elapsed / slideDuration);
            SetY(newY);

            yield return null;
        }
        SetY(endY);

        if(show)
        {
            CursorManager.Instance.SetCursor(CursorState.Tablet);
        }
        else
        {
            CursorManager.Instance.SetCursor(CursorState.Normal);
        }

        Time.timeScale = show ? 0f : 1f;

        if (!show && pointAndMovement != null)
        {
            pointAndMovement.enabled = true;
            playerInteract.enabled = true;
        }

    }

    public void CloseTablet()
    {
        if(isTabletOpen)
        {
            StartCoroutine(CloseTabletRoutine());
        }
    }
    private IEnumerator CloseTabletRoutine()
    {
        yield return SlideTablet(false);
        isTabletOpen = false;
    }
    void SetY(float y)
    {
        Vector2 pos = tabletPanel.anchoredPosition;
        pos.y = y;
        tabletPanel.anchoredPosition = pos;
    }

    public void OpenPuzzle(PuzzleData puzzle)
    {   
        validator.currentPuzzle = puzzle;

        if(puzzle.isCompleted)
        {
            DisplayCompletedSentence(puzzle);
            clueBox.SetActive(false);
            return;
        }
        
        sentenceText.text = puzzle.sentenceTemplate;

        Debug.Log("Opening puzzle: " + puzzle.name);
        Debug.Log("Sentence: " + puzzle.sentenceTemplate);
        Debug.Log("sentenceText object: " + sentenceText);

        BuildSentenceWithDropZone(puzzle.sentenceTemplate);
    }

    void ShowClues(PuzzleData puzzle)
    {
        ClueManager.Instance.ShowRelevantClue(puzzle);
    }
    public void TogglePuzzle(PuzzleData puzzle)
    {
       if(!puzzlePanel.activeSelf)
        {
            currentDisplayedPuzzle = puzzle;
            OpenPuzzle(puzzle);
            ShowClues(puzzle);
            puzzlePanel.SetActive(true);

            if(validator.feedbackText != null)
            {
                validator.feedbackText.gameObject.SetActive(true);
                validator.feedbackText.text = "";
            }
        }

       else if (puzzle ==  currentDisplayedPuzzle)
        {
            puzzlePanel.SetActive(false);
            currentDisplayedPuzzle = null;

            if(validator.feedbackText != null)
                validator.feedbackText.gameObject.SetActive(false);
        }
        else
        {
            currentDisplayedPuzzle = puzzle;
            OpenPuzzle(puzzle);
            ShowClues(puzzle);

            if (validator.feedbackText != null)
            {
                validator.feedbackText.gameObject.SetActive(true);
                validator.feedbackText.text = "";
            }
        }
    }

    
    void BuildSentenceWithDropZone(string sentence)
    {
       
        foreach (Transform child in sentencePanel)
            Destroy(child.gameObject);

        validator.dropZones.Clear();

        GameObject currentRow = CreateNewRow();
        float rowWidth = 0f;

        string[] tokens = sentence.Split(' ');

        foreach(string token in tokens)
        {
            GameObject element;
            if (token == "_")
            {
                element = Instantiate(dropZonePrefab);
                DropZone dropZone = element.GetComponent<DropZone>();
                dropZone.validator = validator;

                validator.dropZones.Add(dropZone);
            }
            else
            {
                element = Instantiate(wordPrefab);
                TMP_Text text = element.GetComponentInChildren<TMP_Text>();
                if(text != null) text.text = token;
            }

            RectTransform rect = element.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            
            float elementWidth = rect.rect.width;

            if (rowWidth + elementWidth > maxRowWidth)
            {
                currentRow = CreateNewRow();
                rowWidth = 0f;
            }
            element.transform.SetParent(currentRow.transform, false);
            rowWidth += elementWidth;
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(sentencePanel);
        
    }
    private GameObject CreateNewRow()
    {
        GameObject row = new GameObject("SentenceRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        row.transform.SetParent(sentencePanel, false);

        var layout = row.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = 5;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;

        var fitter = row.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        return row;
    }

    public void DisplayCompletedSentence(PuzzleData puzzle)
    {
       
        // Clear previous sentence
        foreach (Transform child in sentencePanel)
            Destroy(child.gameObject);

        GameObject currentRow = CreateNewRow();
        float rowWidth = 0f;

        string[] tokens = puzzle.sentenceTemplate.Split(' ');
        int clueIndex = 0;
       
        foreach (string token in tokens)
        {
            GameObject element;

            if (token == "_" && clueIndex < puzzle.correctAnswers.Count)
            {
                // Create clue word prefab for the correct answer
                element = Instantiate(correctWordPrefab);
                var text = element.GetComponentInChildren<TMP_Text>();
                if (text != null)
                    text.text = puzzle.correctAnswers[clueIndex];
                clueIndex++;
               
            }
            else
            {
                // Regular word
                element = Instantiate(wordPrefab);
                var text = element.GetComponentInChildren<TMP_Text>();
                if (text != null)
                    text.text = token;
            }

            // Fit into current row
            RectTransform rect = element.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            float elementWidth = rect.rect.width;

            if (rowWidth + elementWidth > maxRowWidth)
            {
                currentRow = CreateNewRow();
                rowWidth = 0f;
            }

            element.transform.SetParent(currentRow.transform, false);
            rowWidth += elementWidth;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(sentencePanel);
    }
  
}
