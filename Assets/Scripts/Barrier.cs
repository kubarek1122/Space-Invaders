using TMPro;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private TMP_Text healthText;

    private int _currentHealth;

    private void Start()
    {
        ResetBarrier();
    }

    /// <summary>
    /// Reset barrier
    /// </summary>
    public void ResetBarrier()
    {
        _currentHealth = maxHealth;
        healthText.text = _currentHealth.ToString();
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("EnemyProjectile") && !other.CompareTag("Projectile")) return;
        
        var projectile = other.GetComponent<Projectile>();
        _currentHealth -= projectile.Damage;
        projectile.Destroy();
        
        healthText.text = _currentHealth.ToString();
        if (_currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
