//using System.Collections;
//using Unity.MLAgents;
//using Unity.MLAgents.Actuators;
//using Unity.MLAgents.Sensors;
//using UnityEngine;

//public class RocketAgent : Agent
//{
//    private RocketLaunch rocketLaunch;
//    public float altitudeRewardScale = 0.01f;
//    public float tippingPenaltyScale = 0.02f;
//    public float maxTippingAngle = 45f; // Max angle before considering a failure

//    public override void Initialize()
//    {
//        rocketLaunch = GetComponentInChildren<RocketLaunch>();
//    }

//    public override void OnEpisodeBegin()
//    {
//        // Reset position, rotation, velocity, and fuel at the start of each episode
//        rocketLaunch.rb.velocity = Vector3.zero;
//        rocketLaunch.rb.angularVelocity = Vector3.zero;
//        transform.position = new Vector3(0, 0, 0);
//        transform.rotation = Quaternion.identity;
//        rocketLaunch.currentFuel = rocketLaunch.initialMass * 0.8f; // Reset fuel
//        rocketLaunch.isLaunched = false;
//    }

//    public override void CollectObservations(VectorSensor sensor)
//    {
//        sensor.AddObservation(transform.position.y); // Altitude
//        sensor.AddObservation(Vector3.Angle(transform.up, Vector3.up)); // Angle from vertical
//        sensor.AddObservation(rocketLaunch.rb.velocity); // Velocity
//        sensor.AddObservation(rocketLaunch.rb.angularVelocity); // Angular velocity
//        sensor.AddObservation(rocketLaunch.currentFuel); // Fuel level
//    }

//    public override void OnActionReceived(ActionBuffers actionBuffers)
//    {
//        // Generate random thrusts between 0 and 1 for each control
//        float mainThrustControl = Random.Range(0f, 1f);
//        float leftControl = Random.Range(0f, 1f);
//        float rightControl = Random.Range(0f, 1f);

//        // Update booster controls
//        rocketLaunch.leftBoosterControl = leftControl;
//        rocketLaunch.rightBoosterControl = rightControl;

//        // Apply thrusts using the RocketLaunch methods
//        rocketLaunch.ApplyMainThrust();
//        rocketLaunch.ApplyBoosterThrust();

//        // Reward for increasing altitude
//        AddReward(transform.position.y * altitudeRewardScale);

//        // Penalty for tipping
//        float angle = Vector3.Angle(transform.up, Vector3.up);
//        if (angle > maxTippingAngle)
//        {
//            SetReward(-1f); // Major penalty for excessive tipping
//            EndEpisode();
//        }
//        else
//        {
//            AddReward(-angle * tippingPenaltyScale); // Smaller penalty for small angles
//        }

//        // Penalty for fuel usage
//        if (rocketLaunch.currentFuel <= 0)
//        {
//            SetReward(-1f); // Major penalty if fuel is depleted
//            EndEpisode();
//        }
//    }
//}

/*
public class RocketAgent : Agent
{
    private RocketLaunch rocketLaunch;
    public float altitudeRewardScale = 1f;
    public float tippingPenaltyScale = 0.02f;
    public float maxTippingAngle = 45f; // Max angle before considering a failure
    [SerializeField] private Transform target;
    public override void Initialize()
    {
        rocketLaunch = GetComponentInChildren<RocketLaunch>();
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("Fuck off");
        // Reset position, rotation, velocity, and fuel at the start of each episode
        rocketLaunch.rb.velocity = Vector3.zero;
        rocketLaunch.rb.angularVelocity = Vector3.zero;
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;
        rocketLaunch.currentFuel = rocketLaunch.initialMass * 0.8f; // Reset fuel
        // Update booster controls
        rocketLaunch.boosterThrust = sideThrust;
        rocketLaunch.mainThrust = mainThrustControl;
        rocketLaunch.ApplySingleBoosterThrust(left: true);//for left booster
        rocketLaunch.ApplySingleBoosterThrust(left: false);//for right booster
        rocketLaunch.ApplyMainThrust();//for main booster
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.y); // Altitude
        //sensor.AddObservation(Vector3.Angle(transform.up, Vector3.up)); // Angle from vertical
        sensor.AddObservation((Vector2)rocketLaunch.rb.velocity); // Velocity
        sensor.AddObservation((Vector2)rocketLaunch.rb.angularVelocity); // Angular velocity
        sensor.AddObservation(rocketLaunch.currentFuel); // Fuel level
        sensor.AddObservation((Vector2)target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Generate random thrusts between 0 and 1 for each control
        float mainThrustControl = actionBuffers.ContinuousActions[0];
        //float rightThrustControl = actionBuffers.ContinuousActions[1];
        //float leftThrusterControl = actionBuffers.ContinuousActions[2];
        float sideThrust = actionBuffers.ContinuousActions[1];

        // Reward for increasing altitude
        AddReward(transform.position.y * altitudeRewardScale);

        // Penalty for tipping
        float angle = Vector3.Angle(transform.up, Vector3.up);
        if (angle > maxTippingAngle)
        {
            SetReward(-1f); // Major penalty for excessive tipping
            EndEpisode();
        }
        else
        {
            AddReward(-angle * tippingPenaltyScale); // Smaller penalty for small angles
        }

        // Penalty for fuel usage
        if (rocketLaunch.currentFuel <= 0)
        {
            SetReward(-1f); // Major penalty if fuel is depleted
            EndEpisode();
        }
    }
}
*/


using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RocketAgent : Agent
{
    private Rigidbody rb;

    // Public variables for thrust multipliers
    public float mainThrustMultiplier = 5f; // Main thruster thrust multiplier
    public float leftBoosterMultiplier = 2f; // Left booster thrust multiplier
    public float rightBoosterMultiplier = 2f; // Right booster thrust multiplier
    public float thrustInterval = 0.1f; // Time interval between thrust applications
    public float resetHeight = 10f; // Height at which the rocket will reset
    public float maxTiltAngle = 45f; // Maximum tilt angle before reset

    private void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        StartCoroutine(ApplyThrustContinuously()); // Start the thrust coroutine
    }

    public override void OnEpisodeBegin()
    {
        // Reset the rocket's position and state for a new episode
        ResetRocket();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Collect observations for the agent
        sensor.AddObservation(transform.position); // Rocket position
        sensor.AddObservation(rb.velocity); // Rocket velocity
        sensor.AddObservation(transform.rotation.eulerAngles); // Rocket rotation
    }

    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        // Get the action values
        float mainThrust = vectorAction.ContinuousActions[0]; // Main thruster value
        float leftBooster = vectorAction.ContinuousActions[1]; // Left booster value
        float rightBooster = vectorAction.ContinuousActions[2]; // Right booster value

        // Apply thrust
        rb.AddForce(transform.up * mainThrust * mainThrustMultiplier, ForceMode.Acceleration); // Main thrust upward
        rb.AddForce(-transform.right * leftBooster * leftBoosterMultiplier, ForceMode.Acceleration); // Left booster thrust
        rb.AddForce(transform.right * rightBooster * rightBoosterMultiplier, ForceMode.Acceleration); // Right booster thrust

        // Calculate tilt angle on the x and z axes
        float tiltAngleX = Quaternion.Angle(Quaternion.identity, Quaternion.Euler(transform.rotation.eulerAngles.x, 0, 0));
        float tiltAngleZ = Quaternion.Angle(Quaternion.identity, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z));

        // Check if the rocket has reached the target height or is tipped over
        if (transform.position.y >= resetHeight || tiltAngleX > maxTiltAngle || tiltAngleZ > maxTiltAngle)
        {
            // End the episode and reset the rocket
            EndEpisode();
            ResetRocket();
        }
    }
    private IEnumerator ApplyThrustContinuously()
    {
        while (true)
        {
            // Generate random thrust values for main thruster and boosters
            float randomMainThrust = Random.Range(0.5f, 1.5f); // Random value for main thrust
            float randomLeftBooster = Random.Range(0f, 1f); // Random value for left booster
            float randomRightBooster = Random.Range(0f, 1f); // Random value for right booster

            // Apply thrust
            rb.AddForce(transform.up * randomMainThrust * mainThrustMultiplier, ForceMode.Acceleration); // Main thrust upward
            rb.AddForce(-transform.right * randomLeftBooster * leftBoosterMultiplier, ForceMode.Acceleration); // Left booster thrust
            rb.AddForce(transform.right * randomRightBooster * rightBoosterMultiplier, ForceMode.Acceleration); // Right booster thrust

            // Wait for a specified interval before applying thrust again
            yield return new WaitForSeconds(thrustInterval);
        }
    }

    private void ResetRocket()
    {
        // Reset the rocket's position and rotation
        rb.velocity = Vector3.zero; // Reset velocity
        rb.angularVelocity = Vector3.zero; // Reset angular velocity
        transform.position = new Vector3(0, 0, 0); // Reset position
        transform.rotation = Quaternion.identity; // Reset rotation
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Implement manual control for testing
        var continuousActions = actionsOut.ContinuousActions;

        // Example of using arrow keys for movement
        continuousActions[0] = Input.GetAxis("Vertical"); // Main thrust
        continuousActions[1] = Input.GetAxis("Horizontal"); // Left booster
        continuousActions[2] = Input.GetAxis("Jump"); // Right booster (use space or another key)
    }
}


