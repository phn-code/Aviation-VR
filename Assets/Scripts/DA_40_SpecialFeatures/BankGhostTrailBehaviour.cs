using UnityEngine;

public class BankGhostTrailBehaviour : MonoBehaviour
{
    [Header("enable")]
    public bool enable = true;          /**< Enable banking behaviour*/
    [Header("references")]
    public ParticleSystem ghostTrail;   /**< Ghost trail particle system */
    [Header("Ghost Trail Motion")]
    public float velocity = 50f;        /**< Velocity of the particles */
    private ParticleSystem.Particle[] ghostParticles; /**< List of particles emitted by the particle system */

    /**
    Main update loop

    Calls UpdateGhostTrailMotion() if enable flag set to true
    */
    void Update()
    {
        if (enable)
        {
            UpdateGhostTrailMotion();
        }
    }

    /** 
    Determines and updates behaviour of particles

    Creates a local copy of the assigned Particle System's particle buffer and modifies the tranform of particles.
    Particles follow the calculated radius and the logic is functional during level, ascending and descending flight.

    Formula for calculating turning radius
    \f[
    R = \frac{V^2}{g \times \tan(\theta)}
    \f]
    where:
    - \f$R\f$ = Radius (m)
    - \f$V\f$ = Velocity (m/s)
    - \f$g\f$ = Gravity (m/s²)
    - \f$\theta\f$ = Bank angle (degrees)

    Particles transforms are modified and buffer is returned to the Particle system.
    */
    void UpdateGhostTrailMotion()
    {
        if (ghostTrail == null) return;

        if (ghostParticles == null || ghostParticles.Length < ghostTrail.main.maxParticles)
        {
            ghostParticles = new ParticleSystem.Particle[ghostTrail.main.maxParticles];
        }

        int count = ghostTrail.GetParticles(ghostParticles);
        //Debug.Log("Particle count: " + count);
        if (count == 0) return;

        // Get bank angle
        float bankAngle = transform.rotation.eulerAngles.x;
        // convert from unsigned 0-360 to range -180 and 180
        float signedBank = Mathf.DeltaAngle(0f, bankAngle);

        // Check if in level flight
        if (Mathf.Abs(signedBank) < 1f)
        {
            // return, particle system handles level flight by default
            return;
        }

        /* Formula for calculating turning radius
         * R = V^2 / (g * tan(BankAngle)
         * R = Radius m
         * V = velocity m/s
         * g = gravity m/s
         * bankAngle = degrees
        */

        float g = 9.81f;
        // convert bank angle from degrees to radians
        float bankRadians = Mathf.Abs(signedBank) * Mathf.Deg2Rad;
        // Mathf.Tan expects radians not degrees
        float tanResult = Mathf.Tan(bankRadians);

        // Calculate radius
        float radius = (velocity * velocity) / (g * tanResult);
        // turn direction -1 = right 1 = left
        float turnDir = Mathf.Sign(signedBank);

        // The planes forward axis is +x therefore transform.forward returns the direction the planes +z axis is facing
        Vector3 sidewaysDir = transform.forward;
        // Isolate horizontal x and z directions
        sidewaysDir.y = 0;
        //Normalize horizontal dir
        sidewaysDir.Normalize();
        // change direction to left or right
        sidewaysDir *= turnDir;

        // calculate turn centre
        Vector3 turnCentre = transform.position + sidewaysDir * radius;

        float verticalComponent = -transform.right.y;

        // Update Each Ghost Particle
        for (int i = 0; i < count; i++)
        {
            // particle position
            Vector3 pos = ghostParticles[i].position;
            // get vector from current pos to centre
            Vector3 toCentre = turnCentre - pos;
            // remove y conponent
            toCentre.y = 0f;
            // normalize
            Vector3 dirToCentre = toCentre.normalized;

            // calculate the "tangent" using the cross product of the diirection to centre and vector3.down
            // returns the a vector perpendicular to both inputs
            Vector3 tangentDir = Vector3.Cross(Vector3.down, dirToCentre).normalized;
            // flip the tangent direction based on the turn side.
            tangentDir *= turnDir;

            // Add vertical component
            tangentDir.y = verticalComponent;

            // set the new velocity along the tangent
            ghostParticles[i].velocity = tangentDir * velocity;

            //Debug.Log("Particles velocity" + tangentDir*velocity);
        }
        // Pass the updated particles back to the ParticleSystem
        ghostTrail.SetParticles(ghostParticles, count);
    }

    /**
    Draw Gizmo Sphere representing turn radius

    Sphere represents the calculated radius of the plane and is rendered when GameObject is selected
    */
    void OnDrawGizmosSelected()
    {
        if (ghostTrail == null) return;

        // Get bank angle
        float bankAngle = transform.rotation.eulerAngles.x;

        if (bankAngle < 5f) return;

        // convert from unsigned 0-360 to range -180 and 180
        float signedBank = Mathf.DeltaAngle(0f, bankAngle);
        float g = 9.81f;
        // convert bank angle from degrees to radians
        float bankRadians = Mathf.Abs(signedBank) * Mathf.Deg2Rad;
        // Mathf.Tan expects radians not degrees
        float tanResult = Mathf.Tan(bankRadians);
        // Calculate radius
        float radius = (velocity * velocity) / (g * tanResult);
        // turn direction -1 = right 1 = left
        float turnDir = Mathf.Sign(signedBank);
        // The planes forward axis is +x therefore transform.forward returns the direction the planes +z axis is facing
        Vector3 sidewaysDir = transform.forward;
        // Isolate horizontal x and z directions
        sidewaysDir.y = 0;
        //Normalize horizontal dir
        sidewaysDir.Normalize();
        // change direction to left or right
        sidewaysDir *= turnDir;
        // calculate turn centre
        Vector3 turnCentre = transform.position + sidewaysDir * radius;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(turnCentre, radius);
    }
}
