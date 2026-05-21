using UnityEngine;

public class StallVisualTrigger : MonoBehaviour
{
    [SerializeField] private DA_40 aircraft;
    [SerializeField] private ParticleSystem ghostTrail;
    
    // Instantly injects this much upward speed the moment a ghost spawns
    [SerializeField] private float initialUpwardSpeed = 40f; 
    
    // How fast it continues to curve up into the sky over time
    [SerializeField] private float upwardAcceleration = 20f; 

    private ParticleSystem.Particle[] particles;

    public void OnStallBegin()
    {
        aircraft.EnableGhostTrail(true);
    }

    public void OnStallEnd()
    {
        aircraft.EnableGhostTrail(false);
    }

    private void LateUpdate()
    {
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