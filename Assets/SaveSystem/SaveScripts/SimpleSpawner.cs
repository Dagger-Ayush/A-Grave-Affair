using System.Collections;
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
}