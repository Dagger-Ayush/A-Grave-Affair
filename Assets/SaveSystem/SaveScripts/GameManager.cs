/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
      SaveLoadManager.Instance.SetGameInProgress(true);
        
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance.SetGameInProgress(false);
    }

    void Update()
    {
        


    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISavable
{
    public static GameManager Instance { get; private set; }

    [Header("Game Data")]
    public Vector3 playerPosition;
    public string currentScene;
    public List<ClueData> cluesFound = new List<ClueData>();
    public List<TaskData> tasksCompleted = new List<TaskData>();
    public List<LocationData> locationsUnlocked = new List<LocationData>();
    public List<string> suspectsInterrogated = new List<string>();
    public int decisionPoints;

    [Header("References")]
    [SerializeField] private string playerTag = "Player";
    private GameObject playerObject;
    private bool isLoading = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.RegisterHandler(this);
            Debug.Log("GameManager registered with save system");
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        SaveLoadManager.Instance.SetGameInProgress(true);

        if (SaveLoadManager.Instance.SavedState != null)
        {
            LoadGameData();
        }
        else
        {
            InitializeNewGame();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isLoading)
        {
            Debug.Log($"Scene loaded: {scene.name}, positioning player...");
            StartCoroutine(PositionPlayerAfterLoad());
            isLoading = false;
        }
    }

    public void PrepareForSave(SavedGamesState gameState)
    {
        Debug.Log("Preparing game data for save...");

        UpdatePlayerPosition();
        currentScene = SceneManager.GetActiveScene().name;

      
        gameState.GameData.LastUpdated = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        gameState.GameData.PlayerPosition = playerPosition;
        gameState.GameData.CurrentScene = currentScene;
        gameState.GameData.CluesFound = cluesFound;
        gameState.GameData.TasksCompleted = tasksCompleted;
        gameState.GameData.LocationsUnlocked = locationsUnlocked;
        gameState.GameData.SuspectsInterrogated = suspectsInterrogated;
        gameState.GameData.DecisionPoints = decisionPoints;

        Debug.Log($"Saved {cluesFound.Count} clues, {tasksCompleted.Count} tasks");
        Debug.Log($"Player position: {playerPosition}");
        Debug.Log($"Current scene: {currentScene}");
    }

    private void UpdatePlayerPosition()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag(playerTag);
        }

        if (playerObject != null)
        {
            playerPosition = playerObject.transform.position;
        }
    }

    private void LoadGameData()
    {
        var savedState = SaveLoadManager.Instance.SavedState;
        if (savedState?.GameData != null) 
        {
            Debug.Log("Loading game data from save...");

           
            playerPosition = savedState.GameData.PlayerPosition;
            currentScene = savedState.GameData.CurrentScene;
            cluesFound = savedState.GameData.CluesFound ?? new List<ClueData>();
            tasksCompleted = savedState.GameData.TasksCompleted ?? new List<TaskData>();
            locationsUnlocked = savedState.GameData.LocationsUnlocked ?? new List<LocationData>();
            suspectsInterrogated = savedState.GameData.SuspectsInterrogated ?? new List<string>();
            decisionPoints = savedState.GameData.DecisionPoints;

            Debug.Log($"Loaded: {cluesFound.Count} clues, {tasksCompleted.Count} tasks");
            Debug.Log($"Player position: {playerPosition}");
            Debug.Log($"Current scene: {currentScene}");

            string currentSceneName = SceneManager.GetActiveScene().name;
            if (!string.IsNullOrEmpty(currentScene) &&
                currentSceneName != currentScene)
            {
                Debug.Log($"Loading saved scene: {currentScene} (current: {currentSceneName})");
                isLoading = true;
                SceneManager.LoadScene(currentScene);
            }
            else
            {
                Debug.Log("Positioning player in current scene...");
                StartCoroutine(PositionPlayerAfterLoad());
            }
        }
    }

    private IEnumerator PositionPlayerAfterLoad()
    {
        yield return new WaitForSeconds(0.5f);

        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag(playerTag);
        }

        if (playerObject != null)
        {
            playerObject.transform.position = playerPosition;
            Debug.Log($"Player positioned at: {playerPosition}");
        }
        else
        {
            Debug.LogError("Player not found for positioning after load!");
        }
    }

    private void InitializeNewGame()
    {
        Debug.Log("Initializing new game...");
        playerPosition = Vector3.zero;
        currentScene = SceneManager.GetActiveScene().name;
        cluesFound = new List<ClueData>();
        tasksCompleted = new List<TaskData>();
        locationsUnlocked = new List<LocationData>();
        suspectsInterrogated = new List<string>();
        decisionPoints = 0;
    }

    
    public void AddClue(string clueID, string clueName = "")
    {
        var existingClue = cluesFound.Find(c => c.clueID == clueID);
        if (existingClue != null)
        {
            existingClue.found = true;
        }
        else
        {
            cluesFound.Add(new ClueData { clueID = clueID, found = true });
        }
        Debug.Log($"Clue added: {clueID}");
    }

    public void CompleteTask(string taskID)
    {
        var existingTask = tasksCompleted.Find(t => t.taskID == taskID);
        if (existingTask != null)
        {
            existingTask.completed = true;
        }
        else
        {
            tasksCompleted.Add(new TaskData { taskID = taskID, completed = true });
        }
        Debug.Log($"Task completed: {taskID}");
    }

    public void UnlockLocation(string locationID)
    {
        var existingLocation = locationsUnlocked.Find(l => l.locationID == locationID);
        if (existingLocation != null)
        {
            existingLocation.unlocked = true;
        }
        else
        {
            locationsUnlocked.Add(new LocationData { locationID = locationID, unlocked = true });
        }
        Debug.Log($"Location unlocked: {locationID}");
    }

    public void AddSuspect(string suspectID)
    {
        if (!suspectsInterrogated.Contains(suspectID))
        {
            suspectsInterrogated.Add(suspectID);
            Debug.Log($"Suspect interrogated: {suspectID}");
        }
    }

    public void AddDecisionPoints(int points)
    {
        decisionPoints += points;
        Debug.Log($"Decision points added: {points}. Total: {decisionPoints}");
    }

    public void ChangeScene(string sceneName)
    {
        currentScene = sceneName;
        SceneManager.LoadScene(sceneName);
    }

    public bool IsClueFound(string clueID)
    {
        var clue = cluesFound.Find(c => c.clueID == clueID);
        return clue != null && clue.found;
    }

    public bool IsTaskCompleted(string taskID)
    {
        var task = tasksCompleted.Find(t => t.taskID == taskID);
        return task != null && task.completed;
    }

    public bool IsLocationUnlocked(string locationID)
    {
        var location = locationsUnlocked.Find(l => l.locationID == locationID);
        return location != null && location.unlocked;
    }

    public void StartNewGame()
    {
        InitializeNewGame();
        SaveLoadManager.Instance.ClearSave();
        Debug.Log("New game started");
        SceneManager.LoadScene("MainScene");
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance.SetGameInProgress(false);

        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.DeregisterHandler(this);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISavable
{
    public static GameManager Instance { get; private set; }

    [Header("Game Data")]
    public Vector3 playerPosition;
    public string currentScene;
    public List<ClueData> cluesFound = new List<ClueData>();
    public List<TaskData> tasksCompleted = new List<TaskData>();
    public List<LocationData> locationsUnlocked = new List<LocationData>();
    public List<string> suspectsInterrogated = new List<string>();
    public int decisionPoints;

    [Header("References")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string defaultScene = "MainScene";
    private GameObject playerObject;
    private bool isLoading = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.RegisterHandler(this);
            Debug.Log("GameManager registered with save system");
        }
        else
        {
            Debug.LogError("SaveLoadManager instance is null in Awake!");
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.SetGameInProgress(true);

            if (SaveLoadManager.Instance.SavedState != null)
            {
                Debug.Log("Save data found, loading game...");
                LoadGameData();
            }
            else
            {
                Debug.Log("No save data found, initializing new game...");
                InitializeNewGame();
            }
        }
        else
        {
            Debug.LogError("SaveLoadManager instance not found in Start!");
            InitializeNewGame();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}, isLoading: {isLoading}");

        if (isLoading)
        {
            Debug.Log($"Loading game data after scene load: {scene.name}");

           
            var savedState = SaveLoadManager.Instance?.SavedState;
            if (savedState?.GameData != null)
            {
                LoadGameDataImmediately(savedState);
            }
            else
            {
                Debug.LogError("No saved state found after scene load!");
            }

            isLoading = false;
        }
    }


    public void PrepareForSave(SavedGamesState gameState)
    {
        Debug.Log("Preparing game data for save...");

        UpdatePlayerPosition();

     
        string activeScene = SceneManager.GetActiveScene().name;
        if (string.IsNullOrEmpty(activeScene))
        {
            activeScene = defaultScene;
            Debug.LogError($"Active scene name is null! Using default: {defaultScene}");
        }

        currentScene = activeScene;

      
        if (gameState.GameData == null)
        {
            gameState.GameData = new SavedGamesState.GameDataState();
        }

        // Set the data directly - don't rely on the handler chain
        gameState.GameData.LastUpdated = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        gameState.GameData.PlayerPosition = new SerializableVector3(playerPosition);
        gameState.GameData.CurrentScene = currentScene;
        gameState.GameData.CluesFound = cluesFound ?? new List<ClueData>();
        gameState.GameData.TasksCompleted = tasksCompleted ?? new List<TaskData>();
        gameState.GameData.LocationsUnlocked = locationsUnlocked ?? new List<LocationData>();
        gameState.GameData.SuspectsInterrogated = suspectsInterrogated ?? new List<string>();
        gameState.GameData.DecisionPoints = decisionPoints;

        Debug.Log($"=== SAVE DATA ===");
        Debug.Log($"Scene: {currentScene}");
        Debug.Log($"Player Position: {playerPosition}");
        Debug.Log($"Clues: {cluesFound?.Count ?? 0}");
        Debug.Log($"Tasks: {tasksCompleted?.Count ?? 0}");
        Debug.Log($"=== END SAVE DATA ===");
    }
    private void UpdatePlayerPosition()
    {
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag(playerTag);
        }

        if (playerObject != null)
        {
            playerPosition = playerObject.transform.position;
            Debug.Log($"Updated player position: {playerPosition}");
        }
        else
        {
            Debug.LogWarning("Player object not found for position update!");
        }
    }

    private void LoadGameData()
    {
        var savedState = SaveLoadManager.Instance.SavedState;
        if (savedState?.GameData != null)
        {
            Debug.Log("Loading game data from save...");

         
            string currentSceneName = SceneManager.GetActiveScene().name;
            string savedSceneName = savedState.GameData.CurrentScene;

            if (string.IsNullOrEmpty(savedSceneName))
            {
                savedSceneName = defaultScene;
                Debug.LogWarning($"Saved scene name was null, using default: {defaultScene}");
            }

            Debug.Log($"Current scene: {currentSceneName}, Saved scene: {savedSceneName}");

            if (currentSceneName != savedSceneName)
            {
                Debug.Log($"Loading saved scene: {savedSceneName} (current: {currentSceneName})");
                isLoading = true;

              
                SceneManager.LoadScene(savedSceneName);
            }
            else
            {
               
                Debug.Log("Already in correct scene, loading data immediately");
                LoadGameDataImmediately(savedState);
            }
        }
        else
        {
            Debug.LogError("SavedState or GameData is null in LoadGameData!");
        }
    }

    private void LoadGameDataImmediately(SavedGamesState savedState)
    {
        try
        {
          
            if (savedState.GameData.PlayerPosition != null)
            {
                playerPosition = savedState.GameData.PlayerPosition.ToVector3();
            }
            else
            {
                playerPosition = Vector3.zero;
                Debug.LogWarning("PlayerPosition was null in save, using default position");
            }

            currentScene = savedState.GameData.CurrentScene ?? defaultScene;
            cluesFound = savedState.GameData.CluesFound ?? new List<ClueData>();
            tasksCompleted = savedState.GameData.TasksCompleted ?? new List<TaskData>();
            locationsUnlocked = savedState.GameData.LocationsUnlocked ?? new List<LocationData>();
            suspectsInterrogated = savedState.GameData.SuspectsInterrogated ?? new List<string>();
            decisionPoints = savedState.GameData.DecisionPoints;

            Debug.Log($"Loaded: {cluesFound.Count} clues, {tasksCompleted.Count} tasks");
            Debug.Log($"Player position: {playerPosition}");
            Debug.Log($"Current scene: {currentScene}");

           
            StartCoroutine(PositionPlayerAfterLoad());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in LoadGameDataImmediately: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }

    private IEnumerator PositionPlayerAfterLoad()
    {
        yield return new WaitForSeconds(0.1f); 

        int attempts = 0;
        while (playerObject == null && attempts < 10)
        {
            playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject == null)
            {
                Debug.Log($"Attempt {attempts + 1}: Player not found, waiting...");
                yield return new WaitForSeconds(0.1f);
                attempts++;
            }
        }

        if (playerObject != null)
        {
            playerObject.transform.position = playerPosition;
            Debug.Log($"Player positioned at: {playerPosition}");
        }
        else
        {
            Debug.LogError("Player not found for positioning after load!");
        }
    }

    private void InitializeNewGame()
    {
        Debug.Log("Initializing new game...");
        playerPosition = Vector3.zero;
        currentScene = SceneManager.GetActiveScene().name;

        // Ensure currentScene is not null
        if (string.IsNullOrEmpty(currentScene))
        {
            currentScene = defaultScene;
        }

        cluesFound = new List<ClueData>();
        tasksCompleted = new List<TaskData>();
        locationsUnlocked = new List<LocationData>();
        suspectsInterrogated = new List<string>();
        decisionPoints = 0;

        Debug.Log($"New game initialized in scene: {currentScene}");
    }

    private void OnDestroy()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.SetGameInProgress(false);
            SaveLoadManager.Instance.DeregisterHandler(this);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}