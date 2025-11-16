using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    [SerializeField]
    private PlayerData data;

    void Start()
    {

    }

    public void SaveGame()
    {

        Debug.Log("Game Saved");
    }
}
