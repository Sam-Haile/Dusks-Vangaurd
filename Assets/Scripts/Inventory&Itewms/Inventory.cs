using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{

    public static Inventory instance { get; private set; }

    public Dictionary<ItemType, List<Item>> itemsByType = new Dictionary<ItemType, List<Item>>
    {
        { ItemType.Weapon, new List<Item>() },
        { ItemType.Armor, new List<Item>() },
        { ItemType.KeyItem, new List<Item>() },
        { ItemType.Consumable, new List<Item>() }
    };

    public PlayableCharacter player1;
    public PlayableCharacter player2;

    public List<Button> itemsSlots= new List<Button>();
    private ItemType currentType;
    private Item chosenItem;

    public Button[] buttons = new Button[6];

    public Sprite emptySprite;
    public event Action OnItemEquipped;
   
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MenuManager.OnInvOpened += () => DisplayItems(ItemType.Weapon);

        // Add click listeners to each item slot
        for (int i = 0; i < itemsSlots.Count; i++)
        {
            int index = i; // Important to capture the current index in a local variable
            itemsSlots[i].GetComponent<Button>().onClick.AddListener(() => OnItemSlotClicked(index));
        }
    }


    private void OnItemSlotClicked(int index)
    {
        // Get the corresponding item type currently displayed
        ItemType currentItemType = currentType; 
        if (itemsByType.ContainsKey(currentItemType) && index < itemsByType[currentItemType].Count)
        {
            chosenItem = itemsByType[currentItemType][index];

            buttons[0].gameObject.SetActive(true);
            buttons[1].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Adds an item to it's respective type
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item)
    {
        itemsByType[item.Type].Add(item);
    }

    /// <summary>
    /// Removes the item from the dictionary
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(Item item)
    {
        if (itemsByType.ContainsKey(item.Type))
        {
            if (itemsByType[item.Type].Contains(item))
            {
                itemsByType[item.Type].Remove(item);
                Debug.Log("Item removed successfully: " + item.itemName);

            }
            else
            {
                Debug.LogWarning("Item not found in the inventory: " + item.itemName);
            }
        }
        else
        {
            Debug.LogWarning("Invalid item type: " + item.Type);
        }
    }

    /// <summary>
    ///  Removes the requested item from your inventory slots
    /// </summary>
    public void RemoveItemInv()
    {
        RemoveItem(chosenItem);
        // Reload the current inventory section you are in
        DisplayItems(chosenItem.Type);
    }

    // The following methods refresh the various sections in the inventory
    public void DisplayWeaponsButton()
    {
        DisplayItems(ItemType.Weapon);
    }

    public void DisplayArmorButton()
    {
        DisplayItems(ItemType.Armor);
    }

    public void DisplayKeyItemsButton()
    {
        DisplayItems(ItemType.KeyItem);
    }

    public void DisplayConsumablesButton()
    {
        DisplayItems(ItemType.Consumable);
    }

    /// <summary>
    /// Displays the icons of each item in your inventory.
    /// </summary>
    /// <param name="type"></param>
    private void DisplayItems(ItemType type)
    {
        currentType = type;
        ClearInventorySlots();
        // Example: Displaying only weapon type items
        var item = itemsByType[type];
        for (int i = 0; i < item.Count; i++)
        {
            itemsSlots[i].image.sprite = item[i].itemIcon;
            itemsSlots[i].enabled = true;
            itemsSlots[i].tag = "ItemSlot";
        }
    }

    /// <summary>
    /// Clears the inventory upon instantiation
    /// </summary>
    private void ClearInventorySlots()
    {
        foreach (var slot in itemsSlots)
        {
            slot.image.sprite = emptySprite;
            slot.enabled = false;
        }
    }

    /// <summary>
    /// Prevents buttons from being pressed
    /// </summary>
    public void DeactivateButtons()
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Equp an item to a player
    /// </summary>
    /// <param name="playerToEquip"> Either player 1 or 2</param>
    public void EquipItem(PlayableCharacter playerToEquip)
    {
        // Check which player you are equipping the item for (player1 or player2)
        if (chosenItem.Type == ItemType.Weapon)
        {
            if (playerToEquip.equippedWeapon != null)
            {
                AddItem(playerToEquip.equippedWeapon);
            }

            playerToEquip.equippedWeapon = chosenItem.GetComponent<Weapon>();

            // Remove the new item from the inventory
            RemoveItem(chosenItem);

            DisplayItems(ItemType.Weapon);
        }
        else if (chosenItem.Type == ItemType.Armor)
        {
            if (playerToEquip.equippedArmor != null)
            {
                AddItem(playerToEquip.equippedArmor);
            }

            playerToEquip.equippedArmor = chosenItem.GetComponent<Armor>();

            // Remove the new item from the inventory
            RemoveItem(chosenItem);

            DisplayItems(ItemType.Armor);
        }

        OnItemEquipped?.Invoke();
    }    

    /// <summary>
    /// Unequips an item from a player
    /// </summary>
    /// <param name="playerToEquip">Either player 1 or 2</param>
    public void UnequipWeapon(PlayableCharacter playerToEquip)
    {
        if (playerToEquip.equippedWeapon != null)
        {
            // Add the equipped weapon back to the inventory
            AddItem(playerToEquip.equippedWeapon);

            // Remove the icon from the weapon slot
            playerToEquip.weaponSlot.GetComponent<Image>().sprite = null;

            // Remove the weapon from the player's equipment
            playerToEquip.equippedWeapon = null;

            // Update the UI
            DisplayItems(ItemType.Weapon);
        }
        else
        {
            Debug.LogWarning("No item to unequip for Person");
        }

        OnItemEquipped?.Invoke();
    }

    public void UnequipArmor(PlayableCharacter playerToEquip)
    {
        if (playerToEquip.equippedArmor != null)
        {
            // Add the equipped weapon back to the inventory
            AddItem(playerToEquip.equippedArmor);

            // Remove the icon from the weapon slot
            playerToEquip.armorSlot.GetComponent<Image>().sprite = null;

            // Remove the weapon from the player's equipment
            playerToEquip.equippedArmor = null;

            // Update the UI
            DisplayItems(ItemType.Armor);
        }
        else
        {
            Debug.Log("No item to unequip for Person");
        }

        OnItemEquipped?.Invoke();
    }

    private void OnDestroy()
    {
        MenuManager.OnInvOpened -= () => DisplayItems(ItemType.Weapon);
    }
}
