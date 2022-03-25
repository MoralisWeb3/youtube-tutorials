using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParticleExamples {

	public string title;
	[TextArea]
	public string description;
	public bool isWeaponEffect;
	public GameObject particleSystemGO;
	public Vector3 particlePosition, particleRotation;
}
