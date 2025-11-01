/*
    using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Resetpuzzle : MonoBehaviour
{
    public List<DialogManager> dialogManagers;

    public void ResetPuzzleCompletion()
    {
        // ✅ Step 1: Reset dialog data
        foreach (DialogManager dialog in dialogManagers)
            dialog.ResetClues();

        // ✅ Step 2: Delete PlayerPrefs keys
        string[] prefsToDelete = {
        "DressInteractionDone",
        "EnteredOutsideDoor",
        "SavedMotelNancyMusicTime",
        "SavedMotelNancyMusicClip",
        "SavedSceneName",
        "Global_ClueCount",
        "AllObjectIDs"
    };

        foreach (string key in prefsToDelete)
        {
            if (PlayerPrefs.HasKey(key))
                PlayerPrefs.DeleteKey(key);
        }

        // ✅ Step 3: Delete all clue counts from PlayerPrefs
        for (int i = 1; i <= 200; i++)
        {
            string clueKey = i + "_ClueCount";
            if (PlayerPrefs.HasKey(clueKey))
                PlayerPrefs.DeleteKey(clueKey);
        }

        PlayerPrefs.Save();

        // ✅ Step 4: Delete from memory (RAM)
        if (ClueProgressManager.Instance != null)
        {
            ClueProgressManager mgr = ClueProgressManager.Instance;
            mgr.clueCount = 0;
            mgr.clueCountMain = 0;
            ClueProgressManager.WasJustReset = true;
            // 🧠 Clear dictionary holding cached clues
            var field = typeof(ClueProgressManager)
                .GetField("clueProgress", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                var dict = field.GetValue(mgr) as Dictionary<string, int>;
                dict?.Clear();
            }

            Debug.Log("🧹 Cleared ClueProgressManager memory and cache!");
        }

        // ✅ Step 5: Force unload ClueProgressManager from memory
        if (ClueProgressManager.Instance != null)
        {
            Destroy(ClueProgressManager.Instance.gameObject);
            ClueProgressManager.Instance = null;
            Debug.Log("💥 ClueProgressManager fully removed from memory!");
        }

        // ✅ Step 6: Save state
        PlayerPrefs.Save();

        Debug.Log("✅ Everything reset — PlayerPrefs + memory cleared!");
    }

}*/
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Resetpuzzle : MonoBehaviour
{
    [Tooltip("Optional manually assigned dialog managers (will auto-find too).")]
    public List<DialogManager> dialogManagers;

    public void ResetPuzzleCompletion()
    {
        StartCoroutine(PerformResetDelayed());
    }

    private System.Collections.IEnumerator PerformResetDelayed()
    {
        // 🕐 Wait one frame to ensure all managers are initialized
        yield return null;

        // ✅ Step 1: Gather all active DialogManagers (auto + manual)
        var foundManagers = FindObjectsOfType<DialogManager>(true).ToList();
        if (dialogManagers != null)
            foundManagers.AddRange(dialogManagers);

        // Remove duplicates + destroyed entries
        foundManagers = foundManagers
            .Where(dm => dm != null)
            .Distinct()
            .ToList();

        // ✅ Step 2: Reset dialog data safely
        foreach (var dialog in foundManagers)
        {
            try
            {
                dialog.ResetClues();
                Debug.Log($"🧩 Reset clues in: {dialog.name}");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"⚠️ Failed to reset {dialog.name}: {ex.Message}");
            }
        }

        // ✅ Step 3: Delete PlayerPrefs keys
        string[] prefsToDelete = {
            "DressInteractionDone",
            "EnteredOutsideDoor",
            "SavedMotelNancyMusicTime",
            "SavedMotelNancyMusicClip",
            "SavedSceneName",
            "Global_ClueCount",
            "AllObjectIDs"
        };

        foreach (string key in prefsToDelete)
        {
            if (PlayerPrefs.HasKey(key))
                PlayerPrefs.DeleteKey(key);
        }

        // ✅ Step 4: Delete all clue counts
        for (int i = 1; i <= 200; i++)
        {
            string clueKey = i + "_ClueCount";
            if (PlayerPrefs.HasKey(clueKey))
                PlayerPrefs.DeleteKey(clueKey);
        }

        PlayerPrefs.Save();

        // ✅ Step 5: Clear ClueProgressManager memory
        if (ClueProgressManager.Instance != null)
        {
            ClueProgressManager mgr = ClueProgressManager.Instance;
            mgr.clueCount = 0;
            mgr.clueCountMain = 0;
            ClueProgressManager.WasJustReset = true;

            // 🧠 Clear private dictionary cache
            var field = typeof(ClueProgressManager)
                .GetField("clueProgress", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                var dict = field.GetValue(mgr) as Dictionary<string, int>;
                dict?.Clear();
            }

            Debug.Log("🧹 Cleared ClueProgressManager memory and cache!");
        }

        // ✅ Step 6: Fully destroy manager instance
        if (ClueProgressManager.Instance != null)
        {
            Destroy(ClueProgressManager.Instance.gameObject);
            ClueProgressManager.Instance = null;
            Debug.Log("💥 ClueProgressManager fully removed from memory!");
        }

        PlayerPrefs.Save();
        Debug.Log("✅ Everything reset — DialogManagers, PlayerPrefs, and memory cleared!");
    }
}
