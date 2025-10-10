//using UnityEngine;

//public class PlayerSpawnHandler : MonoBehaviour
//{
//    private void Start()
//    {
//        string spawnID = SceneTransitionManager.nextSpawnPointID;

//        if(string.IsNullOrEmpty(spawnID) )
//            return;

//        SceneSpawnPoint[] spawnPoints = Object.FindObjectsByType<SceneSpawnPoint>(FindObjectsSortMode.None);
//        foreach (var point in spawnPoints)
//        {
//            if(point.spawnID == spawnID )
//            {
//                transform.position = point.transform.position + Vector3.up * 0.9f;
//                transform.rotation = point.transform.rotation;
//                return;
//            }
//        }
//    }
//}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnHandler : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(SetSpawnAfterFrame());
    }

    private IEnumerator SetSpawnAfterFrame()
    {
        // Wait one frame so all scene colliders and objects initialize
        yield return null;

        string currentScene = SceneManager.GetActiveScene().name;
        string targetID = SceneTransitionManager.targetSpawnID;

        SceneSpawnPoint[] spawnPoints = Object.FindObjectsByType<SceneSpawnPoint>(FindObjectsSortMode.None);

        foreach (var point in spawnPoints)
        {
            if (point.sceneName == currentScene &&
                (string.IsNullOrEmpty(targetID) || point.spawnID == targetID))
            {
                Vector3 spawnPos = point.transform.position;
                Quaternion spawnRot = point.transform.rotation;

                // Adjust Y position using ground detection
                if (Physics.Raycast(spawnPos + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f))
                    spawnPos.y = hit.point.y + 0.1f; // small offset above ground

                // Disable character controller or rigidbody temporarily
                var controller = GetComponent<CharacterController>();
                var rb = GetComponent<Rigidbody>();

                if (controller != null) controller.enabled = false;
                if (rb != null) rb.isKinematic = true;

                transform.SetPositionAndRotation(spawnPos, spawnRot);

                if (controller != null) controller.enabled = true;
                if (rb != null) rb.isKinematic = false;

                Debug.Log($" Spawned player at '{point.spawnID}' in scene '{currentScene}'");
                yield break;
            }
        }

        Debug.LogWarning($" No spawn point found for scene '{currentScene}' and spawn ID '{targetID}'");
    }
}
