using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LocationNameUpdater : MonoBehaviour
{
    [HideInInspector]public string locationNameText; // Assign this in the inspector

    private void Start()
    {
        UpdateLocationName();
    }

    private void UpdateLocationName()
    {
        // Get the current scene's name
        string sceneName = SceneManager.GetActiveScene().name;

        // Update the location name based on the scene name
        // You can modify this switch statement based on your scene names and corresponding location names
        switch (sceneName)
        {
            case "MainWorld":
                locationNameText = "Mystic Temple";
                break;
            case "CityScene":
                locationNameText = "Neo City";
                break;
            // Add more cases as per your scene names
            default:
                locationNameText = "Unknown Location";
                break;
        }
    }
}
