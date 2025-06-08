using UnityEngine;
using Platformer.Mechanics;

public class OdysseyTestManager : MonoBehaviour
{    [Header("Test Configuration")]
    public OdysseyController odysseyController;
    public PlayerController playerController;    [Header("Test Controls")]
    public KeyCode testPlayerMoveKey = KeyCode.Y;
    public KeyCode switchMovementMethodKey = KeyCode.U;
    public KeyCode testTimerKey = KeyCode.I;
    public KeyCode forceDisappearKey = KeyCode.O;
    
    [Header("Test Settings")]
    public float testMoveDistance = 2f;
    public float testMoveSpeed = 5f;
    
    private float lastTimerCheck = 0f;
    
    void Update()
    {
        HandleTestInputs();
        DisplayDebugInfo();
    }
      void HandleTestInputs()
    {
        // Test de mouvement du joueur
        if (Input.GetKeyDown(testPlayerMoveKey))
        {
            TestPlayerMovement();
        }
          // Changement de méthode de mouvement
        if (Input.GetKeyDown(switchMovementMethodKey))
        {
            SwitchMovementMethod();
        }
        
        // Test du timer
        if (Input.GetKeyDown(testTimerKey))
        {
            TestTimer();
        }
        
        // Force la disparition
        if (Input.GetKeyDown(forceDisappearKey))
        {
            ForceDisappear();
        }    }
    
    void TestPlayerMovement()
    {
        if (playerController != null)
        {
            Debug.Log("TEST: Test du mouvement du joueur");
            Vector3 playerPos = playerController.transform.position;
            Debug.Log($"Position du joueur: {playerPos}");
            Debug.Log($"Contrôles activés: {playerController.controlEnabled}");
            Debug.Log($"État du joueur: {playerController.jumpState}");
        }
    }
    
    void SwitchMovementMethod()
    {
        if (odysseyController != null)
        {
            odysseyController.useParentingMethod = !odysseyController.useParentingMethod;
            Debug.Log($"TEST: Méthode de mouvement changée vers: {(odysseyController.useParentingMethod ? "Parentage" : "Manuel")}");
        }
    }
    
    void DisplayDebugInfo()
    {
        // Affiche les informations de debug en continu (avec limitation de fréquence)
        if (Time.time % 2f < Time.deltaTime) // Toutes les 2 secondes
        {
            LogSystemStatus();
        }
    }    void LogSystemStatus()
    {
        if (odysseyController != null)
        {
            Debug.Log("=== STATUS ODYSSEY ===");
            Debug.Log($"Joueur sur l'Odyssey: {odysseyController.IsPlayerControlling}");
            Debug.Log($"Méthode de mouvement: {(odysseyController.useParentingMethod ? "Parentage" : "Manuel")}");
            Debug.Log($"Position Odyssey: {odysseyController.transform.position}");
            Debug.Log($"Limite de temps: {odysseyController.controlTimeLimit}s");
            Debug.Log($"Effet de fade: {odysseyController.fadeOutEffect}");
            
            if (playerController != null)
            {
                Debug.Log($"Position Joueur: {playerController.transform.position}");
                Debug.Log($"Contrôles Joueur: {playerController.controlEnabled}");
            }
            Debug.Log("======================");
        }
    }
    
    void OnGUI()
    {
        // Interface simple pour les tests
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.BeginVertical("box");
          GUILayout.Label("=== ODYSSEY TEST MANAGER ===");
        
        if (GUILayout.Button($"Test Player Movement ({testPlayerMoveKey})"))
        {
            TestPlayerMovement();
        }
        
        if (GUILayout.Button($"Test Timer Info ({testTimerKey})"))
        {
            TestTimer();
        }
        
        if (GUILayout.Button($"Force Disappear ({forceDisappearKey})"))
        {
            ForceDisappear();
        }
        
        if (odysseyController != null)
        {
            if (GUILayout.Button($"Switch Movement Method ({switchMovementMethodKey})"))
            {
                SwitchMovementMethod();
            }            GUILayout.Label($"Current Method: {(odysseyController.useParentingMethod ? "Parenting" : "Manual")}");
            GUILayout.Label($"Player Controlling: {odysseyController.IsPlayerControlling}");
            GUILayout.Label($"Time Limit: {odysseyController.controlTimeLimit}s");
            GUILayout.Label($"Fade Effect: {odysseyController.fadeOutEffect}");
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    
    void TestTimer()
    {
        if (odysseyController != null)
        {
            Debug.Log("TEST: Informations sur le timer");
            Debug.Log($"Limite de temps: {odysseyController.controlTimeLimit}s");
            Debug.Log($"Joueur sur l'Odyssey: {odysseyController.IsPlayerControlling}");
            
            // Utilise la réflexion pour accéder aux variables privées (pour le debug)
            var controlTimerField = typeof(OdysseyController).GetField("controlTimer", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var timerStartedField = typeof(OdysseyController).GetField("timerStarted", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (controlTimerField != null && timerStartedField != null)
            {
                float currentTimer = (float)controlTimerField.GetValue(odysseyController);
                bool timerActive = (bool)timerStartedField.GetValue(odysseyController);
                
                Debug.Log($"Timer actif: {timerActive}");
                Debug.Log($"Temps écoulé: {currentTimer:F1}s");
                Debug.Log($"Temps restant: {(odysseyController.controlTimeLimit - currentTimer):F1}s");
            }
        }
    }
    
    void ForceDisappear()
    {
        if (odysseyController != null)
        {
            Debug.Log("TEST: Force la disparition de l'Odyssey");
            
            // Change temporairement la limite de temps pour forcer la disparition
            float originalLimit = odysseyController.controlTimeLimit;
            odysseyController.controlTimeLimit = 0.1f;
            
            // Restaure après un court délai
            StartCoroutine(RestoreTimeLimit(originalLimit));
        }
    }
    
    System.Collections.IEnumerator RestoreTimeLimit(float originalLimit)
    {
        yield return new WaitForSeconds(1f);
        if (odysseyController != null)
        {
            odysseyController.controlTimeLimit = originalLimit;
            Debug.Log($"TEST: Limite de temps restaurée à {originalLimit}s");
        }
    }
}
