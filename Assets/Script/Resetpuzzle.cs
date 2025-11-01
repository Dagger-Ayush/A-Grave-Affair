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

}
