using System;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private EnemySettings enemySettings;
    
    [SerializeField] private Transform firePoint;

    [SerializeField] private float initialFireDelay = 1f;

    public int Points { get; private set; }
    private int _damage;
    private float _fireRate;
    private int _health;

    private MeshFilter _meshFilter;

    private Action<EnemyController> _destroyAction;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        SetSettings(enemySettings);
    }

    /// <summary>
    /// Start firing projectiles
    /// </summary>
    public void StartFire()
    {
        InvokeRepeating(nameof(Fire), Random.Range(0f, 1f / _fireRate) + initialFireDelay, 1f / _fireRate);
    }

    /// <summary>
    /// Stop firing projectiles
    /// </summary>
    public void StopFire()
    {
        CancelInvoke();
    }

    /// <summary>
    /// Method used to set action triggered on being destroyed
    /// </summary>
    /// <param name="action"></param>
    public void SetDestroyAction(Action<EnemyController> action)
    {
        _destroyAction = action;
    }

    /// <summary>
    /// Method used to set enemy settings
    /// </summary>
    /// <param name="settings">EnemySettings scriptable object</param>
    public void SetSettings(EnemySettings settings)
    {
        enemySettings = settings;
        _meshFilter.mesh = enemySettings.mesh;
        _damage = enemySettings.damage;
        _fireRate = enemySettings.fireRate;
        _health = enemySettings.health;
        Points = enemySettings.points;
    }

    private void Fire()
    {
        if (!gameObject.activeSelf) return;

        var projectile = ProjectileManager.ProjectilePool.Get();
        projectile.SetMaterial(enemySettings.projectileMaterial);
        projectile.speed = enemySettings.projectileSpeed;
        projectile.tag = "EnemyProjectile";
        projectile.isPlayerBullet = false;
        projectile.Damage = _damage;
        projectile.transform.position = firePoint.position;
        projectile.transform.rotation = firePoint.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Projectile")) return;

        var projectile = other.GetComponent<Projectile>();
        if (projectile.isPlayerBullet)
            _health -= projectile.Damage;

        projectile.Destroy();

        if (_health <= 0)
            _destroyAction.Invoke(this);
    }
}