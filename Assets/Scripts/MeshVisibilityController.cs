using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARMeshManager))]
public class MeshVisibilityController : MonoBehaviour
{
    private ARMeshManager _meshManager;
    [SerializeField] private Button _toggleMeshButton;

    private bool _isMeshVisible = true;

    private void Awake()
    {
        _meshManager = GetComponent<ARMeshManager>();
    }

    private void Start()
    {
        if (_toggleMeshButton != null)
        {
            _toggleMeshButton.onClick.AddListener(ToggleVisibility);
        }
    }

    private void ToggleVisibility()
    {
        _isMeshVisible = !_isMeshVisible;
        
        // Use .meshes instead of .trackables for ARMeshManager
        foreach (var mesh in _meshManager.meshes)
        {
            var meshRenderer = mesh.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = _isMeshVisible;
            }
        }
        
        Debug.Log($"Mesh visibility set to: {_isMeshVisible}");
    }
}