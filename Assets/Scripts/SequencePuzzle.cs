using UnityEngine;

public class SequencePuzzle : MonoBehaviour
{
    private int[] correctSequence = { 2, 0, 1 }; // Sequence: pedestal2 (Book C), pedestal0 (Book A), pedestal1 (Book B)
    private int currentStep = 0;
    public GameObject[] pedestals; // Assign in Inspector: [0] = Book A, [1] = Book B, [2] = Book C
    public float interactionRange = 2f; // Distance within which player can interact
    public AudioClip lowSound; // First interaction sound
    public AudioClip mediumSound; // Second interaction sound
    public AudioClip highSound; // Third interaction sound
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject closestPedestal = GetClosestPedestal();
            if (closestPedestal != null)
            {
                int pedestalIndex = System.Array.IndexOf(pedestals, closestPedestal);
                TryInteractWithPedestal(pedestalIndex);
            }
        }
    }

    GameObject GetClosestPedestal()
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        foreach (GameObject pedestal in pedestals)
        {
            float distance = Vector3.Distance(transform.position, pedestal.transform.position);
            if (distance < minDistance && distance < interactionRange)
            {
                minDistance = distance;
                closest = pedestal;
            }
        }
        return closest;
    }

    void TryInteractWithPedestal(int pedestalIndex)
    {
        if (pedestalIndex == correctSequence[currentStep])
        {
            Debug.Log($"Correct pedestal: {pedestalIndex}");
            // Play the appropriate sound based on the current step
            if (currentStep == 0 && lowSound != null)
            {
                audioSource.PlayOneShot(lowSound);
            }
            else if (currentStep == 1 && mediumSound != null)
            {
                audioSource.PlayOneShot(mediumSound);
            }
            else if (currentStep == 2 && highSound != null)
            {
                audioSource.PlayOneShot(highSound);
            }
            currentStep++;
            if (currentStep == correctSequence.Length)
            {
                Timer.instance.ActivateKey2();
                Debug.Log("Sequence complete! Key2 is now collectible.");
            }
        }
        else
        {
            Debug.Log("Incorrect pedestal! Sequence reset.");
            currentStep = 0;
        }
    }
}