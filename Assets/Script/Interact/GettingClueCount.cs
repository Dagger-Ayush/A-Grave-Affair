using System.Collections.Generic;
using UnityEngine;

public class GettingClueCount : MonoBehaviour
{
    public List<GameObject> clueBackgrounds = new(); // background objects (notes)
    public List<GameObject> clueTicks = new();       // tick objects

    public void ShowLineBackgrounds(int totalCluesThisLine)
    {
        totalCluesThisLine = Mathf.Clamp(totalCluesThisLine, 0, clueBackgrounds.Count);

        for (int i = 0; i < clueBackgrounds.Count; i++)
            clueBackgrounds[i].SetActive(i < totalCluesThisLine);
    }

    public void AddTick(int currentTickCount, int totalCluesThisLine)
    {
        currentTickCount = Mathf.Clamp(currentTickCount, 0, clueTicks.Count);
        totalCluesThisLine = Mathf.Clamp(totalCluesThisLine, 0, clueBackgrounds.Count);

        if (totalCluesThisLine <= 0)
        {
            DisableAll();
            return;
        }

        // Always show correct backgrounds
        ShowLineBackgrounds(totalCluesThisLine);

        // Enable ticks up to current count
        for (int i = 0; i < clueTicks.Count; i++)
            clueTicks[i].SetActive(i < currentTickCount);
    }

    public void UpdateTick(int currentTickCount)
    {
        for (int i = 0; i < clueTicks.Count; i++)
            clueTicks[i].SetActive(i < currentTickCount);
    }

    public void ResetTicks(int totalCluesThisLine)
    {
        // Keep backgrounds for current clue line
        ShowLineBackgrounds(totalCluesThisLine);

        // Clear all ticks
        foreach (var tick in clueTicks)
            tick.SetActive(false);
    }

    public void DisableAll()
    {
        foreach (var bg in clueBackgrounds)
            bg.SetActive(false);

        foreach (var tick in clueTicks)
            tick.SetActive(false);
    }
}
