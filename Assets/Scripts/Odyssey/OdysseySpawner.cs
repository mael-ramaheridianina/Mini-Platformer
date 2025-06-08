using UnityEngine;

public class OdysseySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject odysseyPrefab;
    public OdysseySettings odysseySettings;
    public Transform[] spawnPoints;
    
    [Header("Spawn Conditions")]
    public bool spawnOnStart = false;
    public bool spawnOnPlayerTrigger = true;
    public float spawnDelay = 1f;
    
    [Header("Detection")]
    public float triggerRadius = 5f;
    public LayerMask playerLayer = 1;
    
    private bool hasSpawned = false;
    private GameObject currentOdyssey;
    
    void Start()
    {
        if (spawnOnStart)
        {
            SpawnOdyssey();
        }
        
        // Valide la configuration
        ValidateSetup();
    }
    
    void Update()
    {
        if (spawnOnPlayerTrigger && !hasSpawned)
        {
            CheckPlayerProximity();
        }
    }
    
    void CheckPlayerProximity()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, triggerRadius, playerLayer);
        
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("Joueur détecté - Spawn de l'Odyssey");
                Invoke(nameof(SpawnOdyssey), spawnDelay);
                break;
            }
        }
    }
    
    public void SpawnOdyssey()
    {
        if (hasSpawned || odysseyPrefab == null) return;
        
        Vector3 spawnPosition = GetSpawnPosition();
        currentOdyssey = Instantiate(odysseyPrefab, spawnPosition, Quaternion.identity);
          // Applique les paramètres personnalisés
        if (odysseySettings != null)
        {
            var controller = currentOdyssey.GetComponent<OdysseyController>();
            
            if (controller != null) odysseySettings.ApplyToController(controller);
        }
        
        hasSpawned = true;
        Debug.Log($"Odyssey spawné à la position: {spawnPosition}");
    }
    
    Vector3 GetSpawnPosition()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return transform.position;
        }
        
        // Choisit un point de spawn aléatoire
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        return randomSpawnPoint.position;
    }
    
    public void ResetSpawner()
    {
        hasSpawned = false;
        if (currentOdyssey != null)
        {
            Destroy(currentOdyssey);
            currentOdyssey = null;
        }
        Debug.Log("Spawner réinitialisé");
    }
    
    void ValidateSetup()
    {
        if (odysseyPrefab == null)
        {
            Debug.LogError("OdysseySpawner: Aucun prefab Odyssey assigné!", this);
        }
        
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("OdysseySpawner: Aucun point de spawn défini. Utilisation de la position du spawner.", this);
        }
          // Vérifie que le prefab a les bons composants
        if (odysseyPrefab != null)
        {
            var controller = odysseyPrefab.GetComponent<OdysseyController>();
            
            if (controller == null)
            {
                Debug.LogError("OdysseySpawner: Le prefab n'a pas de composant OdysseyController!", this);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Affiche le rayon de détection
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
        
        // Affiche les points de spawn
        if (spawnPoints != null)
        {
            Gizmos.color = Color.blue;
            foreach (var spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawSphere(spawnPoint.position, 0.3f);
                    Gizmos.DrawLine(transform.position, spawnPoint.position);
                }
            }
        }
    }
    
    void OnDrawGizmos()
    {
        // Affiche toujours la position du spawner
        Gizmos.color = hasSpawned ? Color.red : Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
    
    // Méthodes publiques pour être appelées par d'autres scripts
    public bool HasSpawned => hasSpawned;
    public GameObject CurrentOdyssey => currentOdyssey;
    
    public void ForceSpawn()
    {
        if (!hasSpawned)
        {
            SpawnOdyssey();
        }
    }
}
