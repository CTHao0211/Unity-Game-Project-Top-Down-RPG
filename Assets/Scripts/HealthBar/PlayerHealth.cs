using UnityEngine;

public class PlayerHealth : HealthBase
{
    protected override void Die()
    {
        Debug.Log("Player died!");

        // Disable movement
        PlayerControllerCombined.instance.enabled = false;

        // TODO: Trigger Game Over UI

        // Player thường không destroy object
        // Respawn logic hoặc game over screen
    }
}
