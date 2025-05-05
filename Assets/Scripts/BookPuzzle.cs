using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BookPuzzle : MonoBehaviour
{
    private GameObject carriedBook;
    private GameObject nearbyBook;
    public GameObject keyToUnlock;
    public GameObject winMessage;
    public float interactionRange = 5f;
    public Transform bookshelfTransform;

    // UI Elements
    public TextMeshProUGUI bookPickupPopup;
    public GameObject bookshelfPopup;
    public TextMeshProUGUI slot1Text, slot2Text, slot3Text;

    // Specific book references
    public GameObject book1; // Assign "Absolution 101" in Inspector
    public GameObject book2; // Assign "Chronicles of Eldoria" in Inspector
    public GameObject book3; // Assign "Oblivion" in Inspector

    private GameObject[] booksInSlots = new GameObject[3];
    private float popupTimer = 0f;
    private const float POPUP_DURATION = 2f;
    private Dictionary<GameObject, Vector3> bookSpawnPositions = new Dictionary<GameObject, Vector3>(); // Store spawn positions

    void Start()
    {
        if (bookPickupPopup != null) bookPickupPopup.gameObject.SetActive(false);
        if (bookshelfPopup != null) bookshelfPopup.SetActive(false);
        UpdateSlotTexts();

        // Store initial spawn positions of books
        if (book1 != null) bookSpawnPositions[book1] = book1.transform.position;
        if (book2 != null) bookSpawnPositions[book2] = book2.transform.position;
        if (book3 != null) bookSpawnPositions[book3] = book3.transform.position;
    }

    void Update()
    {
        if (bookPickupPopup != null && bookPickupPopup.gameObject.activeSelf)
        {
            popupTimer -= Time.deltaTime;
            if (popupTimer <= 0)
            {
                bookPickupPopup.gameObject.SetActive(false);
            }
        }

        CheckForNearbyBooks();

        float distanceToBookshelf = Vector3.Distance(transform.position, bookshelfTransform.position);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (bookshelfPopup.activeSelf)
            {
                CloseBookshelfPopup();
            }
            else if (distanceToBookshelf < interactionRange)
            {
                ShowBookshelfPopup();
            }
            else if (carriedBook == null && nearbyBook != null)
            {
                PickUpBook(nearbyBook);
            }
            else if (carriedBook != null)
            {
                DropBook();
            }
        }

        if (bookshelfPopup.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                InteractWithSlot(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                InteractWithSlot(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                InteractWithSlot(2);
            }
        }
    }

    void CheckForNearbyBooks()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, interactionRange);
        nearbyBook = null;
        foreach (Collider obj in nearbyObjects)
        {
            if (obj.CompareTag("Book"))
            {
                nearbyBook = obj.gameObject;
                break;
            }
        }
    }

    void PickUpBook(GameObject book)
    {
        carriedBook = book;
        carriedBook.GetComponent<Collider>().enabled = false;
        carriedBook.transform.SetParent(transform);
        carriedBook.transform.localPosition = new Vector3(0, 1, 0);
        if (bookPickupPopup != null)
        {
            bookPickupPopup.text = $"Picked up: {carriedBook.name}";
            bookPickupPopup.gameObject.SetActive(true);
            popupTimer = POPUP_DURATION;
        }
    }

    void DropBook()
    {
        if (carriedBook != null)
        {
            carriedBook.GetComponent<Collider>().enabled = true;
            carriedBook.transform.SetParent(null);
            // Teleport to initial spawn position
            if (bookSpawnPositions.ContainsKey(carriedBook))
            {
                carriedBook.transform.position = bookSpawnPositions[carriedBook];
            }
            else
            {
                Debug.LogWarning($"No spawn position recorded for book {carriedBook.name}");
            }
            carriedBook.SetActive(true);
            carriedBook = null;
        }
    }

    void ShowBookshelfPopup()
    {
        if (bookshelfPopup != null)
        {
            bookshelfPopup.SetActive(true);
            Time.timeScale = 0f;
            UpdateSlotTexts();
        }
    }

    void CloseBookshelfPopup()
    {
        if (bookshelfPopup != null)
        {
            bookshelfPopup.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    void InteractWithSlot(int slotIndex)
    {
        if (carriedBook != null && booksInSlots[slotIndex] == null)
        {
            PlaceInSlot(slotIndex);
        }
        else if (carriedBook == null && booksInSlots[slotIndex] != null)
        {
            TakeFromSlot(slotIndex);
        }
    }

    void PlaceInSlot(int slotIndex)
    {
        booksInSlots[slotIndex] = carriedBook;
        carriedBook.SetActive(false);
        carriedBook.transform.SetParent(null);
        carriedBook = null;
        UpdateSlotTexts();
        CheckWinCondition();
    }

    void TakeFromSlot(int slotIndex)
    {
        carriedBook = booksInSlots[slotIndex];
        booksInSlots[slotIndex] = null;
        carriedBook.SetActive(true);
        carriedBook.transform.SetParent(transform);
        carriedBook.transform.localPosition = new Vector3(0, 1, 0);
        carriedBook.GetComponent<Collider>().enabled = false;
        if (bookPickupPopup != null)
        {
            bookPickupPopup.text = $"Picked up: {carriedBook.name}";
            bookPickupPopup.gameObject.SetActive(true);
            popupTimer = POPUP_DURATION;
        }
        UpdateSlotTexts();
    }

    void UpdateSlotTexts()
    {
        if (slot1Text != null)
        {
            slot1Text.text = booksInSlots[0] == null ? "Slot 1: Empty" : $"Slot 1: Filled by {booksInSlots[0].name}";
        }
        if (slot2Text != null)
        {
            slot2Text.text = booksInSlots[1] == null ? "Slot 2: Empty" : $"Slot 2: Filled by {booksInSlots[1].name}";
        }
        if (slot3Text != null)
        {
            slot3Text.text = booksInSlots[2] == null ? "Slot 3: Empty" : $"Slot 3: Filled by {booksInSlots[2].name}";
        }
    }

    void CheckWinCondition()
    {
        if (booksInSlots[0] == book1 && booksInSlots[1] == book2 && booksInSlots[2] == book3)
        {
            PuzzleCompleted();
        }
    }

    void PuzzleCompleted()
    {
        Timer.instance.ActivateKey3(); // Activate Key 3 via Timer
        if (winMessage != null)
        {
            winMessage.SetActive(true);
        }
    }
}