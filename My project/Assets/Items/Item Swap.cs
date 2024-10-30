using UnityEngine;
using UnityEngine.UI;

public class ItemSwitcher : MonoBehaviour
{
    public GameObject[] itemPrefabs;       // Prefabs for each item
    public Image[] itemIcons;              // UI Icons for HUD
    public Transform spawnPoint;           // Assign spawn point in the Inspector

    private GameObject currentItem;        // Reference to the currently active item
    private int currentIndex = 0;

    void Start()
    {
        // Initial spawn of the first item
        SpawnItem(currentIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToItem(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToItem(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchToItem(2);
    }

    void SwitchToItem(int index)
    {
        if (index != currentIndex)  // Only switch if a new item is selected
        {
            // Destroy current item
            if (currentItem != null) Destroy(currentItem);

            // Update current index and spawn the new item
            currentIndex = index;
            SpawnItem(currentIndex);

            // Update HUD display
            UpdateHUD();
        }
    }

    void SpawnItem(int index)
    {
        currentItem = Instantiate(itemPrefabs[index], spawnPoint.position, spawnPoint.rotation);
        currentItem.transform.SetParent(spawnPoint); // Optional: Parent to keep organized
    }

    void UpdateHUD()
    {
        for (int i = 0; i < itemIcons.Length; i++)
        {
            itemIcons[i].color = i == currentIndex ? Color.white : Color.gray;
        }
    }
}
