using UnityEngine;

public class RocketLaunch : MonoBehaviour
{
    public float mainThrust = 10000f; // Thrust force for the main engine
    public float boosterThrust = 5000f; // Thrust force for each booster
    public Transform booster; // Assign the central position of the booster in the Inspector
    public float boosterWidth = 2.0f; // Distance from the center of the booster to each end
    public float dragCoefficient = 0.5f; // Drag coefficient for the rocket
    public float crossSectionalArea = 1.0f; // Cross-sectional area in square meters
    public float fuelBurnRate = 0.1f; // Rate at which fuel burns, affecting thrust over time
    public float initialMass = 1000f; // Mass of the rocket including fuel
    public float tippingTorqueFactor = 0.5f; // Torque applied when the rocket tips
    public float tippingThreshold = 10f; // Angle in degrees beyond which the rocket is considered tipped
    public ParticleSystem leftBoosterParticles; // Particles for the left booster
    public ParticleSystem rightBoosterParticles; // Particles for the right booster
    public ParticleSystem mainThrusterParticles; // Particles for the main booster

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public float currentFuel;
    [HideInInspector] public bool isLaunched = false;

    // Thrust multipliers controlled by the reinforcement learning model
    [HideInInspector] public float leftBoosterControl = 0f; // Control for left booster
    [HideInInspector] public float rightBoosterControl = 0f; // Control for right booster

    // Define lateral boundaries
    public float xBoundary = 100f; // Max x position
    public float zBoundary = 100f; // Max z position

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = initialMass;
        rb.angularVelocity = Vector3.zero;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        currentFuel = initialMass * 0.8f;

        // Set particle system positions and rotation for left and right boosters
        if (leftBoosterParticles != null)
        {
            leftBoosterParticles.transform.position = transform.position + new Vector3(-2.2842f, -0.5f, 0); // Set position for left booster
            leftBoosterParticles.transform.rotation = Quaternion.Euler(90, 0, 0); // Rotate to face downwards
        }

        if (rightBoosterParticles != null)
        {
            rightBoosterParticles.transform.position = transform.position + new Vector3(2.2842f, -0.5f, 0); // Set position for right booster
            rightBoosterParticles.transform.rotation = Quaternion.Euler(90, 0, 0); // Rotate to face downwards
        }

        // Set the main thruster particle system position
        if (mainThrusterParticles != null)
            mainThrusterParticles.transform.position = transform.position - transform.up * 0.5f; // Adjust as needed for distance
    }

    void Update()
    {
        // Start the launch when space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isLaunched = true;
            ApplyMainThrust(); // Apply thrust immediately when launched
            ApplySingleBoosterThrust(true); // Activate left booster
            ApplySingleBoosterThrust(false); // Activate right booster
        }

        // Temporary booster activation with key presses
        if (Input.GetKeyDown(KeyCode.A))
        {
            ApplySingleBoosterThrust(left: true);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ApplySingleBoosterThrust(left: false);
        }

        // Stop particle systems if fuel is depleted
        if (currentFuel <= 0)
        {
            if (mainThrusterParticles.isPlaying) mainThrusterParticles.Stop();
            if (leftBoosterParticles.isPlaying) leftBoosterParticles.Stop();
            if (rightBoosterParticles.isPlaying) rightBoosterParticles.Stop();
        }
    }

    public void ApplySingleBoosterThrust(bool left)
    {
        Vector3 thrustDirection = transform.up * boosterThrust;
        Vector3 thrustPosition = left
            ? transform.position + new Vector3(-2.2842f, -0.5f, 0) // Left booster position
            : transform.position + new Vector3(2.2842f, -0.5f, 0); // Right booster position

        rb.AddForceAtPosition(thrustDirection, thrustPosition);

        // Activate the corresponding particle system
        if (left && leftBoosterParticles != null && !leftBoosterParticles.isPlaying)
            leftBoosterParticles.Play();
        else if (!left && rightBoosterParticles != null && !rightBoosterParticles.isPlaying)
            rightBoosterParticles.Play();

        // Adjust fuel consumption
        currentFuel -= fuelBurnRate * Time.fixedDeltaTime;
        rb.mass = Mathf.Max(initialMass - (initialMass * 0.8f - currentFuel), initialMass * 0.2f);
    }

    void FixedUpdate()
    {
        if (isLaunched)
        {
            ApplyBoosterThrust();
            ApplyDrag();
            ApplyTippingForce();
            CheckBoundary(); // Check if the rocket has crossed the boundaries
        }
    }

    void CheckBoundary()
    {
        // Check the x position
        if (Mathf.Abs(transform.position.x) > xBoundary || Mathf.Abs(transform.position.z) > zBoundary)
        {
            Debug.Log("Rocket has crossed the boundaries! Fail.");
            // Trigger a failure response
            TriggerFailure();
        }
    }

    public void TriggerFailure()
    {
        // Implement failure response logic here
        isLaunched = false; // Stop the launch
        rb.velocity = Vector3.zero; // Stop the rocket's movement
        rb.angularVelocity = Vector3.zero; // Stop any rotation
        // Optionally reset position
        transform.position = new Vector3(0, 0, 0); // Reset position to origin or any starting point
    }

    public void ApplyMainThrust()
    {
        if (isLaunched)
        {
            // Apply main thrust at the rocket's central position
            float thrustForce = mainThrust * (currentFuel / (initialMass * 0.8f));
            rb.AddForceAtPosition(transform.up * thrustForce, transform.position);

            // Activate main thruster particles if available
            if (mainThrusterParticles != null && !mainThrusterParticles.isPlaying)
                mainThrusterParticles.Play();

            // Burn fuel
            currentFuel -= fuelBurnRate * Time.fixedDeltaTime;
            rb.mass = Mathf.Max(initialMass - (initialMass * 0.8f - currentFuel), initialMass * 0.2f);
        }
    }

    public void ApplyBoosterThrust()
    {
        // Set correct left and right thrust positions around the rocket's center
        Vector3 mainThrustPosition = transform.position;
        Vector3 leftThrustPosition = mainThrustPosition - transform.right * (boosterWidth / 2) - transform.up * 0.5f; // Bottom left
        Vector3 rightThrustPosition = mainThrustPosition + transform.right * (boosterWidth / 2) - transform.up * 0.5f; // Bottom right

        // Apply thrust forces for each booster based on control inputs
        Vector3 leftThrustForce = transform.up * (boosterThrust * leftBoosterControl);
        Vector3 rightThrustForce = transform.up * (boosterThrust * rightBoosterControl);

        // Apply force at left and right positions
        rb.AddForceAtPosition(leftThrustForce, leftThrustPosition);
        rb.AddForceAtPosition(rightThrustForce, rightThrustPosition);

        // Activate the particle systems for visual feedback
        if (leftBoosterControl > 0 && leftBoosterParticles != null && !leftBoosterParticles.isPlaying)
            leftBoosterParticles.Play();

        if (rightBoosterControl > 0 && rightBoosterParticles != null && !rightBoosterParticles.isPlaying)
            rightBoosterParticles.Play();

        // Adjust fuel consumption based on booster thrust usage
        currentFuel -= fuelBurnRate * (leftBoosterControl + rightBoosterControl) * Time.fixedDeltaTime;
        rb.mass = Mathf.Max(initialMass - (initialMass * 0.8f - currentFuel), initialMass * 0.2f);
    }

    void ApplyDrag()
    {
        // Calculate air density, decreases with altitude (simplified model)
        float altitude = transform.position.y;
        float airDensity = Mathf.Max(1.2f * Mathf.Exp(-altitude / 10000f), 0.1f); // Approximation for Earth's atmosphere

        // Calculate drag force
        float velocitySquared = rb.velocity.sqrMagnitude;
        Vector3 dragDirection = -rb.velocity.normalized;
        float dragForceMagnitude = 0.5f * dragCoefficient * airDensity * crossSectionalArea * velocitySquared;
        Vector3 dragForce = dragForceMagnitude * dragDirection;

        rb.AddForce(dragForce);
    }

    void ApplyTippingForce()
    {
        float angle = Vector3.Angle(transform.up, Vector3.up);
        if (angle > tippingThreshold)
        {
            // Apply a corrective torque
            Vector3 tippingDirection = Vector3.Cross(transform.up, Vector3.up).normalized;
            rb.AddTorque(tippingDirection * tippingTorqueFactor * angle);
        }
    }
}
