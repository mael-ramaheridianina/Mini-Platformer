using UnityEngine;

public class OdysseyAscension : MonoBehaviour
{
    [Header("Ascension Settings")]
    [Tooltip("Speed of automatic ascension toward the sky")]
    public float ascensionSpeed = 2f;
    
    [Tooltip("Direction of ascension (normalized). Default is straight up.")]
    public Vector2 ascensionDirection = Vector2.up;
    
    [Header("Camera Detection")]
    [Tooltip("Camera to check visibility against. If null, uses main camera.")]
    public Camera targetCamera;
    
    [Tooltip("Buffer distance outside camera bounds to stop ascension")]
    public float cameraBuffer = 2f;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    private OdysseyController odysseyController;
    private bool isVisible = false;
    private bool wasVisible = false;
    private Renderer objectRenderer;
    
    void Start()
    {
        // Get required components
        odysseyController = GetComponent<OdysseyController>();
        objectRenderer = GetComponent<Renderer>();
        
        if (odysseyController == null)
        {
            Debug.LogError("OdysseyAscension requires OdysseyController component on the same GameObject!");
        }
        
        if (objectRenderer == null)
        {
            Debug.LogError("OdysseyAscension requires a Renderer component on the same GameObject!");
        }
        
        // Use main camera if no camera is specified
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
        
        if (targetCamera == null)
        {
            Debug.LogError("No camera found for OdysseyAscension! Please assign a camera or ensure Camera.main exists.");
        }
        
        // Normalize ascension direction
        ascensionDirection = ascensionDirection.normalized;
    }
    
    void Update()
    {
        if (targetCamera == null || objectRenderer == null) return;
        
        CheckVisibility();
        
        // Only ascend if visible and player is not controlling
        if (isVisible && !IsPlayerControlling())
        {
            PerformAscension();
        }
        
        if (showDebugInfo)
        {
            LogDebugInfo();
        }
    }
    
    void CheckVisibility()
    {
        wasVisible = isVisible;
        
        if (objectRenderer != null)
        {
            // Check if the object is visible to the camera
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(targetCamera);
            isVisible = GeometryUtility.TestPlanesAABB(planes, objectRenderer.bounds);
            
            // If object just became visible, log it
            if (isVisible && !wasVisible && showDebugInfo)
            {
                Debug.Log($"Odyssey became visible to camera. Starting ascension.");
            }
            else if (!isVisible && wasVisible && showDebugInfo)
            {
                Debug.Log($"Odyssey is no longer visible to camera. Stopping ascension.");
            }
        }
    }
    
    void PerformAscension()
    {
        // Calculate movement for this frame
        Vector2 movement = ascensionDirection * ascensionSpeed * Time.deltaTime;
        
        // Apply movement
        transform.Translate(movement);
        
        // Check if we've moved too far outside camera bounds
        if (IsTooFarFromCamera())
        {
            // Stop ascension by making object invisible
            isVisible = false;
            if (showDebugInfo)
            {
                Debug.Log("Odyssey moved too far from camera bounds. Stopping ascension.");
            }
        }
    }
    
    bool IsTooFarFromCamera()
    {
        if (targetCamera == null) return false;
        
        // Get camera bounds in world space
        Vector3 cameraPos = targetCamera.transform.position;
        float cameraHeight = 2f * targetCamera.orthographicSize;
        float cameraWidth = cameraHeight * targetCamera.aspect;
        
        // Calculate distance from camera center
        Vector3 objectPos = transform.position;
        float distanceFromCamera = Vector3.Distance(objectPos, cameraPos);
        
        // Consider object too far if it's beyond camera bounds plus buffer
        float maxDistance = Mathf.Max(cameraWidth, cameraHeight) / 2f + cameraBuffer;
        
        return distanceFromCamera > maxDistance;
    }
    
    bool IsPlayerControlling()
    {
        // Check if player is controlling the Odyssey
        return odysseyController != null && odysseyController.IsPlayerControlling;
    }
    
    void LogDebugInfo()
    {
        string status = isVisible ? "VISIBLE" : "HIDDEN";
        string controlling = IsPlayerControlling() ? "PLAYER CONTROLLING" : "AUTO ASCENSION";
        
        Debug.Log($"Odyssey Status: {status} | {controlling} | Ascending: {isVisible && !IsPlayerControlling()}");
    }
    
    // Public method to manually start/stop ascension
    public void SetAscensionEnabled(bool enabled)
    {
        isVisible = enabled;
    }
    
    // Public method to change ascension direction
    public void SetAscensionDirection(Vector2 newDirection)
    {
        ascensionDirection = newDirection.normalized;
    }
    
    // Public method to change ascension speed
    public void SetAscensionSpeed(float newSpeed)
    {
        ascensionSpeed = Mathf.Max(0f, newSpeed);
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw ascension direction
        Gizmos.color = Color.cyan;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)(ascensionDirection * 3f);
        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawSphere(endPos, 0.2f);
        
        // Draw camera detection info if camera is assigned
        if (targetCamera != null)
        {
            Gizmos.color = isVisible ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
