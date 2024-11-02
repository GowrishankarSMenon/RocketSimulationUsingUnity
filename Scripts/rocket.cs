using UnityEngine;

public class RocketLaunch : MonoBehaviour
{
    public float thrust = 10f;
    public GameObject target;
    public GameObject particlePrefab;

    [HideInInspector] public Rigidbody rb;
    private GameObject particleEffect;
    private float maxHeight;

    public bool violatedConditions = false;
    public float boundaryLimit = 150f; // Boundary limits for x and z
    public bool isTargetReached = false;
    public bool isLoopPassed = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        maxHeight = Mathf.Abs(target.transform.localPosition.y - transform.localPosition.y);
        particleEffect = Instantiate(particlePrefab, transform.localPosition, Quaternion.Euler(90, 0, 0));
        particleEffect.transform.SetParent(transform);
        particleEffect.GetComponent<ParticleSystem>().Stop();
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
            violatedConditions = true;
        }

        // Check if outside x and z boundaries
        if (Mathf.Abs(transform.localPosition.x) > boundaryLimit || Mathf.Abs(transform.localPosition.z) > boundaryLimit)
        {
            violatedConditions = true;
        }
    }

    public void ResetRocket()
    {
        transform.localPosition = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));
        transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

        // Reset velocity and angular velocity
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset conditions
        violatedConditions = false;
        isTargetReached = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("loop")) // Make sure your loop object is tagged "Loop"
        {
            isLoopPassed = true;
            Debug.Log("Rocket passed through the loop!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("loop"))
        {
            isLoopPassed = false;
            Debug.Log("Rocket exited the loop!");
            Destroy(other.gameObject); // Destroy the loop object
        }
    }

}
