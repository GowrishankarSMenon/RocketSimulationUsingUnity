using UnityEngine;

public class RocketLaunch : MonoBehaviour
{
    public float thrust = 10f;
    public GameObject target;
    public GameObject particlePrefab;
    public GameObject loopPrefab; // Assign your loop prefab here
    public int maxLoops = 5; // Maximum loops to spawn per episode
    public float rewardRange = 20f;
    [HideInInspector] public Rigidbody rb;
    private GameObject particleEffect;
    private float maxHeight;

    public bool violatedConditions = false;
    public float boundaryLimit = 150f; // Boundary limits for x and z
    public bool isTargetReached = false;
    public bool isLoopPassed = false;

    private int loopCount = 0; // Tracks the number of loops passed
    private GameObject currentLoop; // Reference to the current loop
    private float proximityReward = 0f;
    public Material planeMaterial;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        maxHeight = Mathf.Abs(target.transform.localPosition.y - transform.localPosition.y);
        particleEffect = Instantiate(particlePrefab, transform.localPosition, Quaternion.Euler(90, 0, 0));
        particleEffect.transform.SetParent(transform);
        particleEffect.GetComponent<ParticleSystem>().Stop();

        SpawnLoop(); // Spawn the first loop when the rocket starts
    }

    void Update()
    {
        ControlRocket();
        CheckUndesirableConditions(); // Check conditions in every frame
    }

    void ControlRocket()
    {
        if (transform.localPosition.y < maxHeight)
        {
            rb.AddForce(transform.up * thrust, ForceMode.Force);
            Vector3 directionToTarget = (target.transform.localPosition - transform.localPosition).normalized;
            float randomHorizontal = Random.Range(-0.5f, 0.5f); // Reduced random force for stability
            Vector3 randomDirection = new Vector3(directionToTarget.x + randomHorizontal, 0, 0).normalized;
            rb.AddForce(randomDirection * thrust * 0.25f, ForceMode.Force); // Reduced random thrust force

            if (!particleEffect.GetComponent<ParticleSystem>().isPlaying)
            {
                particleEffect.GetComponent<ParticleSystem>().Play();
            }
        }
        else if (transform.localPosition == target.transform.localPosition) // Corrected access to localPosition
        {
            isTargetReached = true;
        }
        else
        {
            if (particleEffect.GetComponent<ParticleSystem>().isPlaying)
            {
                particleEffect.GetComponent<ParticleSystem>().Stop();
            }
        }

        Quaternion targetRotation = Quaternion.LookRotation(target.transform.localPosition - transform.localPosition);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 50f);
    }

    void CheckUndesirableConditions()
    {
        // Check tilt angle on z-axis
        if (Mathf.Abs(transform.eulerAngles.z) > 90f)
        {
            planeMaterial.color = Color.red;
            violatedConditions = true;
        }

        // Check if outside x and z boundaries
        if (Mathf.Abs(transform.localPosition.x) > boundaryLimit || Mathf.Abs(transform.localPosition.z) > boundaryLimit)
        {
            planeMaterial.color = Color.red;
            violatedConditions = true;
        }
    }

    public void ResetRocket()
    {
        planeMaterial.color = Color.white;
        transform.localPosition = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));
        transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

        // Reset velocity and angular velocity
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset conditions
        violatedConditions = false;
        isTargetReached = false;
        isLoopPassed = false;

        loopCount = 0;
        SpawnLoop(); // Spawn the first loop for the new episode
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("loop")) // Make sure your loop object is tagged "loop"
        {
            isLoopPassed = true;
            loopCount++;
            Debug.Log("Rocket passed through the loop!");

            if (loopCount < maxLoops)
            {
                Destroy(other.gameObject); // Destroy the passed loop
                SpawnLoop(); // Spawn a new loop after passing
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("loop"))
        {
            planeMaterial.color = Color.green;
            isLoopPassed = false;
            Debug.Log("Rocket exited the loop!");
        }
    }

    private void SpawnLoop()
    {
        Vector3 randomPosition = GetRandomLoopPosition();
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

        // Destroy the previous loop if it exists to avoid multiple loops in the scene
        if (currentLoop != null)
        {
            Destroy(currentLoop);
        }

        currentLoop = Instantiate(loopPrefab, randomPosition, randomRotation);
    }

    private Vector3 GetRandomLoopPosition()
    {
        float randomDistance = Random.Range(5f, 15f); // Distance from the rocket
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.5f, 0.5f),
            Random.Range(0.5f, 1f)
        ).normalized;

        return transform.localPosition + randomDirection * randomDistance;
    }

    public float CalculateProximityReward()
    {
        if (currentLoop == null) return 0f;

        float distanceToLoop = Vector3.Distance(transform.localPosition, currentLoop.transform.localPosition);

        if (distanceToLoop <= rewardRange)
        {
            float rewardFactor = 1 - (distanceToLoop / rewardRange);
            proximityReward = rewardFactor * 10f; // Scale the reward factor as desired
            Debug.Log($"Proximity Reward: {proximityReward}");
            return proximityReward; // Return the calculated reward for the agent to use
        }

        return 0f; // No reward if outside range
    }

}
