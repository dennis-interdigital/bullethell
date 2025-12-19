using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy Stats", fileName = "EnemyStats")]
public class EnemyStats : ScriptableObject
{
    [Header("Identity")]
    public string enemyName = "EnemyName";

    [Header("Core")]
    [Min(1f)] public float maxHealth = 50f;

    [Header("Combat (optional)")]
    public float contactDamage = 10f;

    [Header("Rewards (optional)")]
    public float scoreReward = 10;
    public int goldReward = 50;

}
