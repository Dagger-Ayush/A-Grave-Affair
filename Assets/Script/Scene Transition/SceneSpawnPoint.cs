using UnityEngine;

public class SceneSpawnPoint : MonoBehaviour
{
    [Tooltip("Exact name of the scene this spawn point belongs to.")]
    public string sceneName;

    [Tooltip("Unique identifier for this spawn point (e.g., 'DoorA', 'CaveExit')")]
    public string spawnID;
}
