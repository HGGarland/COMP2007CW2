using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 600f; // 10 minutes in seconds
    public TextMeshPro timerText;
    public GameObject gameOverUI;
    public GameObject startScreen;
    public GameObject pauseScreen;
    public GameObject winScreen; // Reference to the WinScreen panel
    public TextMeshProUGUI winTimeText; // Reference to the WinTimeText on the win screen
    private bool timerIsRunning = false;
    private bool gameStarted = false;
    private float startTime; // To calculate the time taken to win

    // Key references
    public GameObject key1; // Associated with button puzzle
    public GameObject key2; // Associated with sequence puzzle
    public GameObject key3; // Associated with bookshelf puzzle
    private List<GameObject> collectedKeys = new List<GameObject>(); // Track collected keys

    // Sound effects
    public AudioClip keyFindSound; // Played when a key becomes visible
    public AudioClip keyCollectSound; // Played when a key is collected (except final)
    public AudioClip doorOpenSound; // Played when the final key is collected
    public AudioClip backingMusic; // Background music clip
    private AudioSource audioSourceEffects; // For sound effects
    private AudioSource audioSourceMusic; // For background music

    // Singleton instance for easy access
    public static Timer instance;
    private PlayerKeyCollector keyCollector;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gameOverUI.SetActive(false);
        startScreen.SetActive(true);
        pauseScreen.SetActive(false);
        winScreen.SetActive(false); // Ensure the win screen is hidden at start
        Time.timeScale = 0f;

        // Hide and deactivate keys at the start
        if (key1 != null) key1.SetActive(false);
        if (key2 != null) key2.SetActive(false);
        if (key3 != null) key3.SetActive(false);

        // Check if timerText is assigned
        if (timerText == null)
        {
            Debug.LogError("TimerText is not assigned in the Inspector!");
        }

        keyCollector = FindObjectOfType<PlayerKeyCollector>();
        if (keyCollector == null)
        {
            Debug.LogError("PlayerKeyCollector not found in the scene!");
        }

        // Setup AudioSources
        audioSourceEffects = gameObject.AddComponent<AudioSource>();
        audioSourceEffects.playOnAwake = false;
        audioSourceMusic = gameObject.AddComponent<AudioSource>();
        audioSourceMusic.playOnAwake = false;
        audioSourceMusic.loop = true; // Enable looping for background music
        if (backingMusic != null)
        {
            audioSourceMusic.clip = backingMusic;
        }
        else
        {
            Debug.LogError("BackingMusic clip is not assigned in the Inspector!");
        }
    }

    void Update()
    {
        // Handle start screen input
        if (!gameStarted && startScreen.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

        // Handle pause and game running logic
        if (timerIsRunning)
        {
            if (timeRemaining > 0.1)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                GameOver();
            }

            // Toggle pause with the "P" key
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (Time.timeScale == 1f)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }
            }
        }
        // Resume with the P key when paused
        else if (pauseScreen.activeSelf && Input.GetKeyDown(KeyCode.P))
        {
            ResumeGame();
        }
        // Handle game over screen input
        else if (gameOverUI.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            ResetGame();
        }
        // Handle win screen input
        else if (winScreen.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            ResetGame();
        }

        // Manage background music state
        UpdateMusicState();
    }

    void UpdateTimerDisplay()
    {
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);
        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void GameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
        Invoke("ResetGame", 3f); // Optional: Remove if you only want manual reset
    }

    public void ResetGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void StartGame()
    {
        startScreen.SetActive(false);
        Time.timeScale = 1f;
        timerIsRunning = true;
        gameStarted = true;
        startTime = Time.time; // Record the start time for calculating win time
        PlayMusic();
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        timerIsRunning = false;
        pauseScreen.SetActive(true);
        PauseMusic();
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        timerIsRunning = true;
        pauseScreen.SetActive(false);
        PlayMusic();
    }

    public void WinGame()
    {
        // Calculate the time taken to win
        float timeTaken = Time.time - startTime;
        float minutes = Mathf.FloorToInt(timeTaken / 60);
        float seconds = Mathf.FloorToInt(timeTaken % 60);
        winTimeText.text = $"Time: {minutes:00}:{seconds:00}";
        // Show the win screen and pause the game
        winScreen.SetActive(true);
        Time.timeScale = 0f;
        timerIsRunning = false;
        PauseMusic();
        Debug.Log("WinGame executed, winScreen set to active.");
    }

    // Methods to activate keys when puzzles are completed
    public void ActivateKey1()
    {
        if (key1 != null)
        {
            key1.SetActive(true);
            Debug.Log("Key1 activated!");
            PlayKeyFindSound();
        }
        else
        {
            Debug.LogError("Key1 is not assigned in the Timer script!");
        }
    }

    public void ActivateKey2()
    {
        if (key2 != null)
        {
            key2.SetActive(true);
            Debug.Log("Key2 activated!");
            PlayKeyFindSound();
        }
        else
        {
            Debug.LogError("Key2 is not assigned in the Timer script!");
        }
    }

    public void ActivateKey3()
    {
        if (key3 != null)
        {
            key3.SetActive(true);
            Debug.Log("Key3 activated!");
            PlayKeyFindSound();
        }
        else
        {
            Debug.LogError("Key3 is not assigned in the Timer script!");
        }
    }

    private void PlayKeyFindSound()
    {
        if (keyFindSound != null)
        {
            audioSourceEffects.PlayOneShot(keyFindSound);
        }
    }

    // Method to handle key collection
    public void CollectKey(GameObject key)
    {
        if (!collectedKeys.Contains(key))
        {
            collectedKeys.Add(key);
            Debug.Log($"Key collected: {key.name}. Total collected: {collectedKeys.Count}");
            if (keyCollector != null)
            {
                keyCollector.KeyCollected(); // Notify PlayerKeyCollector
            }
            if (collectedKeys.Count == 3) // Now expecting 3 keys
            {
                Debug.Log("All keys collected!");
                if (doorOpenSound != null)
                {
                    audioSourceEffects.PlayOneShot(doorOpenSound); // Play DoorOpen sound for final key
                }
            }
            else
            {
                if (keyCollectSound != null)
                {
                    audioSourceEffects.PlayOneShot(keyCollectSound); // Play KeyCollect sound for non-final keys
                }
            }
        }
    }

    private void UpdateMusicState()
    {
        if (startScreen.activeSelf || pauseScreen.activeSelf || winScreen.activeSelf || gameOverUI.activeSelf)
        {
            if (audioSourceMusic.isPlaying)
            {
                PauseMusic();
            }
        }
        else if (timerIsRunning && !audioSourceMusic.isPlaying)
        {
            PlayMusic();
        }
    }

    private void PlayMusic()
    {
        if (backingMusic != null && !audioSourceMusic.isPlaying)
        {
            audioSourceMusic.Play();
        }
    }

    private void PauseMusic()
    {
        if (audioSourceMusic.isPlaying)
        {
            audioSourceMusic.Pause();
        }
    }
}