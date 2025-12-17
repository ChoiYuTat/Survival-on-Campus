using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/ActionConfig", fileName = "NewEnemyActionConfig")]
public class EnemyActionConfig : ScriptableObject
{
    [Header("������Ϣ")]
    public string skillName;                // �� SkillData.name ��Ӧ

    [Header("��ͨ�����ݳ�����")]
    public int attackCount = 1;             // ��ͨ��������
    public float approachDistance = 2f;     // ��ȫ����
    public float attackDistance = 0.5f;     // ��������
    public bool changeColorBeforeAttack = true; // �Ƿ񹥻�ǰ��ɫ
    public bool returnToOrigin = true;      // �������Ƿ񷵻�ԭλ

    [Header("��Ծ�����ݳ�����")]
    public bool useJumpAttack = false;      // �Ƿ�������Ծ����
    public float jumpHeight = 2f;           // ��Ծ�߶�
    public float jumpDuration = 1f;         // ��Ծ��ʱ��
}
