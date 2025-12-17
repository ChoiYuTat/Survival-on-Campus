using UnityEngine;

public class SaveID : MonoBehaviour
{
    private int saveID = 0;
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetSaveID(int id)
    {
        saveID = id;
    }

    public int GetSaveID()
    {
        return saveID;
    }
}
