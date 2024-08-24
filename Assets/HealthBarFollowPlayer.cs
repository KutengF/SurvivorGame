using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarFollowPlayer : MonoBehaviour
{
    public Transform player;            // Reference to the player's transform
    public RectTransform healthBar;     // Reference to the health bar's RectTransform in the UI
    public Camera mainCamera;           // Reference to the main camera
    public float yOffset = -30f;        // Negative offset to move the health bar below the player

    void Update()
    {
        // Convert the player's world position to screen space
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(player.position);

        // Adjust the Y position downward by the offset to place the health bar below the player
        screenPosition.y += yOffset;

        // Update the health bar's position in screen space
        healthBar.position = screenPosition;
    }
}
