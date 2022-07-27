using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace Managers
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance;

        public enum EnemyDifficulty
        {
            Easy,
            Normal,
            Hard
        }

        #region Serialized Fields

        [Header("References")] 
        [SerializeField] private EnemyController enemyBasePrefab;
        [SerializeField] private EnemySettings easyEnemySettings;
        [SerializeField] private EnemySettings normalEnemySettings;
        [SerializeField] private EnemySettings hardEnemySettings;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Transform barriersParent;
    
        [Header("Enemy settings")]
        [Tooltip("Easy - only easy enemies\nNormal - half of rows are normal rest easy\nHard - one row of hard, half normal rest easy")]
        [SerializeField] private EnemyDifficulty enemyDifficulty;
    
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float moveDelay = 1f;
        [SerializeField] private Transform enemyFormationPoint;
        [SerializeField] private bool showFormation;
        [SerializeField] private float spread = 1f;
        [SerializeField] private int formationWidth = 5;
        [SerializeField] private int formationHeight = 5;
        [SerializeField] private float nthOffset;
        [SerializeField] private float boundsPadding;

        #endregion
    
        private ObjectPool<EnemyController> _enemyPool;
        private List<EnemyController> _pooledEnemies;
        private List<Barrier> _barriers;
        private Bounds _bounds;
        private int _moveDirection = 1;
        private Camera _camera;
        private bool _isMoving;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            _enemyPool = new ObjectPool<EnemyController>(
                () => Instantiate(enemyBasePrefab),
                controller => controller.gameObject.SetActive(true),
                controller => controller.gameObject.SetActive(false),
                controller => Destroy(controller.gameObject),
                defaultCapacity: formationWidth * formationHeight, maxSize: 100
            );

            _pooledEnemies = new List<EnemyController>();
            _barriers = new List<Barrier>();
            _bounds = CalculateBounds();
            _camera = Camera.main;

            foreach (Transform barrier in barriersParent)
            {
                _barriers.Add(barrier.GetComponent<Barrier>());
            }
        }

        private void Update()
        {
            _bounds = CalculateBounds();
        }
        
        /// <summary>
        /// Method used to set difficulty level
        /// </summary>
        /// <param name="difficulty">Difficulty</param>
        public void SetDifficulty(EnemyDifficulty difficulty)
        {
            enemyDifficulty = difficulty;
        }
        
        /// <summary>
        /// Method used to move enemy formation
        /// </summary>
        public async void MoveEnemies()
        {
            if (_isMoving)
                return;

            _isMoving = true;

            var minViewportPosition = _camera.WorldToViewportPoint(_bounds.min);
            var maxViewportPosition = _camera.WorldToViewportPoint(_bounds.max);

            if (maxViewportPosition.x > 0.95f)
                _moveDirection = -1;
            if (minViewportPosition.x < 0.05f)
                _moveDirection = 1;

            await Task.Delay((int)(moveDelay * 1000f));

            enemyFormationPoint.Translate(new Vector3(_moveDirection, 0f, 0f) * (moveSpeed * Time.timeScale));

            _isMoving = false;
        }

        /// <summary>
        /// Method used to spawn enemies in formation
        /// </summary>
        public void SpawnEnemies()
        {
            enemyFormationPoint.position = new Vector3(0f, enemyFormationPoint.position.y, 0f);
            _moveDirection = 1;

            foreach (var point in GetBoxFormationPoints())
            {
                var enemy = _enemyPool.Get();
                enemy.transform.SetParent(enemyFormationPoint);
                enemy.transform.position = enemyFormationPoint.position + point;
                enemy.SetDestroyAction(DestroyEnemy);
                _pooledEnemies.Add(enemy);
            }
        
            DistributeDifficulty();
        }
        
        private void DistributeDifficulty()
        {
            int easyCount = 0, normalCount = 0, hardCount = 0;
            switch (enemyDifficulty)
            {
                case EnemyDifficulty.Easy:
                    easyCount = formationWidth * formationHeight;
                    break;
                case EnemyDifficulty.Normal:
                    normalCount = formationWidth * (formationHeight / 2);
                    easyCount = formationWidth * formationHeight - normalCount;
                    break;
                case EnemyDifficulty.Hard:
                    hardCount = formationWidth;
                    normalCount = formationWidth * (formationHeight / 2);
                    easyCount = formationWidth * formationHeight - normalCount - hardCount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var enemy in _pooledEnemies)
            {
                if (easyCount > 0)
                {
                    enemy.SetSettings(easyEnemySettings);
                    easyCount--;
                } else if (normalCount > 0)
                {
                    enemy.SetSettings(normalEnemySettings);
                    normalCount--;
                } else if (hardCount > 0)
                {
                    enemy.SetSettings(hardEnemySettings);
                    hardCount--;
                }
            }
        }

        /// <summary>
        /// Method used to activate firing on enemies
        /// </summary>
        public void BeginFiring()
        {
            foreach (var enemy in _pooledEnemies)
            {
                enemy.StartFire();
            }
        }
        
        /// <summary>
        /// Method used to get active enemy count
        /// </summary>
        /// <returns>Amount of active enemies</returns>
        public int GetActiveEnemiesCount()
        {
            return _enemyPool.CountActive;
        }
        
        /// <summary>
        /// Method used to return all enemies to pool
        /// </summary>
        public void DespawnEnemies()
        {
            foreach (var enemy in _pooledEnemies)
            {
                enemy.StopFire();
                _enemyPool.Release(enemy);
            }

            _pooledEnemies.Clear();
        }

        private void DestroyEnemy(EnemyController enemy)
        {
            enemy.StopFire();
            playerController.AddPoints(enemy.Points);
            _pooledEnemies.Remove(enemy);
            _enemyPool.Release(enemy);
        }

        /// <summary>
        /// Method used to reset barriers
        /// </summary>
        public void ResetBarriers()
        {
            foreach (var barrier in _barriers)
            {
                barrier.ResetBarrier();
            }
        }

        private IEnumerable<Vector3> GetBoxFormationPoints()
        {
            var middle = new Vector3(formationWidth * 0.5f, formationHeight * 0.5f, 0);

            for (var y = 0; y < formationHeight; y++)
            {
                for (var x = 0; x < formationWidth; x++)
                {
                    var pos = new Vector3(x + (y % 2 == 0 ? 0 : nthOffset), y, 0);

                    pos -= middle;

                    pos *= spread;

                    yield return pos;
                }
            }
        }

        private Bounds CalculateBounds()
        {
            Vector2 min = Vector2.positiveInfinity, max = Vector2.negativeInfinity;

            if (_pooledEnemies is { Count: > 0 })
            {
                foreach (var enemy in _pooledEnemies)
                {
                    var pos = enemy.transform.position;

                    min = Vector2.Min(min, pos);
                    max = Vector2.Max(max, pos);
                }
            }
            else
            {
                foreach (var point in GetBoxFormationPoints())
                {
                    var pos = enemyFormationPoint.position + point;

                    min = Vector2.Min(min, pos);
                    max = Vector2.Max(max, pos);
                }
            }

            var position = Vector2.Lerp(min, max, 0.5f);
            return new Bounds(position, new Vector3(max.x - min.x + boundsPadding, max.y - min.y + boundsPadding, 1f));
        }

        private void OnDrawGizmos()
        {
            if (!showFormation)
                return;

            Gizmos.color = Color.green;

            var cubeSize = Vector3.one * 0.5f;

            foreach (var point in GetBoxFormationPoints())
            {
                var pos = enemyFormationPoint.position + point;
                Gizmos.DrawCube(pos, cubeSize);
            }

            var bounds = CalculateBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}