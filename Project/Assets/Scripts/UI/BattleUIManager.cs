using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    private GameObject[] battleManager;

    private void Start()
    {
        battleManager = GameObject.FindGameObjectsWithTag("BattleManager");
    }
    public void SkipTrun() 
    {
        for (int i = 0; i < battleManager.Length; i++) 
        {
            if (battleManager[i] != null) 
            {
                battleManager[i].GetComponent<BattleManager>().UseItem();
            }
        }
    }
}
