using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class RocketAgent : Agent
{
    public RocketLaunch rocketLaunch;

    public override void OnEpisodeBegin()
    {
        // Reset the rocket at the beginning of each episode
        rocketLaunch.ResetRocket();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Check if undesirable conditions have been violated
        if (rocketLaunch.violatedConditions)
        {
            SetReward(-10f); // Apply a negative reward with adjusted magnitude
            EndEpisode();    // End the episode to restart
            return;
        }

        // Apply actions to control the rocket if no violations
        var continuousActions = actions.ContinuousActions;
        float thrustControl = Mathf.Clamp(continuousActions[0], -1f, 1f);
        float pitchControl = Mathf.Clamp(continuousActions[1], -1f, 1f);

        // Adjust thrust multiplier for lift-off
        rocketLaunch.rb.AddForce(rocketLaunch.transform.up * thrustControl * rocketLaunch.thrust * 1.5f);
        rocketLaunch.rb.AddTorque(rocketLaunch.transform.right * pitchControl * rocketLaunch.thrust);

        // Provide a small positive reward if the rocket is moving upward
        if (rocketLaunch.rb.velocity.y > 0)
        {
            AddReward(0.1f); // Reward for upward movement, adjusted to a smaller value
        }

        // Reward and end episode if target is reached
        if (rocketLaunch.isTargetReached)
        {
            AddReward(10f); // Reward for reaching the target
            EndEpisode();
        }

        //Reward if passes thorugh loop
        if (rocketLaunch.isLoopPassed)
        {
            AddReward(7.5f);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Add normalized observations
        sensor.AddObservation(rocketLaunch.transform.position / 10f);       // Position (normalized)
        sensor.AddObservation(rocketLaunch.transform.rotation);             // Rotation
        sensor.AddObservation(rocketLaunch.rb.velocity / 10f);              // Velocity (normalized)
        sensor.AddObservation(rocketLaunch.rb.angularVelocity / 10f);       // Angular velocity (normalized)
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        if (continuousActionsOut.Length > 0)
        {
            continuousActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1f : 0f;  // Thrust control
            continuousActionsOut[1] = Input.GetAxis("Horizontal");            // Pitch control
        }
    }
}
