using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OdysseyEvent : UnityEvent<OdysseyController> { }

[System.Serializable]
public class PlayerOdysseyEvent : UnityEvent<OdysseyController, GameObject> { }

public class OdysseyEventManager : MonoBehaviour
{
    [Header("Events")]
    public OdysseyEvent OnOdysseySpawned = new OdysseyEvent();
    public PlayerOdysseyEvent OnPlayerMountedOdyssey = new PlayerOdysseyEvent();
    public PlayerOdysseyEvent OnPlayerDismountedOdyssey = new PlayerOdysseyEvent();
    public OdysseyEvent OnOdysseyDisappeared = new OdysseyEvent();
    public UnityEvent OnTimerWarning = new UnityEvent();
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip mountSound;
    public AudioClip dismountSound;
    public AudioClip warningSound;
    public AudioClip disappearSound;
    
    private static OdysseyEventManager _instance;
    public static OdysseyEventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<OdysseyEventManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("OdysseyEventManager");
                    _instance = go.AddComponent<OdysseyEventManager>();
                }
            }
            return _instance;
        }
    }
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        
        // Configure l'AudioSource si pas assigné
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }
    
    // Méthodes statiques pour faciliter l'utilisation
    public static void TriggerOdysseySpawned(OdysseyController odyssey)
    {
        Instance.OnOdysseySpawned?.Invoke(odyssey);
        Debug.Log($"Event: Odyssey spawné - {odyssey.name}");
    }
    
    public static void TriggerPlayerMounted(OdysseyController odyssey, GameObject player)
    {
        Instance.OnPlayerMountedOdyssey?.Invoke(odyssey, player);
        Instance.PlaySound(Instance.mountSound);
        Debug.Log($"Event: Joueur {player.name} monté sur {odyssey.name}");
    }
    
    public static void TriggerPlayerDismounted(OdysseyController odyssey, GameObject player)
    {
        Instance.OnPlayerDismountedOdyssey?.Invoke(odyssey, player);
        Instance.PlaySound(Instance.dismountSound);
        Debug.Log($"Event: Joueur {player.name} descendu de {odyssey.name}");
    }
    
    public static void TriggerOdysseyDisappeared(OdysseyController odyssey)
    {
        Instance.OnOdysseyDisappeared?.Invoke(odyssey);
        Instance.PlaySound(Instance.disappearSound);
        Debug.Log($"Event: Odyssey disparu - {odyssey.name}");
    }
    
    public static void TriggerTimerWarning()
    {
        Instance.OnTimerWarning?.Invoke();
        Instance.PlaySound(Instance.warningSound);
        Debug.Log("Event: Avertissement timer Odyssey");
    }
    
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    // Méthodes pour enregistrer des callbacks
    public static void RegisterOnPlayerMounted(UnityAction<OdysseyController, GameObject> callback)
    {
        Instance.OnPlayerMountedOdyssey.AddListener(callback);
    }
    
    public static void UnregisterOnPlayerMounted(UnityAction<OdysseyController, GameObject> callback)
    {
        Instance.OnPlayerMountedOdyssey.RemoveListener(callback);
    }
    
    public static void RegisterOnPlayerDismounted(UnityAction<OdysseyController, GameObject> callback)
    {
        Instance.OnPlayerDismountedOdyssey.AddListener(callback);
    }
    
    public static void UnregisterOnPlayerDismounted(UnityAction<OdysseyController, GameObject> callback)
    {
        Instance.OnPlayerDismountedOdyssey.RemoveListener(callback);
    }
    
    public static void RegisterOnTimerWarning(UnityAction callback)
    {
        Instance.OnTimerWarning.AddListener(callback);
    }
    
    public static void UnregisterOnTimerWarning(UnityAction callback)
    {
        Instance.OnTimerWarning.RemoveListener(callback);
    }
}
