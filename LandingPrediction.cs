/*
 * LandingPrediction Script:
 * Use this script to predict and visualize the landing point for a Unity game. The script calculates the landing point based on the player's current position, velocity, and gravity.
 * It also displays a visual indicator (landingPoint) and arrow to guide the player during jetpack use. Different materials are applied to indicate whether landing is possible or not.
 *
*/

using UnityEngine;

public class LandingPrediction : MonoBehaviour
{
    [Header("Prediction Settings")]
    public float timeToPredict = 2f;
    public int predictionSteps = 100;
    public float smoothAmount;

    [Header("GameObjects References")]
    public GameObject landingPoint; // Reference to the landing point indicator
    public GameObject arrow; // Reference to the arrow indicator
    public LayerMask groundLayer; // Layer of ground objects

    [Header("Materials")]
    [SerializeField] private Material canLandMaterial;
    [SerializeField] private Material cantLandMaterial;

    private Rigidbody rb;
    private Movement playerMovement; // Reference to the "Movement" script
    private bool isUsingJetpack; // Flag to check if the player is using the jetpack

    private MeshRenderer landingPointRenderer;
    private MeshRenderer arrowRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        landingPointRenderer = landingPoint.GetComponent<MeshRenderer>();
        arrowRenderer = arrow.GetComponent<MeshRenderer>();
        playerMovement = GetComponent<Movement>();
    }

    private void Update()
    {
        isUsingJetpack = !playerMovement.IsGrounded();

        landingPoint.SetActive(isUsingJetpack);
        arrow.SetActive(isUsingJetpack);

        if (isUsingJetpack)
        {
            float timeIncrement = timeToPredict / predictionSteps;

            Vector3 predictedPosition = transform.position;
            Vector3 predictedVelocity = rb.velocity;

            // Simulate future positions based on current velocity and gravity
            for (int i = 0; i < predictionSteps; i++)
            {
                predictedPosition += predictedVelocity * timeIncrement;
                predictedVelocity += Physics.gravity * timeIncrement;
            }

            Debug.DrawLine(transform.position, predictedPosition, Color.red);

            RaycastHit hit;
            // Cast a ray towards the predicted position to check for obstacles
            if (Physics.Raycast(transform.position, predictedPosition - transform.position, out hit, Mathf.Infinity, groundLayer))
            {
                // Update landing indicators if a landing point is found
                UpdateLandingIndicators(hit.point, canLandMaterial);
            }
            else
            {
                // If no direct path is found, find the closest ground and update indicators
                RaycastHit closestHit = FindClosestGround();
                if (closestHit.collider != null)
                {
                    UpdateLandingIndicators(closestHit.point, cantLandMaterial);
                }
                else
                {
                    // If no suitable landing point is found, hide the indicators
                    HideLandingIndicators();
                }
            }
        }
        else
        {
            // If not using the jetpack, hide the indicators
            HideLandingIndicators();
        }
    }

    private void UpdateLandingIndicators(Vector3 position, Material material)
    {
        // Update the position and material of landingPoint
        landingPoint.transform.position = Vector3.Lerp(landingPoint.transform.position, position, Time.deltaTime * smoothAmount);
        landingPointRenderer.material = material;

        // Update the rotation and material of the arrow
        arrow.transform.rotation = Quaternion.LookRotation(position - transform.position);
        arrowRenderer.material = material;
    }

    private void HideLandingIndicators()
    {
        // Hide both landingPoint and arrow indicators
        landingPoint.SetActive(false);
        arrow.SetActive(false);
    }

    private RaycastHit FindClosestGround()
    {
        // Find colliders within a sphere and determine the closest ground point
        Collider[] colliders = Physics.OverlapSphere(transform.position, 50f, groundLayer);
        float closestDistance = Mathf.Infinity;
        RaycastHit closestHit = new RaycastHit();

        foreach (Collider collider in colliders)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, collider.transform.position - transform.position, out hit, Mathf.Infinity, groundLayer))
            {
                float distance = Vector3.Distance(transform.position, hit.point);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestHit = hit;
                }
            }
        }

        return closestHit;
    }
}
