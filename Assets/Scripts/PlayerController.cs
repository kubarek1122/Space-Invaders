using System;
using System.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Serialized Fields

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Material projectileMaterial;
    [SerializeField] private ParticleSystem particles;
    
    [Header("Player Settings")]
    [SerializeField] private int maxLives = 3;
    [Tooltip("Bullets per second")]
    [SerializeField] private float fireRate = 1.0f;

    [SerializeField] private float deathDelay = 0.5f;
    [Tooltip("Counted from the moment of taking damage")]
    [SerializeField] private float invincibilityTime = 1.0f;
    
    [Header("Movement Settings")]
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float screenEdgeThreshold = 0.02f;

    #endregion

    public static event Action<int> OnLivesChanged;
    public static event Action<int> OnScoreChanged;
    
    private Camera _camera;

    private Vector2 _movement;
    private Vector3 _initialPosition;
    
    private bool _isOnCooldown;

    private bool _isRespawning = false;
    private bool _isInvincible = false;

    public int CurrentScore { get; private set; }
    public int CurrentLives { get; private set; }

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        _initialPosition = transform.position;
        ResetPlayer();
    }

    private void Update()
    {
        if (_isRespawning) return;
        
        var viewportPosition = _camera.WorldToViewportPoint(transform.position);
        if ((viewportPosition.x < screenEdgeThreshold && _movement.x < 0) ||
            (viewportPosition.x > 1 - screenEdgeThreshold && _movement.x > 0))
            return;

        transform.Translate(new Vector3(_movement.x, 0f, 0f) * (speed * Time.deltaTime));
    }

    /// <summary>
    /// Method used to reset player stats and position
    /// </summary>
    public void ResetPlayer()
    {
        CurrentScore = 0;
        CurrentLives = maxLives;

        transform.position = _initialPosition;
        
        OnLivesChanged?.Invoke(CurrentLives);
        OnScoreChanged?.Invoke(CurrentScore);
    }
    
    /// <summary>
    /// Method used to add points on enemy kill
    /// </summary>
    /// <param name="points">Amount of points to add</param>
    public void AddPoints(int points)
    {
        CurrentScore += points;
        OnScoreChanged?.Invoke(CurrentScore);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.performed ? context.ReadValue<Vector2>() : Vector2.zero;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed && !_isOnCooldown)
            Fire();
    }

    private async void Fire()
    {
        _isOnCooldown = true;
        
        var projectile = ProjectileManager.ProjectilePool.Get();
        projectile.SetMaterial(projectileMaterial);
        projectile.tag = "Projectile";
        projectile.isPlayerBullet = true;
        projectile.Damage = 1;
        projectile.transform.position = firePoint.position;
        projectile.transform.rotation = firePoint.rotation;

        await Task.Delay((int)(1f / fireRate * 1000));
        
        _isOnCooldown = false;
    }
    
    private async void TakeDamage(int damage)
    {
        InvincibilityTime();
        
        _isRespawning = true;
        particles.Play();
        await Task.Delay((int)(deathDelay * 1000f));
        CurrentLives -= damage;
        transform.position = _initialPosition;
        OnLivesChanged?.Invoke(CurrentLives);
        _isRespawning = false;
    }

    private async void InvincibilityTime()
    {
        _isInvincible = true;
        await Task.Delay((int)(invincibilityTime * 1000f));
        _isInvincible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("EnemyProjectile")) return;
        
        var projectile = other.GetComponent<Projectile>();
        if (!projectile.isPlayerBullet && !_isInvincible)
            TakeDamage(projectile.Damage);
        
        projectile.Destroy();
    }
}