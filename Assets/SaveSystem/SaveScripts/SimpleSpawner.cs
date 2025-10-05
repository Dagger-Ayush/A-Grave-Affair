/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class SimpleSpawner : MonoBehaviour, ISavable
{
    [SerializeField] string ID = "SimpleSpawner-1";
    [SerializeField] int NumObjects = 5;
    List<System.Tuple<GameObject, PrimitiveType>> SpawnedObjects = new List<System.Tuple<GameObject, PrimitiveType>>();

    public void PrepareForSave(SavedGamesState gameState)
    {
        gameState.SpawnerState.ID = ID;

        foreach (var spawnedGOinfo in SpawnedObjects)
        {
            var location = spawnedGOinfo.Item1.transform.position;
            gameState.SpawnerState.SpawnedObjects.Add(new SavedGamesState.SimpleSpawnerState.Entry()
            {
                Location = new System.Tuple<float, float, float>(location.x, location.y, location.z),
                Type = spawnedGOinfo.Item2
            });
        }
    }

    void Start()
    {
        if (SaveLoadManager.Instance == null)
        {
            Debug.LogError("SaveLoadManager instance is null!");
            return;
        }

        SaveLoadManager.Instance.RegisterHandler(this);

        if (SaveLoadManager.Instance.SavedState != null)
        {
            var spawnerState = SaveLoadManager.Instance.SavedState.SpawnerState;

            if (spawnerState.ID == ID)
            {
                foreach (var entry in spawnerState.SpawnedObjects)
                {
                    PrimitiveType typeToSpawn = entry.Type;
                    Vector3 location = new Vector3(entry.Location.Item1, entry.Location.Item2, entry.Location.Item3);
                    SpawnObject(typeToSpawn, location);
                }
                return;
            }
        }

        var availableTypes = System.Enum.GetValues(typeof(PrimitiveType));
        for (int i = 0; i < NumObjects; i++)
        {
            int typeIndex = Random.Range(0, availableTypes.Length);
            PrimitiveType typeToSpawn = (PrimitiveType)availableTypes.GetValue(typeIndex);
            Vector3 location = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            SpawnObject(typeToSpawn, location);
        }
    }

    void SpawnObject(PrimitiveType type, Vector3 location)
    {
        var newGO = GameObject.CreatePrimitive(type);
        newGO.transform.position = location;
        SpawnedObjects.Add(new System.Tuple<GameObject, PrimitiveType>(newGO, type));
    }

    void OnDestroy()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.DeregisterHandler(this);
        }
    }

    void Update()
    {
       
    }
}*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpawner : MonoBehaviour, ISavable
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private string spawnerID;

    private bool hasSpawned = false;
    private GameObject spawnedObject;

    void Start()
    {
       
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.RegisterHandler(this);
        }

       
        if (SaveLoadManager.Instance?.SavedState != null)
        {
            LoadState();
        }
    }

    public void SpawnObject()
    {
        if (!hasSpawned && objectToSpawn != null)
        {
            spawnedObject = Instantiate(objectToSpawn, transform.position, transform.rotation);
            hasSpawned = true;

          
            SaveLoadManager.Instance?.RequestSave(ESaveSlot.Slot1, ESaveType.Manual);
        }
    }

    public void PrepareForSave(SavedGamesState gameState)
    {
      
        if (!string.IsNullOrEmpty(spawnerID))
        {
           
            gameState.GameData.CluesFound ??= new List<ClueData>();

         
            var existingClue = gameState.GameData.CluesFound.Find(c => c.clueID == $"spawner_{spawnerID}");
            if (existingClue != null)
            {
                existingClue.found = hasSpawned;
            }
            else
            {
                gameState.GameData.CluesFound.Add(new ClueData
                {
                    clueID = $"spawner_{spawnerID}",
                    found = hasSpawned
                });
            }
        }
    }

    private void LoadState()
    {
        var savedState = SaveLoadManager.Instance.SavedState;
        if (savedState?.GameData?.CluesFound != null && !string.IsNullOrEmpty(spawnerID))
        {
            var spawnerClue = savedState.GameData.CluesFound.Find(c => c.clueID == $"spawner_{spawnerID}");
            if (spawnerClue != null)
            {
                hasSpawned = spawnerClue.found;

                if (hasSpawned && objectToSpawn != null)
                {
                    spawnedObject = Instantiate(objectToSpawn, transform.position, transform.rotation);
                }
            }
        }
    }

   
    public bool HasSpawned()
    {
        return hasSpawned;
    }

    public void ResetSpawner()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }
        hasSpawned = false;
    }

    private void OnDestroy()
    {
        if (SaveLoadManager.Instance != null)
        {
            SaveLoadManager.Instance.DeregisterHandler(this);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = hasSpawned ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
    }
}