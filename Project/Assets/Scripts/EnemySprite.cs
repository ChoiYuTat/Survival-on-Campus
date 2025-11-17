using UnityEngine;

public class EnemySprite : MonoBehaviour
{
    [SerializeField]
    private GameObject targetMovement;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = targetMovement.transform.position;
    }
}
