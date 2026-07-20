using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARAnchorManager))]
public class ReticleTargeter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _arCamera;
    [SerializeField] private GameObject _reticlePrefab;
    [SerializeField] private GameObject _objectToPlacePrefab;

    private GameObject _activeReticle;

    private void Awake()
    {
        // Instantiate the visual reticle and hide it initially
        _activeReticle = Instantiate(_reticlePrefab);
        _activeReticle.SetActive(false);
    }

    private void Update()
    {
        UpdateReticlePosition();
    }

    private void UpdateReticlePosition()
    {
        Ray ray = _arCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (!_activeReticle.activeSelf) _activeReticle.SetActive(true);

            _activeReticle.transform.position = hit.point;
            _activeReticle.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        else
        {
            if (_activeReticle.activeSelf) _activeReticle.SetActive(false);
        }
    }

    // Standard public void so the Unity UI Button can never fail to trigger it
    public void PlaceObject()
    {
        Debug.Log("Button Clicked!");

        if (_activeReticle != null && _activeReticle.activeSelf)
        {
            if (_objectToPlacePrefab == null)
            {
                Debug.LogError("FAILED: The Object To Place Prefab slot is empty in the Inspector!");
                return;
            }

            // 1. Immediately spawn the object exactly where the reticle is
            GameObject spawnedObject = Instantiate(_objectToPlacePrefab, _activeReticle.transform.position, _activeReticle.transform.rotation);
            
            // 2. Attach an AR Anchor directly to the object. 
            // In AR Foundation, simply attaching this component automatically asks the system to lock it in place physically.
            spawnedObject.AddComponent<ARAnchor>();
            
            Debug.Log($"Object successfully spawned and anchored at {spawnedObject.transform.position}");
        }
        else
        {
            Debug.LogWarning("FAILED: Reticle is not hitting the mesh.");
        }
    }
}