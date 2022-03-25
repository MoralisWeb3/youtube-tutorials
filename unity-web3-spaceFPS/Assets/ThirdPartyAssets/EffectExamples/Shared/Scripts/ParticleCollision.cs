using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script demonstrate how to use the particle system collision callback.
/// The sample using it is the "Extinguish" prefab. It use a second, non displayed
/// particle system to lighten the load of collision detection.
/// </summary>
public class ParticleCollision : MonoBehaviour
{
    private List<ParticleCollisionEvent> m_CollisionEvents = new List<ParticleCollisionEvent>();
    private ParticleSystem m_ParticleSystem;


    private void Start()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }


    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = m_ParticleSystem.GetCollisionEvents(other, m_CollisionEvents);
        for (int i = 0; i < numCollisionEvents; ++i)
        {
            var col = m_CollisionEvents[i].colliderComponent;

            var fire = col.GetComponent<ExtinguishableFire>();
            if (fire != null)
                fire.Extinguish();
        }
    }
}
