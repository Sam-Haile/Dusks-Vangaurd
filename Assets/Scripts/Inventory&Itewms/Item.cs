using TMPro;
using UnityEngine;

/// <summary>
/// General Item Types
/// </summary>
public enum ItemType
{
    Weapon,
    Armor,
    KeyItem,
    Consumable
}

/// <summary>
/// More specific items/consumable
/// </summary>
public enum SpecificType
{
    Sword,
    Bow,
    Shield,
    Plating,
    NonEquippable
}

public class Item : MonoBehaviour
{
    public ItemType Type;

    public SpecificType specificType;
    public string itemName;
    public int itemID;
    public Sprite itemIcon;
    public bool keyDown = false;
    public bool collected = false;

    public TextMeshPro textBox;

    private void Start()
    {
        if (textBox != null)
        {
            textBox.text = itemName;
            textBox.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            keyDown = true;
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            keyDown = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            textBox.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            textBox.gameObject.SetActive(false);
    }

    private void OnMouseEnter()
    {
        textBox.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        textBox.gameObject.SetActive(false);
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && keyDown)
        {
            Inventory.instance.AddItem(this);
            collected = true;
            gameObject.SetActive(false);
            keyDown = false;
        }
    }

}
