using UnityEngine;
using UnityEngine.Events;
using Platformer.Mechanics; // Ajout pour accéder à PlayerController

[System.Serializable]
public class PlayerEvent : UnityEvent<GameObject> { }

public class OdysseyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float acceleration = 5f;
    public float deceleration = 8f;
      [Header("Player Detection")]
    public LayerMask playerLayer = 1;
    public float detectionRadius = 2f;    [Header("Debug Visualization")]
    public bool showDetectionRadius = true;    [Header("Movement Method")]
    [Tooltip("Use Parenting method for smoother movement (recommended)")]
    public bool useParentingMethod = true;
      [Header("Timer Settings")]
    [Tooltip("Temps en secondes avant que l'Odyssey disparaisse une fois le joueur monté")]
    public float controlTimeLimit = 5f;
    [Tooltip("Effet de disparition (optionnel)")]
    public bool fadeOutEffect = true;
    
    [Header("Events")]
    public PlayerEvent OnPlayerMounted = new PlayerEvent();
    public PlayerEvent OnPlayerDismounted = new PlayerEvent();
    public UnityEvent OnTimerWarning = new UnityEvent();
    public UnityEvent OnDisappearing = new UnityEvent();
    
    private Rigidbody2D rb;
    private bool playerOnBoard = false;    // Propriété publique pour que l'autre script puisse vérifier si le joueur contrôle l'odyssey
    public bool IsPlayerControlling => playerOnBoard; // Retourne true dès que le joueur est sur l'odyssey
    
    private GameObject playerObject;
    private Vector2 currentVelocity;    private Vector3 lastPosition;
    private Rigidbody2D playerRigidbody;
    private float originalPlayerGravity;    private PlayerController playerController; // Référence au contrôleur du joueur
    private GameObject debugCircle;
    private Transform originalPlayerParent; // Pour sauvegarder le parent original du joueur
    
    // Variables pour le timer de disparition
    private float controlTimer = 0f;
    private bool timerStarted = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D odysseyCollider;    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // Récupère les composants pour l'effet de disparition
        spriteRenderer = GetComponent<SpriteRenderer>();
        odysseyCollider = GetComponent<Collider2D>();
        
        // Configure le Rigidbody2D en mode kinematic pour éviter la chute
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearDamping = 2f;
        
        // Initialise la position précédente
        lastPosition = transform.position;
        
        // Crée le cercle de debug si activé
        CreateDebugCircle();
    }    void Update()
    {
        CheckPlayerPresence();
        
        // Gère le timer de disparition
        if (timerStarted)
        {
            UpdateDisappearanceTimer();
        }
        
        // Déplace le joueur avec l'odyssey s'il est dessus
        MovePlayerWithOdyssey();
        
        if (playerOnBoard)
        {
            HandleMovementInput();
        }
        else
        {
            // Décélération graduelle quand le joueur n'est pas sur l'odyssey
            ApplyDeceleration();
        }
        
        // Met à jour la position précédente
        lastPosition = transform.position;
    }void CheckPlayerPresence()
    {
        // Méthode alternative : détecte tous les colliders dans le rayon
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        
        Collider2D playerCollider = null;
        
        // Cherche le joueur parmi tous les colliders
        foreach (Collider2D collider in allColliders)
        {
            if (collider.CompareTag("Player"))
            {
                playerCollider = collider;
                Debug.Log($"Joueur trouvé: {collider.name}, Position: {collider.transform.position}");
                break;
            }
        }
        
        Debug.Log($"Nombre de colliders détectés: {allColliders.Length}, Joueur trouvé: {playerCollider != null}");
          if (playerCollider != null)
        {
            if (!playerOnBoard)
            {
                playerOnBoard = true;
                playerObject = playerCollider.gameObject;
                
                // Sauvegarde le parent original et attache le joueur à l'Odyssey si la méthode de parentage est activée
                if (useParentingMethod)
                {
                    originalPlayerParent = playerObject.transform.parent;
                    playerObject.transform.SetParent(transform);
                }
                
                // Récupère et sauvegarde la gravité du joueur
                playerRigidbody = playerObject.GetComponent<Rigidbody2D>();
                if (playerRigidbody != null)
                {
                    originalPlayerGravity = playerRigidbody.gravityScale;
                    playerRigidbody.gravityScale = 0f; // Désactive la gravité
                }                // Désactive les contrôles du joueur
                playerController = playerObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.controlEnabled = false;
                }                // Démarre le timer de disparition
                StartDisappearanceTimer();
                
                // Déclenche l'événement
                OnPlayerMounted?.Invoke(playerObject);
                
                Debug.Log("Joueur monté sur l'Odyssey - Contrôles de l'Odyssey activés, contrôles du joueur désactivés! Timer de 5s démarré.");
            }
        }
        else
        {
            if (playerOnBoard)
            {
                playerOnBoard = false;
                  // Arrête le timer si le joueur descend avant la fin
                StopDisappearanceTimer();
                
                // Déclenche l'événement de descente
                OnPlayerDismounted?.Invoke(playerObject);
                
                // Restaure le parent original si la méthode de parentage était utilisée
                if (useParentingMethod && playerObject != null)
                {
                    playerObject.transform.SetParent(originalPlayerParent);
                }
                
                // Restaure la gravité du joueur
                if (playerRigidbody != null)
                {
                    playerRigidbody.gravityScale = originalPlayerGravity;
                    playerRigidbody = null;
                }                // Restaure les contrôles du joueur
                if (playerController != null)
                {
                    playerController.controlEnabled = true;
                    playerController = null;
                }
                
                playerObject = null;
                Debug.Log("Joueur descendu de l'Odyssey - Contrôles de l'Odyssey désactivés, contrôles du joueur restaurés!");
            }
        }
    }
      void HandleMovementInput()
    {
        Debug.Log("HandleMovementInput appelé - Joueur sur l'odyssey");
        
        // Récupère les inputs du clavier et du joystick
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        Debug.Log($"Input détecté - Horizontal: {horizontalInput}, Vertical: {verticalInput}");
        
        Vector2 inputDirection = new Vector2(horizontalInput, verticalInput).normalized;
        
        if (inputDirection.magnitude > 0)
        {
            // Accélération vers la direction d'input
            Vector2 targetVelocity = inputDirection * moveSpeed;
            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            // Décélération quand aucun input
            ApplyDeceleration();
        }
        
        // Applique le mouvement avec MovePosition pour les corps kinematiques
        Vector2 newPosition = rb.position + currentVelocity * Time.deltaTime;
        rb.MovePosition(newPosition);
    }    void ApplyDeceleration()
    {
        // Décélération graduelle pour les corps kinematiques
        currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);
    }    void MovePlayerWithOdyssey()
    {
        // Si le joueur est sur l'odyssey, le déplace avec lui
        if (playerOnBoard && playerObject != null)
        {
            // Si on utilise la méthode de parentage, le joueur bouge automatiquement avec l'Odyssey
            if (useParentingMethod)
            {
                // Pas besoin de déplacer manuellement le joueur, le système de parentage s'en charge
                return;
            }
            
            // Méthode manuelle de déplacement (pour compatibilité)
            Vector3 odysseyMovement = transform.position - lastPosition;
            
            // Déplace le joueur de la même distance que l'odyssey
            if (odysseyMovement.magnitude > 0.001f) // Évite les micro-mouvements
            {
                // Méthode sophistiquée : utilise la logique de plateforme mobile
                // Pour éviter les conflits avec le système de collision du joueur
                MovePlayerSafely(odysseyMovement);
            }
        }
    }
      void MovePlayerSafely(Vector3 movement)
    {
        if (playerController == null || playerObject == null) return;
        
        // Méthode améliorée qui prend en compte le système de collision Unity
        // Sauvegarde la velocité actuelle du joueur
        Vector2 originalVelocity = playerController.velocity;
        
        // Approche différentielle selon le type de mouvement
        if (Mathf.Abs(movement.y) > 0.01f && Mathf.Abs(movement.x) > 0.01f)
        {
            // Mouvement diagonal - divise en composantes pour éviter les conflits
            MovePlayerInSteps(movement);
        }
        else if (Mathf.Abs(movement.y) > 0.01f)
        {
            // Mouvement principalement vertical
            MovePlayerVertically(movement);
        }
        else if (Mathf.Abs(movement.x) > 0.01f)
        {
            // Mouvement principalement horizontal
            MovePlayerHorizontally(movement);
        }
    }
    
    void MovePlayerInSteps(Vector3 movement)
    {
        // Divise le mouvement en étapes pour éviter les collisions
        Vector3 horizontalMovement = new Vector3(movement.x, 0, 0);
        Vector3 verticalMovement = new Vector3(0, movement.y, 0);
        
        // D'abord le mouvement horizontal
        if (horizontalMovement.magnitude > 0.001f)
        {
            MovePlayerHorizontally(horizontalMovement);
        }
        
        // Puis le mouvement vertical
        if (verticalMovement.magnitude > 0.001f)
        {
            MovePlayerVertically(verticalMovement);
        }
    }
    
    void MovePlayerVertically(Vector3 verticalMovement)
    {
        // Pour les mouvements verticaux, utilise Teleport pour éviter les conflits avec la gravité
        Vector3 newPosition = playerObject.transform.position + verticalMovement;
        playerController.Teleport(newPosition);
        
        // Reset de la vitesse verticale pour éviter les accumulations
        playerController.velocity = new Vector2(playerController.velocity.x, 0);
    }
    
    void MovePlayerHorizontally(Vector3 horizontalMovement)
    {
        // Pour les mouvements horizontaux, utilise MovePosition qui respecte mieux les collisions
        if (playerRigidbody != null)
        {
            Vector2 newPosition = playerRigidbody.position + (Vector2)horizontalMovement;
            playerRigidbody.MovePosition(newPosition);
        }
        else
        {
            // Fallback avec Teleport
            Vector3 newPosition = playerObject.transform.position + horizontalMovement;
            playerController.Teleport(newPosition);
        }
    }
    
    void CreateDebugCircle()
    {
#if UNITY_EDITOR
        if (!showDetectionRadius || Application.isPlaying) return;
        
        // Supprime l'ancien cercle s'il existe
        if (debugCircle != null)
        {
            DestroyImmediate(debugCircle);
        }
        
        // Crée un nouvel objet pour le cercle
        debugCircle = new GameObject("Debug_DetectionRadius");
        debugCircle.transform.SetParent(transform);
        debugCircle.transform.localPosition = Vector3.zero;
          // Ajoute un LineRenderer pour dessiner le cercle
        LineRenderer lineRenderer = debugCircle.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        
        // Calcule les points du cercle
        int segments = 64;
        lineRenderer.positionCount = segments;
        
        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            Vector3 pos = new Vector3(
                Mathf.Cos(angle) * detectionRadius,
                Mathf.Sin(angle) * detectionRadius,
                0f
            );
            lineRenderer.SetPosition(i, pos);
        }
        
        // Cache l'objet en mode play
        debugCircle.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
#endif
    }
    
    void OnValidate()
    {
        // Recrée le cercle quand les paramètres changent dans l'éditeur
        if (!Application.isPlaying)
        {
            CreateDebugCircle();
        }
    }
    
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Affiche la zone de détection du joueur dans l'éditeur uniquement (pas en mode play)
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            
            // Affiche aussi un cercle rempli semi-transparent
            Color fillColor = Color.yellow;
            fillColor.a = 0.1f;
            Gizmos.color = fillColor;
            Gizmos.DrawSphere(transform.position, detectionRadius);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Affiche un cercle plus épais quand sélectionné (uniquement en éditeur)
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            
            // Affiche le centre
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.1f);
            
            // Affiche des lignes pour marquer les directions
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + Vector3.left * detectionRadius, transform.position + Vector3.right * detectionRadius);
            Gizmos.DrawLine(transform.position + Vector3.up * detectionRadius, transform.position + Vector3.down * detectionRadius);
        }
    }
#endif

    // Méthodes de gestion du timer de disparition
    void StartDisappearanceTimer()
    {
        timerStarted = true;
        controlTimer = 0f;
        Debug.Log($"Timer de disparition démarré - {controlTimeLimit}s avant disparition");
    }
    
    void StopDisappearanceTimer()
    {
        timerStarted = false;
        controlTimer = 0f;
        Debug.Log("Timer de disparition arrêté");
    }
    
    void UpdateDisappearanceTimer()
    {
        controlTimer += Time.deltaTime;        // Affiche un warning à 4 secondes
        if (controlTimer >= 4f && controlTimer < 4.1f)
        {
            Debug.Log("ATTENTION: L'Odyssey va disparaître dans 1 seconde!");
            OnTimerWarning?.Invoke();
        }
        
        // Effet de fade si activé
        if (fadeOutEffect && spriteRenderer != null)
        {
            float fadeStartTime = controlTimeLimit - 1f; // Commence le fade 1 seconde avant
            if (controlTimer >= fadeStartTime)
            {
                float fadeProgress = (controlTimer - fadeStartTime) / 1f;
                Color currentColor = spriteRenderer.color;
                currentColor.a = Mathf.Lerp(1f, 0f, fadeProgress);
                spriteRenderer.color = currentColor;
            }
        }
        
        // Disparition après le délai
        if (controlTimer >= controlTimeLimit)
        {
            DisappearOdyssey();
        }
    }
    
    void DisappearOdyssey()
    {
        Debug.Log("L'Odyssey disparaît après 5 secondes de contrôle!");
        
        // Restaure le joueur avant de disparaître
        if (playerOnBoard)
        {
            RestorePlayer();
        }
        
        // Désactive l'objet ou le détruit
        StartCoroutine(DisappearEffect());
    }
    
    void RestorePlayer()
    {
        if (playerObject != null)
        {
            // Restaure le parent original si la méthode de parentage était utilisée
            if (useParentingMethod && originalPlayerParent != null)
            {
                playerObject.transform.SetParent(originalPlayerParent);
            }
            
            // Restaure la gravité du joueur
            if (playerRigidbody != null)
            {
                playerRigidbody.gravityScale = originalPlayerGravity;
            }
            
            // Restaure les contrôles du joueur
            if (playerController != null)
            {
                playerController.controlEnabled = true;
            }
            
            Debug.Log("Joueur restauré après disparition de l'Odyssey");
        }
        
        // Reset des variables
        playerOnBoard = false;
        playerObject = null;
        playerRigidbody = null;
        playerController = null;
        StopDisappearanceTimer();
    }
    
    System.Collections.IEnumerator DisappearEffect()
    {
        // Désactive le collider pour que le joueur ne reste pas collé
        if (odysseyCollider != null)
        {
            odysseyCollider.enabled = false;
        }
        
        // Effet de rétrécissement (optionnel)
        Vector3 originalScale = transform.localScale;
        float shrinkDuration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < shrinkDuration)
        {
            float t = elapsed / shrinkDuration;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Détruit l'objet
        Destroy(gameObject);
    }
}
