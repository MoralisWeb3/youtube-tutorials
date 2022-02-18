using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;

namespace SpriteShapeExtras
{

    // Please add this Component to Camera or some top level object on each loadable scene.
    public class GenerateSpriteShapes : MonoBehaviour
    {
    
        // Once all SpriteShapes are rendered, remove this Component if On or remove it from elsewhere.
        public bool destroyOnCompletion = true;
    
        void OnGUI()
        {
            
            // Loop all invisible SpriteShapeRenderers and generate geometry. 
            SpriteShapeRenderer[] spriteShapeRenderers = (SpriteShapeRenderer[]) GameObject.FindObjectsOfType (typeof(SpriteShapeRenderer));
            CommandBuffer rc = new CommandBuffer();
            rc.GetTemporaryRT(0, 256, 256, 0);
            rc.SetRenderTarget(0);        
            foreach (var spriteShapeRenderer in spriteShapeRenderers)
            {
                var spriteShapeController = spriteShapeRenderer.gameObject.GetComponent<SpriteShapeController>();
                if (spriteShapeRenderer != null && spriteShapeController != null)
                {
                    if (!spriteShapeRenderer.isVisible)
                    {
                        spriteShapeController.BakeMesh();
                        rc.DrawRenderer(spriteShapeRenderer, spriteShapeRenderer.sharedMaterial);
                        // Debug.Log("generating shape for " + spriteShapeRenderer.gameObject.name);
                    }
                }
            }
            rc.ReleaseTemporaryRT(0);
            Graphics.ExecuteCommandBuffer(rc);
            
            // SpriteShape Renderers are generated. This component is no longer needed. Delete this [or] remove this Component from elsewhere.
            if (destroyOnCompletion)
                Destroy(this);
            
        }
        
    }

}