using UnityEngine;

public class PosandAnimationUpdate : MonoBehaviour
{
    public static PosandAnimationUpdate Instance { get; private set; }

    [Header("Phase_1")]
    public GameObject[] charObjects;
    public GameObject[] targetposition_1;
    public AnimationClip[] newAnimationClips_1;
    public Animator[] charAnimators;

    [Header("Phase_2")]
    public GameObject[] targetposition_2;
    public AnimationClip[] newAnimationClips_2;

    [Header("Phase_2 Player Reference")]
    public GameObject player;
    public GameObject playerTarget_2;

    [Header("Phase_3")]
    public GameObject[] targetposition_3;
    public AnimationClip[] newAnimationClips_3;

    [Header("Phase_4")]
    public AnimationClip[] newAnimationClips_4;

    [Header("Phase_5")]
    public AnimationClip[] newAnimationClips_5;

    [Header("Phase_6")]
    public GameObject[] targetposition_6;
    public AnimationClip[] newAnimationClips_6;

    // cache override controllers
    private AnimatorOverrideController[] overrideControllers;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (charAnimators == null || charAnimators.Length == 0)
            return;

        overrideControllers = new AnimatorOverrideController[charAnimators.Length];

        for (int i = 0; i < charAnimators.Length; i++)
        {
            if (charAnimators[i] == null) continue;

            // Create unique override controller
            overrideControllers[i] = new AnimatorOverrideController(charAnimators[i].runtimeAnimatorController);
            charAnimators[i].runtimeAnimatorController = overrideControllers[i];
        }
    }

    // --- PHASES ---
    public void UpdatePhase_1() => UpdatePhase(charObjects, targetposition_1, charAnimators, newAnimationClips_1, 1);
    public void UpdatePhase_2() { UpdatePhase(charObjects, targetposition_2, charAnimators, newAnimationClips_2, 2); UpdatePlayerPhase_2(); }
    public void UpdatePhase_3() => UpdatePhase(charObjects, targetposition_3, charAnimators, newAnimationClips_3, 3);
    public void UpdatePhase_5() { UpdatePhase(charObjects, targetposition_2, charAnimators, newAnimationClips_4, 4); UpdatePlayerPhase_2(); }
    public void UpdatePhase_6() => UpdatePhase(charObjects, targetposition_2, charAnimators, newAnimationClips_5, 5);
    public void UpdatePhase_7() => UpdatePhase(charObjects, targetposition_6, charAnimators, newAnimationClips_6, 6);

    private void UpdatePhase(GameObject[] chars, GameObject[] targets, Animator[] animators, AnimationClip[] clips, int currentPhase)
    {
        // 🎵 Update background music for current phase
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSceneBackgroundByPhase(currentPhase);
        }

        for (int i = 0; i < chars.Length; i++)
        {
            if (i >= animators.Length || i >= targets.Length || i >= chars.Length) continue;
            if (chars[i] == null || targets[i] == null || animators[i] == null) continue;

            // ✅ Always reposition characters even if animation clip is missing
            chars[i].transform.SetParent(targets[i].transform);
            chars[i].transform.SetPositionAndRotation(targets[i].transform.position, targets[i].transform.rotation);

            // 🎬 If there are no clips or this index doesn’t have a valid clip, skip animation update
            if (clips == null || clips.Length == 0 || i >= clips.Length || clips[i] == null)
                continue;

            // ✅ Safe animation override logic
            Animator animator = animators[i];
            AnimatorOverrideController overrideController = overrideControllers[i];
            AnimationClip newClip = clips[i];

            // “Delete” extra states by overriding all clips with the new one
            foreach (var oldClip in overrideController.animationClips)
                overrideController[oldClip.name] = newClip;

            animator.runtimeAnimatorController = overrideController;
            animator.Play(overrideController.animationClips[0].name, 0, 0f);

            Debug.Log($"▶ {chars[i].name} now only plays '{newClip.name}' (all other states disabled)");
        }
    }

    // --- PLAYER ---
    private void UpdatePlayerPhase_2()
    {
        if (player == null || playerTarget_2 == null) return;
        player.transform.SetParent(playerTarget_2.transform);
        player.transform.SetPositionAndRotation(playerTarget_2.transform.position, playerTarget_2.transform.rotation);
    }
}
