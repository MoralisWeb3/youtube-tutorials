using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class U10PS_SnowOverTime : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    public float speed = 0.6f;
    
    private void Start(){
        meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

        totalTime = (1.0f / speed) * 4.71f; // 3.0f * 3.14f / 2.0f
    }

    private float totalTime = 0.0f;
    private void Update(){
        Material[] mats = meshRenderer.materials;

        mats[0].SetFloat("_SnowAmount", (Mathf.Sin(totalTime * speed) + 1.0f) / 2.0f);
        totalTime += Time.deltaTime;
        
        // Unity does not allow meshRenderer.materials[0]...
        meshRenderer.materials = mats;
    }
}
