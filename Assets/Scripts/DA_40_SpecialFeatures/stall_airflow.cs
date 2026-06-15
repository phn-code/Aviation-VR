using UnityEngine;

public class StallVisualTrigger : MonoBehaviour
{
    [SerializeField] private DA_40 aircraft;
    [SerializeField] private ParticleSystem ghostTrail;
    
    // Instantly injects this much upward speed the moment a ghost spawns
    [SerializeField] private float initialUpwardSpeed = 40f; 
    
    // How fast it continues to curve up into the sky over time
    [SerializeField] private float upwardAcceleration = 20f; 

    private bool isStalling;
    private ParticleSystem.Particle[] particles;

    public void OnStallBegin()
    {
        isStalling = true;
        // Do NOT auto-enable the ghost trail. By design it must only ever be turned on by the
        // user's controller toggle. We only flag the stall so LateUpdate can shape the trail's
        // particles IF the user already has it on (LateUpdate no-ops when the trail is off).
    }

    public void OnStallEnd()
    {
        isStalling = false;
        // Don't force the trail off either — its on/off state is entirely the user's to control.
    }

    private void LateUpdate()
    {
        if (!isStalling) return;
        if (ghostTrail == null || ghostTrail.particleCount == 0) return;

        if (particles == null || particles.Length < ghostTrail.main.maxParticles)
        {
            particles = new ParticleSystem.Particle[ghostTrail.main.maxParticles];
        }

        int count = ghostTrail.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            Vector3 vel = particles[i].velocity;

            // 1. INSTANT TOP-RIGHT ANGLE:
            // Since the horizontal speed is ~50, we force the Y speed to roughly match it.
            // This instantly creates a 45-degree angle to the top-right!
            if (vel.y < initialUpwardSpeed) 
            {
                vel.y = initialUpwardSpeed; 
            }
            
            // 2. Continually add curve so it swoops up beautifully
            vel.y += upwardAcceleration * Time.deltaTime; 
            
            particles[i].velocity = vel;
        }

        ghostTrail.SetParticles(particles, count);
    }
}
