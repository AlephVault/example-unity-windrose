using UnityEngine;
using WindRose.Behaviours;

[RequireComponent(typeof(TriggerReceiver))]
class SampleLoggingTriggerReceiver : MonoBehaviour
{
    private TriggerReceiver receiver;
    
    [SerializeField]
    private bool logEnter = true;

    [SerializeField]
    private bool logExit = true;

    [SerializeField]
    private bool logMoved = true;

    [SerializeField]
    private bool logStay = false;

    // Use this for initialization
    void Start()
    {
        receiver = GetComponent<TriggerReceiver>();
        if (logEnter)
        {
            receiver.AddOnMapTriggerEnterListener((obj, platform, x, y) =>
            {
                Debug.Log(string.Format("Entering the trigger (currently, object is at ({0}, {1})", x, y));
            });
        }

        if (logExit)
        {
            receiver.AddOnMapTriggerExitListener((obj, platform, x, y) =>
            {
                Debug.Log(string.Format("Leaving the trigger (currently, object is at ({0}, {1})", x, y));
            });
        }

        if (logMoved)
        {
            receiver.AddOnMapTriggerMovedListener((obj, platform, x, y) =>
            {
                Debug.Log(string.Format("Moved the object in the trigger (currently, object is at ({0}, {1})", x, y));
            });
        }

        if (logStay)
        {
            receiver.AddOnMapTriggerStayListener((obj, platform, x, y) =>
            {
                Debug.Log(string.Format("Staying in the trigger (currently, object is at ({0}, {1})", x, y));
            });
        }
    }
}
