using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    public void SkipTrun() 
    {
        BattleManager.Instance.UseItem();
    }
}
