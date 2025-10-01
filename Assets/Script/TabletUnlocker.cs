using UnityEngine;

public class TabletUnlocker : MonoBehaviour
{
    public static TabletUnlocker instance;

    private bool unlocked = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

    }
    public void UnlockTablet()
    {
        if(unlocked) return;

        TabletManager.Instance.canOpenTablet = true;
        unlocked = true;

        Debug.Log("Tablet unlocked! Now you can open it with Tab.");
    }
}
