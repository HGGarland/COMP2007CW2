using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private bool doorOpened = false;
    private MeshRenderer doorRenderer; // Reference to the door's renderer

    void Start()
    {
        doorRenderer = GetComponent<MeshRenderer>();
        if (doorRenderer == null)
        {
            Debug.LogError("No MeshRenderer found on ExitDoor!");
        }
    }

    public void OpenDoor()
    {
        if (doorRenderer != null)
        {
            doorRenderer.enabled = false; // Hide the door but keep the GameObject active
        }
        doorOpened = true;
        Debug.Log("Door opened! doorOpened is now: " + doorOpened);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"ExitDoor trigger entered by: {other.gameObject.name} with tag: {other.tag}, doorOpened: {doorOpened}");

        if (other.CompareTag("Player") && doorOpened)
        {
            Debug.Log("Win condition triggered! Calling WinGame...");
            Timer timer = FindObjectOfType<Timer>();
            if (timer != null)
            {
                timer.WinGame();
                Debug.Log("WinGame called successfully.");
            }
            else
            {
                Debug.LogError("Timer script not found in the scene!");
            }
        }
    }
}