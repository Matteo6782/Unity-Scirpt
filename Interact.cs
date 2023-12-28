/*
 * Interact Script:
 * Use this script to create an interactive object in a Unity game. The script adjusts the color and rotation of a TextMeshPro object based on the distance and orientation relative to the player.
 * The color fades in/out based on proximity, and the rotation of the TextMeshPro object is constrained within a specified angle.
 *
*/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private float R, G, B; // RGB values for the text color
    [Space]
    [SerializeField] private float radius, maxRotationAngle; // Interaction radius and maximum rotation angle
    [SerializeField] private Vector3 fixedRot; // Fixed rotation for the TextMeshPro object
    [SerializeField] private GameObject textObj; // Reference to the TextMeshPro object
    [SerializeField] private string textToShow; // Text to display

    private GameObject target; // Reference to the player GameObject

    private TextMeshPro tmp;
    private Color startColor; // Color with alpha set to 0
    private Color endColor; // Color with alpha set to 1

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player"); // Find the GameObject with the "Player" tag and set it as the target

        startColor = new Color(R, G, B, 0);
        endColor = new Color(R, G, B, 1);
    }

    private void Start()
    {
        if (!textObj)
            return;

        tmp = textObj.GetComponent<TextMeshPro>();
        tmp.SetText(textToShow); // Set the text to display
        tmp.color = startColor; // Initialize the text color to startColor
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        float alpha = Mathf.InverseLerp(0, radius, distance); // Calculate alpha based on the distance within the interaction radius

        Color currentColor = Color.Lerp(endColor, startColor, alpha); // Interpolate color based on alpha
        tmp.color = currentColor; // Set the TextMeshPro color

        Vector3 targetDirection = target.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion limitedRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationAngle); // Limit rotation within the specified angle

        textObj.transform.rotation = limitedRotation * Quaternion.Euler(fixedRot); // Apply the limited rotation and fixed rotation to the TextMeshPro object
    }
}
