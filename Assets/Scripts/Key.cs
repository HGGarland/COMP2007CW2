using UnityEngine;

public class Key : MonoBehaviour
{
    public float collectionRange = 2f; // Range within which the key can be collected
    private bool isGlowing = false;
    private Transform playerTransform;

    void Update()
    {
        if (!gameObject.activeSelf) return;

        // Find player transform if not already set
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                Debug.Log($"Player found for key {gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"Player with tag 'Player' not found for key {gameObject.name}!");
                return;
            }
        }

        if (!isGlowing)
        {
            StartGlow();
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= collectionRange)
        {
            Debug.Log($"Player within range of key {gameObject.name}: Distance = {distanceToPlayer}");
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log($"Collecting key {gameObject.name}");
                CollectKey();
            }
        }
    }

    void CollectKey()
    {
        Timer.instance.CollectKey(gameObject);
        gameObject.SetActive(false); // Hide the key after collection
    }

    void StartGlow()
    {
        isGlowing = true;
        StartCoroutine(GlowEffect());
    }

    System.Collections.IEnumerator GlowEffect()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null || renderer.material == null)
        {
            Debug.LogError($"Key {gameObject.name} has no Renderer or material assigned!");
            yield break;
        }

        Color baseColor = Color.yellow; // Base color of the key
        Color glowColor = Color.yellow * 2f; // Brighter glow color
        float duration = 1.5f; // Duration of one glow cycle
        float elapsedTime = 0f;

        while (gameObject.activeSelf)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.PingPong(elapsedTime / duration, 1f);
            Color currentColor = Color.Lerp(baseColor, glowColor, t);
            renderer.material.SetColor("_EmissionColor", currentColor);
            yield return null;
        }
    }
}