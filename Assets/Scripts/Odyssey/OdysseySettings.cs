using UnityEngine;

[CreateAssetMenu(fileName = "OdysseySettings", menuName = "Odyssey/Settings")]
public class OdysseySettings : ScriptableObject
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float acceleration = 5f;
    public float deceleration = 8f;
    
    [Header("Detection Settings")]
    public float detectionRadius = 2f;
    public LayerMask playerLayer = 1;
    
    [Header("Timer Settings")]
    public float controlTimeLimit = 5f;
    public bool fadeOutEffect = true;
  
    
    [Header("Movement Method")]
    public bool useParentingMethod = true;
    
    [Header("Visual Effects")]
    public bool showDetectionRadius = true;
    public Color detectionRadiusColor = Color.yellow;
    
    [Header("Audio Settings")]
    public AudioClip mountSound;
    public AudioClip dismountSound;
    public AudioClip warningSound;
    public AudioClip disappearSound;
    
    /// <summary>
    /// Applique ces paramètres à un OdysseyController
    /// </summary>
    public void ApplyToController(OdysseyController controller)
    {
        if (controller == null) return;
        
        controller.moveSpeed = moveSpeed;
        controller.acceleration = acceleration;
        controller.deceleration = deceleration;
        controller.detectionRadius = detectionRadius;
        controller.playerLayer = playerLayer;
        controller.controlTimeLimit = controlTimeLimit;
        controller.fadeOutEffect = fadeOutEffect;
        controller.useParentingMethod = useParentingMethod;
        controller.showDetectionRadius = showDetectionRadius;    }
    
    /// <summary>
    /// Crée des paramètres par défaut
    /// </summary>
    public static OdysseySettings CreateDefault()
    {
        var settings = CreateInstance<OdysseySettings>();
        settings.name = "Default Odyssey Settings";
        return settings;
    }
      /// <summary>
    /// Crée des paramètres pour un Odyssey rapide
    /// </summary>
    public static OdysseySettings CreateFast()
    {
        var settings = CreateDefault();
        settings.name = "Fast Odyssey Settings";
        settings.moveSpeed = 15f;
        settings.acceleration = 8f;
        settings.controlTimeLimit = 3f;
        return settings;
    }
      /// <summary>
    /// Crée des paramètres pour un Odyssey lent et stable
    /// </summary>
    public static OdysseySettings CreateSlow()
    {
        var settings = CreateDefault();
        settings.name = "Slow Odyssey Settings";
        settings.moveSpeed = 6f;
        settings.acceleration = 3f;
        settings.controlTimeLimit = 8f;
        return settings;
    }
}
