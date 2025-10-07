using UnityEngine;

public class PosandAnimationUpdate : MonoBehaviour
{
    public static PosandAnimationUpdate Instance { get; private set; }

    [Header("Phase_1")]
    public GameObject[] charObjects;
    public GameObject[] targetposition_1;
    public AnimationClip[] newAnimationClips_1;
    public Animator[] charAnimators; // same animators for both phases

    [Header("Phase_2")]
    public GameObject[] targetposition_2;
    public AnimationClip[] newAnimationClips_2;

    [Header("Phase_2 Player Reference")]
    public GameObject player;
    public GameObject playerTarget_2;

    [Header("Phase_2")]
    public GameObject[] targetposition_3;
    public AnimationClip[] newAnimationClips_3;
    private void Awake()
    {
        Instance = this;
    }

    // --- PHASE 1 ---
    public void UpdatePhase_1()
    {
        UpdatePhase(charObjects, targetposition_1, charAnimators, newAnimationClips_1);
    }

    // --- PHASE 2 ---
    public void UpdatePhase_2()
    {
        // Update characters
        UpdatePhase(charObjects, targetposition_2, charAnimators, newAnimationClips_2);

        // Update player
        UpdatePlayerPhase_2();
    }
    public void UpdatePhase_3()
    {
        // Update characters
        UpdatePhase(charObjects, targetposition_3, charAnimators, newAnimationClips_3);

       
    }
    // --- Shared Logic for Characters ---
    private void UpdatePhase(GameObject[] chars, GameObject[] targets, Animator[] animators, AnimationClip[] clips)
    {
        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == null || i >= targets.Length || targets[i] == null)
                continue;

            // --- Parent to target ---
            chars[i].transform.SetParent(targets[i].transform);

            // --- Snap instantly to target position and rotation ---
            chars[i].transform.position = targets[i].transform.position;
            chars[i].transform.rotation = targets[i].transform.rotation;

            // --- Play animation if assigned ---
            if (i < animators.Length && animators[i] != null && i < clips.Length && clips[i] != null)
            {
                animators[i].Play(clips[i].name);
            }
        }
    }

    // --- Player Phase 2 Logic ---
    private void UpdatePlayerPhase_2()
    {
        if (player == null || playerTarget_2 == null)
            return;

        // Parent player to target
        player.transform.SetParent(playerTarget_2.transform);

        // Snap instantly to target position and rotation
        player.transform.position = playerTarget_2.transform.position;
        player.transform.rotation = playerTarget_2.transform.rotation;

        
    }
}
