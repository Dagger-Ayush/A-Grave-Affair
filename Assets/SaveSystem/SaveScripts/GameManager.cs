using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
      SaveLoadManager.Instance.SetGameInProgress(true);
        
    }

    private void OnDestroy()
    {
        SaveLoadManager.Instance.SetGameInProgress(false);
    }

    void Update()
    {
        


    }
}
