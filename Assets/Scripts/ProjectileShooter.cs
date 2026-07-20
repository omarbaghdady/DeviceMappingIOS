using UnityEngine;
using UnityEngine.Pool;

public class ProjectileShooter : MonoBehaviour
{
    [SerializeField] private PooledProjectile _projectilePrefab;
    [SerializeField] private Transform _shootOrigin;
    [SerializeField] private float _shootForce = 15f;
    [SerializeField] private float _lifeTime = 4f;

    // Unity's built-in Object Pool
    private ObjectPool<PooledProjectile> _projectilePool;

    private void Awake()
    {
        // Initialize the pool with constructor functions
        _projectilePool = new ObjectPool<PooledProjectile>(
            createFunc: CreateProjectile,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: false, // Set to false for max performance
            defaultCapacity: 10,
            maxSize: 20 // Prevents memory leaks if you spam the button
        );
    }

    private PooledProjectile CreateProjectile()
    {
        // Instantiates the object but leaves it deactivated in the pool
        PooledProjectile projectile = Instantiate(_projectilePrefab);
        projectile.gameObject.SetActive(false);
        return projectile;
    }

    private void OnTakeFromPool(PooledProjectile projectile)
    {
        projectile.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(PooledProjectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(PooledProjectile projectile)
    {
        Destroy(projectile.gameObject);
    }

    public void Shoot()
    {
        if (_projectilePrefab == null || _shootOrigin == null)
        {
            Debug.LogError("[Shooter] Missing references.");
            return;
        }

        // 1. Grab a ball from the pool (or create a new one if empty)
        PooledProjectile projectile = _projectilePool.Get();

        // 2. Calculate direction and initialize
        Vector3 force = _shootOrigin.forward * _shootForce;
        projectile.Initialize(_projectilePool, _shootOrigin.position, _shootOrigin.rotation, force, _lifeTime);
    }
}