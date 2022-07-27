using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "GameplaySettings/EnemySettings", order = 1)]
public class EnemySettings : ScriptableObject
{
    public Mesh mesh;
    public Material projectileMaterial;
    
    public int health;
    public int damage;
    public float fireRate;
    public float projectileSpeed;

    public int points;
}