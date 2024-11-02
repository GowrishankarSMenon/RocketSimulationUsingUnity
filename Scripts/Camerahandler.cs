using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera rocketCamera;
    public Camera groundCamera;

    private bool isRocketCameraActive = true;

    void Start()
    {
        rocketCamera.enabled = true;
        groundCamera.enabled = false;

        // Ensure only one AudioListener is active
        rocketCamera.GetComponent<AudioListener>().enabled = true;
        groundCamera.GetComponent<AudioListener>().enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isRocketCameraActive = !isRocketCameraActive;

            // Switch camera and audio listeners
            rocketCamera.enabled = isRocketCameraActive;
            groundCamera.enabled = !isRocketCameraActive;

            rocketCamera.GetComponent<AudioListener>().enabled = isRocketCameraActive;
            groundCamera.GetComponent<AudioListener>().enabled = !isRocketCameraActive;
        }
    }
}
