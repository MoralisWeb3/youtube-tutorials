using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This simulate an extinguishable fire, 
/// </summary>
public class ExtinguishableFire : MonoBehaviour
{
    public ParticleSystem fireParticleSystem;
    public ParticleSystem smokeParticleSystem;

    protected bool m_isExtinguished;

    const float m_FireStartingTime = 2.0f;

    private void Start()
    {
        m_isExtinguished = true;

        smokeParticleSystem.Stop();
        fireParticleSystem.Stop();

        StartCoroutine(StartingFire());
    }

    public void Extinguish()
    {
        if (m_isExtinguished)
            return;

        m_isExtinguished = true;
        StartCoroutine(Extinguishing());
    }

    IEnumerator Extinguishing()
    {
        fireParticleSystem.Stop();
        smokeParticleSystem.time = 0;
        smokeParticleSystem.Play();

        float elapsedTime = 0.0f;
        while (elapsedTime < m_FireStartingTime)
        {
            float ratio = Mathf.Max(0.0f, 1.0f - (elapsedTime / m_FireStartingTime));

            fireParticleSystem.transform.localScale = Vector3.one * ratio;

            yield return null;

            elapsedTime += Time.deltaTime;
        }

        yield return new WaitForSeconds(2.0f);

        smokeParticleSystem.Stop();
        fireParticleSystem.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(4.0f);

        StartCoroutine(StartingFire());
    }

    IEnumerator StartingFire()
    {
        smokeParticleSystem.Stop();
        fireParticleSystem.time = 0;
        fireParticleSystem.Play();

        float elapsedTime = 0.0f;
        while (elapsedTime < m_FireStartingTime)
        {
            float ratio = Mathf.Min(1.0f, (elapsedTime / m_FireStartingTime));

            fireParticleSystem.transform.localScale = Vector3.one * ratio;

            yield return null;

            elapsedTime += Time.deltaTime;
        }

        fireParticleSystem.transform.localScale = Vector3.one;
        m_isExtinguished = false;
    }
}
