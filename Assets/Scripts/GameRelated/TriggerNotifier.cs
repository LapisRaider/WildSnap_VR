using UnityEngine;

public class TriggerNotifier : MonoBehaviour
{
    public delegate void OnTriggerHandler(bool entered);
    public OnTriggerHandler onTriggerCallback;

    public string m_tag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != m_tag)
            return;

        if (onTriggerCallback != null)
            onTriggerCallback.Invoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != m_tag)
            return;

        if (onTriggerCallback != null)
            onTriggerCallback.Invoke(false);
    }
}
