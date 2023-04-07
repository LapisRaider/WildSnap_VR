using UnityEngine;

public class TriggerNotifier : MonoBehaviour
{
    public delegate void OnTriggerHandler(bool entered);
    public OnTriggerHandler onTriggerCallback;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            onTriggerCallback.Invoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            onTriggerCallback.Invoke(false);
    }
}
