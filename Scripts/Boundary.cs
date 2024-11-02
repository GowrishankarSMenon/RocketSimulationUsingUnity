using UnityEngine;

public class BoundaryDetection : MonoBehaviour
{
    public RocketLaunch launch;
    private void OnTriggerExit(Collider other)
    {
        // Check if the rocket has exited the boundary cube
        if (other.CompareTag("boundary")) // Ensure your boundary cube has the tag "Boundary"
        {
            // Call a method to handle the reset of the rocket
            launch.ResetRocket();
        }
    }
}
