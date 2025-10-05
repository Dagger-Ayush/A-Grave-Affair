/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class SavedGamesState
{
    public int Version = 1;

    public class SimpleSpawnerState
    {
        public class Entry
        {
            public PrimitiveType Type;
            public System.Tuple<float, float, float> Location;
        }

        public string ID;
        public List<Entry> SpawnedObjects = new List<Entry>();
    }
     
    public SimpleSpawnerState SpawnerState = new SimpleSpawnerState();

}

public enum ESaveSlot
{
    None,

    Slot1,
    Slot2,
    Slot3,
    Slot4,
    Slot5,

}

public enum ESaveType
{
    Manual,
    Automatic

}

public interface ISavable
{
    void PrepareForSave(SavedGamesState gameState);
}

public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] float AutoSaveInterval= 300f;
    
   
    public static SaveLoadManager Instance{ get; private set; }=null;

    public SavedGamesState SavedState { get; private set; } = null;
    ESaveSlot CurrentSlot = ESaveSlot.None;

    List<ISavable> SaveHandlers = new List<ISavable>();
    float TimeUntilNextAutosave = 0f;

    bool GameInProgress = false;

    public bool HasSavedGames
    {
        get
        {
            var allSlots = System.Enum.GetValues(typeof(ESaveSlot));
            foreach(var slot in allSlots)
            {
                var slotEnum = (ESaveSlot)slot;

                if(slotEnum == ESaveSlot.None)
                {
                    continue;
                }
                if (DoesSaveExist(slotEnum, ESaveType.Manual))
                {
                    return true;
                }
                if (DoesSaveExist(slotEnum, ESaveType.Automatic))
                {
                    return true;
                }
            }
            return false;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadPersistentLevel()
    {
        for(int sceneIndex = 0;sceneIndex < SceneManager.sceneCount; ++sceneIndex)
        {
           if(SceneManager.GetSceneAt(sceneIndex).name == "SaveLoadPersistentLevel")
            {
                return;
            }
        }

        SceneManager.LoadScene("SaveLoadPersistentLevel", LoadSceneMode.Additive);
    }

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError($"Found a duplicate SaveLoadManager on {gameObject.name}");
            Destroy(gameObject);
            return;

        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        AutoSaveInterval = 10f;

        RequestLoad(ESaveSlot.Slot1, ESaveType.Manual);
    }
    void Start()
    {
        
    }

    public void SetGameInProgress(bool newValue)
    {
        GameInProgress = newValue;
    }
    void Update()
    {
        if(SavedState != null && GameInProgress)
        {
            TimeUntilNextAutosave -= Time.deltaTime;
            if(TimeUntilNextAutosave <= 0)
            {
                TimeUntilNextAutosave = AutoSaveInterval;

                RequestSave(CurrentSlot,ESaveType.Automatic);
            }
        }
        
    }

    public void RegisterHandler(ISavable handler)
    {
        if(SaveHandlers.Contains(handler))
           SaveHandlers.Add(handler);
        
    }
    public void DeregisterHandler(ISavable handler)
    {
        SaveHandlers.Remove(handler);
    }

    public string GetLastSavedTime(ESaveSlot slot, ESaveType saveType)
    {
        var lastSavedTime = File.GetLastWriteTime(GetSaveFilePath(slot,saveType));

        return $"{lastSavedTime.ToLongDateString()} @ {lastSavedTime.ToLongTimeString()}";
    }

   
    private string GetSaveFilePath(ESaveSlot slot, ESaveType saveType)
    {
        return Path.Combine(Application.persistentDataPath, $"SaveFile_Slot{(int)slot}_{saveType}.json");
    }

    public void RequestSave(ESaveSlot slot,ESaveType saveType)
    {
        SavedGamesState savedState = new SavedGamesState();

        //populate the savestate
        foreach(var handler in SaveHandlers)
        {
            if(handler == null) continue;

            handler.PrepareForSave(savedState);
        }

        var filePath = GetSaveFilePath(slot,saveType);
        
        File.WriteAllText(filePath,JsonConvert.SerializeObject(savedState,Formatting.Indented));
    }

    public bool DoesSaveExist(ESaveSlot slot, ESaveType saveType)
    {
        return File.Exists(GetSaveFilePath(slot,saveType));
    }
    public void RequestLoad(ESaveSlot slot,ESaveType saveType)
    {
        var filePath = GetSaveFilePath(slot, saveType);

        CurrentSlot = slot;
        SavedState = JsonConvert.DeserializeObject<SavedGamesState>(File.ReadAllText(filePath));
        TimeUntilNextAutosave = AutoSaveInterval;
    }

    public void ClearSave()
    {
        SavedState = null;
        CurrentSlot = ESaveSlot.None;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ClueData
{
    public string clueID;
    public bool found;
}

[System.Serializable]
public class TaskData
{
    public string taskID;
    public bool completed;
}

[System.Serializable]
public class LocationData
{
    public string locationID;
    public bool unlocked;
}

public class SavedGamesState
{
    public int Version = 1;

    [System.Serializable]
    public class GameDataState
    {
        public long LastUpdated;
        public Vector3 PlayerPosition;
        public string CurrentScene;
        public List<ClueData> CluesFound;
        public List<TaskData> TasksCompleted;
        public List<LocationData> LocationsUnlocked;
        public List<string> SuspectsInterrogated;
        public int DecisionPoints;
    }

    // FIX: Renamed the field to avoid ambiguity
    public GameDataState GameData = new GameDataState();
}

public enum ESaveSlot
{
    None,
    Slot1,
    Slot2,
    Slot3,
    Slot4,
    Slot5,
}

public enum ESaveType
{
    Manual,
    Automatic
}

public interface ISavable
{
    void PrepareForSave(SavedGamesState gameState);
}

// Main SaveLoadManager class
public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] float AutoSaveInterval = 300f;

    public static SaveLoadManager Instance { get; private set; } = null;

    public SavedGamesState SavedState { get; private set; } = null;
    ESaveSlot CurrentSlot = ESaveSlot.None;

    List<ISavable> SaveHandlers = new List<ISavable>();
    float TimeUntilNextAutosave = 0f;
    bool GameInProgress = false;

    public bool HasSavedGames
    {
        get
        {
            var allSlots = System.Enum.GetValues(typeof(ESaveSlot));
            foreach (var slot in allSlots)
            {
                var slotEnum = (ESaveSlot)slot;

                if (slotEnum == ESaveSlot.None)
                {
                    continue;
                }
                if (DoesSaveExist(slotEnum, ESaveType.Manual))
                {
                    return true;
                }
                if (DoesSaveExist(slotEnum, ESaveType.Automatic))
                {
                    return true;
                }
            }
            return false;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadPersistentLevel()
    {
        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; ++sceneIndex)
        {
            if (SceneManager.GetSceneAt(sceneIndex).name == "SaveLoadPersistentLevel")
            {
                return;
            }
        }

        SceneManager.LoadScene("SaveLoadPersistentLevel", LoadSceneMode.Additive);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Found a duplicate SaveLoadManager on {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;

        Debug.Log("SaveLoadManager initialized");
    }

    void Update()
    {
        if (SavedState != null && GameInProgress)
        {
            TimeUntilNextAutosave -= Time.deltaTime;
            if (TimeUntilNextAutosave <= 0)
            {
                TimeUntilNextAutosave = AutoSaveInterval;
                RequestSave(CurrentSlot, ESaveType.Automatic);
                Debug.Log("Auto-saved game");
            }
        }
    }

    public void SetGameInProgress(bool newValue)
    {
        GameInProgress = newValue;
        if (newValue)
        {
            TimeUntilNextAutosave = AutoSaveInterval;
        }
    }

    public void RegisterHandler(ISavable handler)
    {
        if (!SaveHandlers.Contains(handler))
        {
            SaveHandlers.Add(handler);
            Debug.Log($"Registered save handler: {handler.GetType().Name}");
        }
    }

    public void DeregisterHandler(ISavable handler)
    {
        SaveHandlers.Remove(handler);
        Debug.Log($"Deregistered save handler: {handler.GetType().Name}");
    }

    public string GetLastSavedTime(ESaveSlot slot, ESaveType saveType)
    {
        var filePath = GetSaveFilePath(slot, saveType);
        if (File.Exists(filePath))
        {
            var lastSavedTime = File.GetLastWriteTime(filePath);
            return $"{lastSavedTime.ToShortDateString()} {lastSavedTime.ToShortTimeString()}";
        }
        return "Never";
    }

    private string GetSaveFilePath(ESaveSlot slot, ESaveType saveType)
    {
        return Path.Combine(Application.persistentDataPath, $"SaveFile_Slot{(int)slot}_{saveType}.json");
    }

    public void RequestSave(ESaveSlot slot, ESaveType saveType)
    {
        SavedGamesState savedState = new SavedGamesState();

        Debug.Log($"Saving game to slot {(int)slot}, type: {saveType}");
        Debug.Log($"Number of save handlers: {SaveHandlers.Count}");

        foreach (var handler in SaveHandlers)
        {
            if (handler == null)
            {
                Debug.LogWarning("Found null handler in SaveHandlers");
                continue;
            }

            try
            {
                handler.PrepareForSave(savedState);
                Debug.Log($"Handler {handler.GetType().Name} prepared data for save");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error in handler {handler.GetType().Name}: {e.Message}");
            }
        }

        var filePath = GetSaveFilePath(slot, saveType);

        try
        {
            string jsonData = JsonConvert.SerializeObject(savedState, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
            CurrentSlot = slot;
            Debug.Log($"Game saved successfully to: {filePath}");

            SavedState = savedState;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    public bool DoesSaveExist(ESaveSlot slot, ESaveType saveType)
    {
        return File.Exists(GetSaveFilePath(slot, saveType));
    }

    public void RequestLoad(ESaveSlot slot, ESaveType saveType)
    {
        var filePath = GetSaveFilePath(slot, saveType);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"No save file found at: {filePath}");
            return;
        }

        try
        {
            string jsonData = File.ReadAllText(filePath);
            SavedState = JsonConvert.DeserializeObject<SavedGamesState>(jsonData);
            CurrentSlot = slot;
            TimeUntilNextAutosave = AutoSaveInterval;

            Debug.Log($"Game loaded successfully from: {filePath}");
            Debug.Log($"Loaded scene: {SavedState.GameData.CurrentScene}"); // FIXED: Changed from GameDataState to GameData
            Debug.Log($"Player position: {SavedState.GameData.PlayerPosition}"); // FIXED: Changed from GameDataState to GameData

            if (GameManager.Instance != null)
            {
                Debug.Log("GameManager instance found, it will handle the loaded data");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
        }
    }

    public void ClearSave()
    {
        SavedState = null;
        CurrentSlot = ESaveSlot.None;
        Debug.Log("Save data cleared");
    }

    public void DeleteSave(ESaveSlot slot, ESaveType saveType)
    {
        var filePath = GetSaveFilePath(slot, saveType);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Deleted save file: {filePath}");
        }
    }
}*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ClueData
{
    public string clueID;
    public bool found;
}

[System.Serializable]
public class TaskData
{
    public string taskID;
    public bool completed;
}

[System.Serializable]
public class LocationData
{
    public string locationID;
    public bool unlocked;
}

public class SavedGamesState
{
    public int Version = 1;

    [System.Serializable]
    public class GameDataState
    {
        public long LastUpdated;
        public SerializableVector3 PlayerPosition;
        public string CurrentScene;
        public List<ClueData> CluesFound;
        public List<TaskData> TasksCompleted;
        public List<LocationData> LocationsUnlocked;
        public List<string> SuspectsInterrogated;
        public int DecisionPoints;
    }

    public GameDataState GameData = new GameDataState();
}

public enum ESaveSlot
{
    None,
    Slot1,
    Slot2,
    Slot3,
    Slot4,
    Slot5,
}

public enum ESaveType
{
    Manual,
    Automatic
}

public interface ISavable
{
    void PrepareForSave(SavedGamesState gameState);
}

// Main SaveLoadManager class
public class SaveLoadManager : MonoBehaviour
{
    [SerializeField] float AutoSaveInterval = 300f;

    public static SaveLoadManager Instance { get; private set; } = null;

    public SavedGamesState SavedState { get; private set; } = null;
    ESaveSlot CurrentSlot = ESaveSlot.None;

    List<ISavable> SaveHandlers = new List<ISavable>();
    float TimeUntilNextAutosave = 0f;
    bool GameInProgress = false;

    public bool HasSavedGames
    {
        get
        {
            var allSlots = System.Enum.GetValues(typeof(ESaveSlot));
            foreach (var slot in allSlots)
            {
                var slotEnum = (ESaveSlot)slot;

                if (slotEnum == ESaveSlot.None)
                {
                    continue;
                }
                if (DoesSaveExist(slotEnum, ESaveType.Manual))
                {
                    return true;
                }
                if (DoesSaveExist(slotEnum, ESaveType.Automatic))
                {
                    return true;
                }
            }
            return false;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadPersistentLevel()
    {
        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; ++sceneIndex)
        {
            if (SceneManager.GetSceneAt(sceneIndex).name == "SaveLoadPersistentLevel")
            {
                return;
            }
        }

        SceneManager.LoadScene("SaveLoadPersistentLevel", LoadSceneMode.Additive);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Found a duplicate SaveLoadManager on {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;

        Debug.Log("SaveLoadManager initialized");
    }

    void Update()
    {
        if (SavedState != null && GameInProgress)
        {
            TimeUntilNextAutosave -= Time.deltaTime;
            if (TimeUntilNextAutosave <= 0)
            {
                TimeUntilNextAutosave = AutoSaveInterval;
                RequestSave(CurrentSlot, ESaveType.Automatic);
                Debug.Log("Auto-saved game");
            }
        }
    }

    public void SetGameInProgress(bool newValue)
    {
        GameInProgress = newValue;
        if (newValue)
        {
            TimeUntilNextAutosave = AutoSaveInterval;
        }
    }

    public void RegisterHandler(ISavable handler)
    {
        if (!SaveHandlers.Contains(handler))
        {
            SaveHandlers.Add(handler);
            Debug.Log($"Registered save handler: {handler.GetType().Name}");
        }
    }

    public void DeregisterHandler(ISavable handler)
    {
        SaveHandlers.Remove(handler);
        Debug.Log($"Deregistered save handler: {handler.GetType().Name}");
    }

    public string GetLastSavedTime(ESaveSlot slot, ESaveType saveType)
    {
        var filePath = GetSaveFilePath(slot, saveType);
        if (File.Exists(filePath))
        {
            var lastSavedTime = File.GetLastWriteTime(filePath);
            return $"{lastSavedTime.ToShortDateString()} {lastSavedTime.ToShortTimeString()}";
        }
        return "Never";
    }

    private string GetSaveFilePath(ESaveSlot slot, ESaveType saveType)
    {
        return Path.Combine(Application.persistentDataPath, $"SaveFile_Slot{(int)slot}_{saveType}.json");
    }

    public void RequestSave(ESaveSlot slot, ESaveType saveType)
    {
        SavedGamesState savedState = new SavedGamesState();

        Debug.Log($"Saving game to slot {(int)slot}, type: {saveType}");
        Debug.Log($"Number of save handlers: {SaveHandlers.Count}");

        savedState.GameData.CurrentScene = SceneManager.GetActiveScene().name;
        savedState.GameData.LastUpdated = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        Debug.Log($"Base scene saved: {savedState.GameData.CurrentScene}");

    
        foreach (var handler in SaveHandlers)
        {
            if (handler == null)
            {
                Debug.LogWarning("Found null handler in SaveHandlers");
                continue;
            }

            try
            {
                handler.PrepareForSave(savedState);
                Debug.Log($"Handler {handler.GetType().Name} prepared data for save");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error in handler {handler.GetType().Name}: {e.Message}");
            }
        }

        var filePath = GetSaveFilePath(slot, saveType);

        try
        {
            
            if (string.IsNullOrEmpty(savedState.GameData.CurrentScene))
            {
                savedState.GameData.CurrentScene = SceneManager.GetActiveScene().name;
                Debug.LogWarning($"Scene was still null, re-set to: {savedState.GameData.CurrentScene}");
            }

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            string jsonData = JsonConvert.SerializeObject(savedState, settings);
            File.WriteAllText(filePath, jsonData);
            CurrentSlot = slot;

            Debug.Log($"Game saved successfully to: {filePath}");
            Debug.Log($"Final saved scene: {savedState.GameData.CurrentScene}");
            Debug.Log($"Saved data: {jsonData}"); 

            SavedState = savedState;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    public bool DoesSaveExist(ESaveSlot slot, ESaveType saveType)
    {
        return File.Exists(GetSaveFilePath(slot, saveType));
    }

    public void RequestLoad(ESaveSlot slot, ESaveType saveType)
    {
        var filePath = GetSaveFilePath(slot, saveType);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"No save file found at: {filePath}");
            return;
        }

        try
        {
            string jsonData = File.ReadAllText(filePath);
            SavedState = JsonConvert.DeserializeObject<SavedGamesState>(jsonData);
            CurrentSlot = slot;
            TimeUntilNextAutosave = AutoSaveInterval;

            Debug.Log($"Game loaded successfully from: {filePath}");

            if (SavedState?.GameData != null)
            {
                Debug.Log($"Loaded scene: {SavedState.GameData.CurrentScene ?? "NULL"}");
                Debug.Log($"Player position: {SavedState.GameData.PlayerPosition?.ToVector3()}");

                if (string.IsNullOrEmpty(SavedState.GameData.CurrentScene))
                {
                    SavedState.GameData.CurrentScene = SceneManager.GetActiveScene().name;
                    Debug.LogWarning("CurrentScene was null, set to current scene: " + SavedState.GameData.CurrentScene);
                }
            }
            else
            {
                Debug.LogError("SavedState or GameData is null after loading!");
            }

            if (GameManager.Instance != null)
            {
                Debug.Log("GameManager instance found, it will handle the loaded data");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    public void ClearSave()
    {
        SavedState = null;
        CurrentSlot = ESaveSlot.None;
        Debug.Log("Save data cleared");
    }

    public void DeleteSave(ESaveSlot slot, ESaveType saveType)
    {
        var filePath = GetSaveFilePath(slot, saveType);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Deleted save file: {filePath}");
        }
    }
}