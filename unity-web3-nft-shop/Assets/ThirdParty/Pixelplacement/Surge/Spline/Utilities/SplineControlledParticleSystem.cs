/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Forces a referenced particle system to flow along this spline. To optimize performance limit Max Particles to just what is necessary.  Particle speed along the path is determined by Start Lifetime. 
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

[ExecuteInEditMode]
[RequireComponent (typeof (Spline))]
public class SplineControlledParticleSystem : MonoBehaviour
{
    //Public Variables:
    public float startRadius;
    public float endRadius;

    //Private Variables:
    [SerializeField] ParticleSystem _particleSystem = null;
    Spline _spline;
    ParticleSystem.Particle[] _particles;
    const float _previousDiff = .01f;

    //Init:
    void Awake ()
    {
        _spline = GetComponent<Spline> ();
    }

    //Loops:
    void LateUpdate ()
    {
        if (_particleSystem == null) return;

        if (_particles == null) _particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];

        int aliveParticlesCount = _particleSystem.GetParticles (_particles);

        for (int i = 0; i < aliveParticlesCount; i++)
        {
            //get calculation pieces:
            float seedMax = Mathf.Pow(10, _particles[i].randomSeed.ToString().Length);
            float seedAsPercent = _particles[i].randomSeed / seedMax;
            float travelPercentage = 1 - (_particles[i].remainingLifetime / _particles[i].startLifetime);

            //bypass issue while running at edit time when particles haven't reached the spline end:
            if (_spline.GetDirection(travelPercentage, false) == Vector3.zero) continue;

            //get a direction off our current point on the path - rotating by 1080 results in better distribution since Unity's randomSeed for particles favors lower numbers:
            Vector3 offshootDirection = Quaternion.AngleAxis(1080 * seedAsPercent, -_spline.GetDirection(travelPercentage, false)) * _spline.Up(travelPercentage);
            Vector3 previousOffshootDirection = Quaternion.AngleAxis(1080 * seedAsPercent, -_spline.GetDirection(travelPercentage - _previousDiff, false)) * _spline.Up(travelPercentage - _previousDiff, false);

            //cache our positions:
            Vector3 position = _spline.GetPosition(travelPercentage, false);

            //cache a previous position for velocity if possible:
            Vector3 lastPosition = position;
            if (travelPercentage - .01f >= 0) lastPosition = _spline.GetPosition(travelPercentage - _previousDiff, false);

            //decide how far to offshoot from the spline based on where we are between the start and end radius:
            float offset = Mathf.Lerp(startRadius, endRadius, travelPercentage);
            float previousOffset = Mathf.Lerp(startRadius, endRadius, travelPercentage - _previousDiff);

            //place particles depending on simulation space:
            Vector3 currentPosition = Vector3.zero;
            Vector3 previousPosition = Vector3.zero;

            switch (_particleSystem.main.simulationSpace)
            {
                case ParticleSystemSimulationSpace.Local:

                    currentPosition = _particleSystem.transform.InverseTransformPoint(position + offshootDirection * offset);
                    previousPosition = _particleSystem.transform.InverseTransformPoint(lastPosition + previousOffshootDirection * previousOffset);
                    break;

                case ParticleSystemSimulationSpace.World:
                case ParticleSystemSimulationSpace.Custom:
                    currentPosition = position + offshootDirection * offset;
                    previousPosition = position + previousOffshootDirection * previousOffset;
                    break;
            }

            //apply:
            _particles[i].position = currentPosition;
            _particles[i].velocity = currentPosition - previousPosition;
        }

        //apply the particle changes back to the system:
        _particleSystem.SetParticles (_particles, _particles.Length);
    }
}