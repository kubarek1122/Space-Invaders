using UnityEngine;
using UnityEngine.Pool;

namespace Managers
{
    public class ProjectileManager : MonoBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;

        public static ObjectPool<Projectile> ProjectilePool { get; private set; }

        private void Start()
        {
            ProjectilePool = new ObjectPool<Projectile>(
                () => Instantiate(projectilePrefab),
                projectile =>
                {
                    projectile.SetDestroyAction(DestroyProjectile);
                    projectile.tag = "Untagged";
                    projectile.gameObject.SetActive(true);
                },
                projectile => projectile.gameObject.SetActive(false),
                projectile => Destroy(projectile.gameObject), 
                false, 25, 50);
        }

        private void DestroyProjectile(Projectile projectile)
        {
            ProjectilePool.Release(projectile);
        }
    }
}