using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Puzzle", menuName = "Puzzle Data")]
public class PuzzleData : ScriptableObject
{
    [TextArea] public string sentenceTemplate;
    public List<string> correctAnswers;
    public List<string> requiredClues;
}
