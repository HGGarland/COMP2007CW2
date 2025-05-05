using UnityEngine;

public class PlayerKeyCollector : MonoBehaviour
{
    public float interactionRange = 5f; // Range within which the button can be interacted with
    private int keysCollected = 0;
    private const int totalKeys = 3;
    private ExitDoor exitDoor; // Reference to the ExitDoor script
    private Transform playerTransform;
    private GameObject nearestButton;

    void Start()
    {
        exitDoor = FindObjectOfType<ExitDoor>();
        if (exitDoor == null)
        {
            Debug.LogError("ExitDoor script not found in the scene!");
        }
        playerTransform = transform; // Use the player's own transform
    }

    void Update()
    {
        nearestButton = GetNearestButton();
        if (nearestButton != null)
        {
            float distanceToButton = Vector3.Distance(playerTransform.position, nearestButton.transform.position);
            if (distanceToButton <= interactionRange && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log($"Interacting with button: {nearestButton.name}");
                Timer.instance.ActivateKey1(); // Use Timer to activate Key1
                nearestButton.SetActive(false); // Deactivate the button after interaction
            }
        }
    }

    GameObject GetNearestButton()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        foreach (GameObject button in buttons)
        {
            float distance = Vector3.Distance(playerTransform.position, button.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = button;
            }
        }
        return closest;
    }

    public void KeyCollected()
    {
        keysCollected++;
        Debug.Log($"Key collected! Total keys: {keysCollected}/{totalKeys}");
        if (keysCollected >= totalKeys && exitDoor != null)
        {
            exitDoor.OpenDoor();
        }
    }
}