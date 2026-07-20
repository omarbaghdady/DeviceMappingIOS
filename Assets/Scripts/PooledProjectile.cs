using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class PooledProjectile : MonoBehaviour
{
    private IObjectPool<PooledProjectile> _pool;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        // Safety check to prevent returning to the pool twice if disabled manually
        CancelInvoke(nameof(ReturnToPool));
    }

    public void Initialize(IObjectPool<PooledProjectile> pool, Vector3 position, Quaternion rotation, Vector3 force, float lifeTime)
    {
        _pool = pool;
        
        // 1. Reset position and rotation
        transform.position = position;
        transform.rotation = rotation;
        
        // 2. Wipe old physics momentum (Unity 6 uses linearVelocity)
        _rb.linearVelocity = Vector3.zero; 
        _rb.angularVelocity = Vector3.zero;
        
        // 3. Launch
        _rb.AddForce(force, ForceMode.Impulse);
        
        // 4. Schedule the return to the pool
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    private void ReturnToPool()
    {
        if (gameObject.activeInHierarchy)
        {
            _pool.Release(this);
        }
    }
}