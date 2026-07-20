using System.Collections;
using System.IO;
using NianticSpatial.NSDK.AR.Mapping;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation; 

public class Mapper : MonoBehaviour
{
    [SerializeField] private ARDeviceMappingManager _deviceMappingManager;
    [SerializeField] private Button _startMappingButton;
    [SerializeField] private ARMeshManager _meshManager; 

    private const string MapFileName = "SerializedDeviceMapData";

    private void Start()
    {
        // 1. Ensure the mesher is OFF before we start scanning
        if (_meshManager != null)
        {
            _meshManager.enabled = false;
        }

        _deviceMappingManager.MapFinalized += OnDeviceMapFinalized;
        _startMappingButton.onClick.AddListener(OnStartMappingClicked);
    }

    private void OnDestroy()
    {
        if (_deviceMappingManager != null)
        {
            _deviceMappingManager.MapFinalized -= OnDeviceMapFinalized;
        }
    }

    public void OnStartMappingClicked()
    {
        StartCoroutine(RunMapping());
    }

    private IEnumerator RunMapping()
    {
        _startMappingButton.gameObject.SetActive(false);
        
        if (_meshManager != null)
        {
            // 2. Wipe the old visual mesh and turn the mesher ON
            _meshManager.DestroyAllMeshes();
            _meshManager.enabled = true;
            Debug.Log("AR Meshing enabled.");
        }

        _deviceMappingManager.StartMapping();
        Debug.Log("Mapping started. Please scan the room...");
        
        // Scan duration
        yield return new WaitForSeconds(20.0f);
        
        _deviceMappingManager.StopMapping();
        
        if (_meshManager != null)
        {
            // 3. Turn the mesher OFF so it stops adding to the room
            _meshManager.enabled = false;
            Debug.Log("AR Meshing disabled. Geometry locked.");
        }

        _startMappingButton.gameObject.SetActive(true);
        Debug.Log("Mapping stopped. Compiling map...");
    }

    private void OnDeviceMapFinalized(byte[] mapData)
    {
        if (_deviceMappingManager.TryGetMapData(out var entireMapData))
        {
            var path = Path.Combine(Application.persistentDataPath, MapFileName);
            File.WriteAllBytes(path, entireMapData); 
            Debug.Log($"Map saved successfully to: {path}");
        }
        else
        {
            Debug.LogError("Map generation failed: entireMapData was empty.");
        }
    }
}