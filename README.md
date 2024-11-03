# Rocket Simulation Using Unity

## Project Overview
This hackathon project simulates a rocket launch using reinforcement learning in Unity. The simulation applies advanced techniques to model rocket behavior and environmental interactions, creating a challenging task for the AI agent to control the rocket under realistic physics conditions. The primary goal is to simulate a stable ascent, navigate through loops a specified number of times (denoted as *n*), and avoid undesirable conditions.

## Features
- **Reinforcement Learning**: Utilizes Unity's ML-Agents to train an agent to control rocket dynamics effectively.
- **Dynamic Environment**: Conditions such as altitude, boundaries, and target loops guide the agent's training.
- **User Interface**: Provides visual feedback during simulation, including real-time metrics and conditions tracking.
- **Condition-Based Visual Cues**: The plane changes color based on rocket conditions—red for undesirable states and green for passing through a loop.

## Getting Started

### Prerequisites
- **Unity**: Version 2022.3.9f1
- **Python**: Version 3.9.13
- **ML-Agents**: Version 1.1.0
- **PyTorch**: (Optional but recommended) Version 1.9.1

### Project Files
- **Initial Commit**: [Initial commit](https://drive.google.com/drive/folders/15iTQurSdcJn8OeoL7bpSKCDWmAgPxFFS?usp=sharing)
- **Second Commit**: [Second commit](https://drive.google.com/drive/folders/16aFZkkReiwz5arkJLj06_aTd20dL3fkq?usp=sharing)
- **Third Commit**: [Third commit](https://drive.google.com/drive/folders/1MzVgCjg5PtfUeL6o56_2-BZyHeFJLXb1?usp=sharing)

### Installation & Setup
1. **Download**: Access the project files through the provided Google Drive link and download the complete directory.
2. **Setup Environment**:
    - Ensure Unity and Python are installed with the specified versions.
    - Install ML-Agents with `pip install mlagents==1.1.0`.
    - If needed, install PyTorch with `pip install torch==1.9.1` to enhance performance and model compatibility.
3. **Open Project**: Open Unity and load the downloaded project directory.
4. **Configure ML-Agents**:
    - Open the `Configs/config.yaml` file and adjust any desired parameters, including the number of loops (*n*) the rocket should pass through.
    - Run the simulation at 20x speed using the following command (replace `testid` with your choice):
      ```bash
      mlagents-learn Configs/config.yaml --run-id=testid --time-scale=20
      ```
5. **Run Simulation**: Start the simulation in Unity and monitor the agent's performance based on the feedback and visual cues in the Game view.

---

## Flow of Training and Usage
1. **Training the Model**:
   - After completing the setup, initiate the training process by running the ML-Agents command specified in the installation section.
   - Monitor the output in the terminal for real-time feedback on reward progression and completion of episodes. The agent will learn to navigate through the loops by receiving rewards for successfully passing through them and penalties for undesirable actions.

2. **Setting Loop Goal**:
   - The number of loops (*n*) that the rocket must pass through is set in the `Configs/config.yaml` file. Adjust this value as needed for your specific training objectives.

3. **Using the Trained Model**:
   - After training is complete, you can save the trained model.
   - To use the pre-trained model, assign it within Unity's ML-Agents configuration and disable the training feature to focus solely on evaluation.

4. **Visual Cues**:
   - During the simulation, the plane will turn **red** when the rocket enters undesirable conditions (e.g., crashing, exceeding bounds) and will turn **green** when it successfully passes through a loop.

---

## How to Run the Simulation
- **Training the Model**: After setting up, run the ML-Agents training command and monitor the output for real-time feedback on reward progression and completion of episodes.
- **Using Pre-trained Model**: If you have a trained model, assign it within Unity's ML-Agents configurations and disable training to focus on evaluation.
- **Visual Cues**: The simulation includes visual cues:
  - The plane turns **red** when the rocket violates undesirable conditions.
  - The plane turns **green** when the rocket successfully passes through a loop.

## Notes
- Follow the video guide in the Google Drive link for a step-by-step tutorial.
- This simulation is set to end episodes on critical violations or successful completion of loops.
