using UnityEngine;

public class RocketController : MonoBehaviour
{
    private Rigidbody _rb;                // Reference to the rocket's Rigidbody component
    private GameObject _target;          // Reference to the "Player" GameObject to follow
    [Header("REFERENCES")]
    [SerializeField] private GameObject _explosionPrefab; // Explosion effect prefab

    [Header("MOVEMENT")]
    [SerializeField] private float _speed = 15;          // Rocket speed
    [SerializeField] private float _rotateSpeed = 95;    // Rocket rotation speed

    public float fuelCapacity;       // Maximum fuel capacity
    public float fuel;               // Current fuel level

    private void Awake()
    {
        fuel = fuelCapacity;                // Initialize fuel to the maximum capacity
        _target = GameObject.FindGameObjectWithTag("Player"); // Find the GameObject with the "Player" tag and set it as the target
        _rb = gameObject.GetComponent<Rigidbody>(); // Get the reference to the Rigidbody component
    }

    private void FixedUpdate()
    {
        _rb.velocity = transform.forward * _speed; // Move the rocket forward with the specified speed

        RotateRocket(); // Call the method for rocket rotation
    }

    private void Update()
    {
        if (!_target)
            return;

        fuel -= Time.deltaTime; // Decrease fuel over time

        if (fuel < 0)
        {
            explode(); // Call the method to explode the rocket when out of fuel
        }
    }

    private void RotateRocket()
    {
        if (!_target)
            return;

        var heading = _target.transform.position - transform.position; // Calculate the direction to the target

        var rotation = Quaternion.LookRotation(heading); // Determine the rotation to look at the target
        _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime)); // Rotate the rocket smoothly towards the target
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Player"))
        {
            explode(); // Call the method to explode the rocket upon collision with "Ground" or "Player" objects
        }
    }

    void explode()
    {
        if (_explosionPrefab) Instantiate(_explosionPrefab, transform.position, Quaternion.identity); // Instantiate the explosion effect if the explosion prefab is assigned

        Destroy(gameObject); // Destroy the rocket GameObject
    }
}
