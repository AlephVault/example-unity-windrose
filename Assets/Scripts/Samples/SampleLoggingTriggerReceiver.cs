using UnityEngine;
using WindRose.Behaviours.Entities.Objects;

[RequireComponent(typeof(TriggerPlatform))]
class SampleLoggingTriggerReceiver : MonoBehaviour
{
    private TriggerPlatform platform;
    
    [SerializeField]
    private bool logEnter = true;

    [SerializeField]
    private bool logExit = true;

    [SerializeField]
    private bool logMoved = true;

    [SerializeField]
    private bool logStay = false;

    void Awake()
    {
        platform = GetComponent<TriggerPlatform>();
    }

    // Use this for initialization
    void Start()
    {
        if (logEnter)
        {
            platform.onMapTriggerEnter.AddListener((obj, platform, x, y) =>
            {
                Debug.Log(string.Format("Entering the trigger (currently, object is at ({0}, {1}))", x, y));
            });
        }

        if (logExit)
        {
            platform.onMapTriggerExit.AddListener((obj, platform, x, y) =>
            {
                Debug.Log(string.Format("Leaving the trigger (currently, object is at ({0}, {1}))", x, y));
            });
        }

        if (logMoved)
        {
            platform.onMapTriggerMoved.AddListener((obj, platform, x, y) =>
            {
                Debug.Log(string.Format("Moved the object in the trigger (currently, object is at ({0}, {1}))", x, y));
            });
        }

        if (logStay)
        {
            platform.onMapTriggerStay.AddListener((obj, platform, x, y) =>
            {
                Debug.Log(string.Format("Staying in the trigger (currently, object is at ({0}, {1}))", x, y));
            });
        }
    }
}
