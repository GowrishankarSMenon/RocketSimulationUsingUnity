# Rocket Launch Simulation Documentation

## Acknowledgments
This project uses Unity with ML-Agents for training a reinforcement learning agent to control a rocket launch simulation.

## Troubleshooting

### Training the Model
- Ensure you have the correct version of Unity and ML-Agents installed.
- If the model fails to train, check the console for errors regarding agent configuration or environment setup.

### Visual Cues
- The rocket will turn red if it violates boundary conditions or tilts more than 90 degrees on the z-axis.
- The rocket turns green when it passes through a loop.

### Agent Configuration
- The agent uses a reinforcement learning approach to learn the best actions based on the feedback it receives.
- Rewards are given for passing through loops and proximity to the loop.

## Project Structure
This section has been removed as per your request.

## RocketLaunch Script Overview

### Variables
- **thrust**: The force applied to the rocket during flight.
- **target**: The target object for the rocket to reach.
- **particlePrefab**: The particle effect used during the launch.
- **loopPrefab**: Prefab for the loops the rocket can pass through.
- **maxLoops**: Maximum number of loops to spawn during an episode.
- **rewardRange**: Range within which proximity rewards are calculated.
- **rb**: Reference to the Rigidbody component of the rocket.
- **maxHeight**: The maximum height the rocket can reach.
- **violatedConditions**: Flag to check if the rocket has violated any conditions.
- **boundaryLimit**: Limits for the x and z coordinates.
- **isTargetReached**: Flag to indicate if the target has been reached.
- **isLoopPassed**: Flag to indicate if the rocket has passed through a loop.
- **loopCount**: Counter for the number of loops passed.
- **currentLoop**: Reference to the currently active loop.
- **proximityReward**: Reward based on the proximity to the loop.
- **planeMaterial**: Material used to change the color of the rocket.

### Methods
- **Start()**: Initializes the rocket, sets up the Rigidbody, and spawns the first loop.
- **Update()**: Called every frame to control the rocket and check undesirable conditions.
- **ControlRocket()**: Applies thrust and controls the rocket’s movement towards the target.
- **CheckUndesirableConditions()**: Monitors the rocket's position and tilt to determine if any conditions are violated.
- **ResetRocket()**: Resets the rocket's position and conditions for a new episode.
- **OnTriggerEnter(Collider other)**: Checks for collisions with loops and updates the loop count and rewards.
- **OnTriggerExit(Collider other)**: Resets the loop passing state when the rocket exits a loop.
- **SpawnLoop()**: Instantiates a new loop at a random position.
- **GetRandomLoopPosition()**: Generates a random position for spawning a loop.
- **CalculateProximityReward()**: Calculates and returns a reward based on the distance to the current loop.

---

# Rocket Agent Documentation

## Overview
The `RocketAgent` class is responsible for controlling the rocket in the launch simulation. It utilizes Unity's ML-Agents to implement reinforcement learning, allowing the agent to learn optimal actions based on the environment's feedback.

## Acknowledgments
This agent is designed to work in conjunction with the `RocketLaunch` script, providing control over the rocket's movements and decision-making processes.

## Troubleshooting

### Training the Model
- Ensure that the `RocketLaunch` component is correctly assigned to the `rocketLaunch` variable in the inspector.
- Monitor the console for errors related to agent actions or environment setup.

### Visual Cues
- The agent receives negative rewards when undesirable conditions are violated (e.g., boundary violations).
- Positive rewards are given for reaching targets, passing through loops, and upward movement.

### Agent Configuration
- The agent uses continuous actions for thrust and pitch control.
- Observations include the rocket's position, rotation, velocity, and angular velocity, all normalized for better training performance.

## RocketAgent Script Overview

### Variables
- **rocketLaunch**: Reference to the `RocketLaunch` component, enabling the agent to interact with the rocket's behavior.

### Methods
- **OnEpisodeBegin()**: Resets the rocket at the beginning of each episode to ensure a fresh start.
- **OnActionReceived(ActionBuffers actions)**: Handles the actions received from the neural network.
  - Checks for violations and applies negative rewards if conditions are violated.
  - Applies thrust and torque based on the actions received.
  - Provides rewards for upward movement, reaching the target, passing through loops, and proximity to loops.
- **CollectObservations(VectorSensor sensor)**: Collects the necessary observations to feed into the neural network.
  - Normalizes the position, rotation, velocity, and angular velocity of the rocket.
- **Heuristic(in ActionBuffers actionsOut)**: Allows manual control of the rocket using keyboard input for testing purposes.
  - Uses the spacebar for thrust control and horizontal input for pitch control.

---

The `CameraSwitcher` script facilitates dynamic switching between the rocket and ground cameras, enhancing the user experience by allowing seamless transitions and managing audio listener states.

The `CameraFollow` script provides smooth and responsive camera movement that follows the rocket, ensuring the player has a consistent view of the action while maintaining a defined offset.

## Conclusion
The RocketLaunch script manages the rocket's physics, controls its flight dynamics, and handles the spawning of loops, ensuring an engaging and interactive simulation environment.
The `RocketAgent` script is a crucial component of the rocket launch simulation, enabling intelligent control through reinforcement learning. Ensure that the `rocketLaunch` reference is correctly set up for optimal performance.
The camera scripts are essential for a monitored environiment
