using System.Collections.Generic;
using UnityEngine;

public class ClueProgressManager : MonoBehaviour
{
    public static ClueProgressManager Instance;

    public int clueCount = 0;
    public int clueCountMain = 0;

    private Dictionary<string, int> clueProgress = new Dictionary<string, int>();
    private const string REGISTRY_KEY = "ClueKeysRegistry";
    public static bool WasJustReset = false;
    // existing code...
    public static bool SuppressSave = false;

    public void SaveToPlayerPrefs()
    {
        if (SuppressSave)
        {
            Debug.Log("ClueProgressManager: Save suppressed.");
            return;
        }
        // ...actual saving logic
    }

    private void OnDisable()
    {
        if (!SuppressSave)
            SaveToPlayerPrefs();
        else
            Debug.Log("ClueProgressManager: OnDisable save suppressed.");
    }

    private void OnDestroy()
    {
        if (!SuppressSave)
            SaveToPlayerPrefs();
        else
            Debug.Log("ClueProgressManager: OnDestroy save suppressed.");
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllClueProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ✅ Save a single object's clue count
    public void SaveClueCount(string objectID, int count)
    {
        if (string.IsNullOrEmpty(objectID)) return;

        clueProgress[objectID] = count;
        string key = objectID + "_ClueCount";

        PlayerPrefs.SetInt(key, count);
        RegisterClueKey(key);
        PlayerPrefs.Save();

        Debug.Log($"💾 Saved clue count for {objectID} = {count} (key: {key})");
    }

    // ✅ Get saved clue count
    public int GetClueCount(string objectID)
    {
        if (string.IsNullOrEmpty(objectID)) return 0;

        if (clueProgress.ContainsKey(objectID))
            return clueProgress[objectID];

        int savedCount = PlayerPrefs.GetInt(objectID + "_ClueCount", 0);
        clueProgress[objectID] = savedCount;
        return savedCount;
    }

    // ✅ Save all currently tracked clues
    public void SaveAllClueProgress()
    {
        foreach (var kvp in clueProgress)
        {
            string key = kvp.Key + "_ClueCount";
            PlayerPrefs.SetInt(key, kvp.Value);
            RegisterClueKey(key);
        }
        PlayerPrefs.Save();
        Debug.Log("💾 All clue progress saved.");
    }

    // ✅ Load (initialize)
    private void LoadAllClueProgress()
    {
        clueProgress.Clear();
        Debug.Log("📦 ClueProgressManager initialized.");
    }

    // ✅ Registry helper (tracks all clue keys that exist)
    private void RegisterClueKey(string key)
    {
        if (string.IsNullOrEmpty(key)) return;

        string saved = PlayerPrefs.GetString(REGISTRY_KEY, "");
        var set = new HashSet<string>(
            saved.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)
        );

        if (!set.Contains(key))
        {
            set.Add(key);
            PlayerPrefs.SetString(REGISTRY_KEY, string.Join(",", set));
            PlayerPrefs.Save();
        }
    }

    // ✅ NEW: Full reset of all clues, safe across scenes
    public void ResetAllClues()
    {
        Debug.Log("🧹 ResetAllClues() started...");

        // Get all keys from registry
        string saved = PlayerPrefs.GetString(REGISTRY_KEY, "");
        if (!string.IsNullOrEmpty(saved))
        {
            var keys = saved.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var key in keys)
            {
                PlayerPrefs.DeleteKey(key);
                Debug.Log($"🗑️ Deleted clue key: {key}");
            }
        }

        // Remove registry entry itself
        PlayerPrefs.DeleteKey(REGISTRY_KEY);

        // Clear runtime memory
        clueProgress.Clear();
        clueCount = 0;
        clueCountMain = 0;

        PlayerPrefs.Save();
        Debug.Log("✅ All clue data (registry + memory) cleared!");
    }
}
