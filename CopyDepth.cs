using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyDepth : MonoBehaviour
{
    [Tooltip("Renders the Texture2D or the RenderTexture to the screen.")]
    [SerializeField] bool showTexture;
    [Tooltip("Use this toggle to test copying to the Texture2D. If disabled, only RenderTexture is used.")]
    [SerializeField] bool copyToTexture = false;
    [SerializeField] float depthMultiplier = 1f;
    [SerializeField] Texture2D texture;
    [SerializeField] RenderTexture renderTexture;
    Material depthCopyMat;
    int xDim;
    int yDim;

    void Start()
    {
        // Access camera
        Camera cam = GetComponent<Camera>();
        // Set depth mode to Depth
        cam.depthTextureMode = DepthTextureMode.Depth;
        // Cache camera dimensions
        xDim = cam.pixelWidth;
        yDim = cam.pixelHeight;
        // Acccess the depth blit shader
        Shader depthShader = Shader.Find("Hidden/CopyDepth");
        // Create material using the shader
        depthCopyMat = new Material(depthShader);
        // Init a new RenderTexture
        renderTexture = new RenderTexture(xDim, yDim, 24, RenderTextureFormat.ARGBFloat);
    }

    // Blit the depth buffer to the RenderTexture
    // From Unity docs: you must always issue a Graphics.Blit or render a fullscreen quad if you override this method
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // Set a visualization help parameter to the shader
        depthCopyMat.SetFloat("_DepthMultiplier", depthMultiplier);
        // Render with the shader from src to dest
        Graphics.Blit(src, renderTexture, depthCopyMat);
        // ReadPixels is very expensive!
        if (copyToTexture)
        {
            // Define read target texture
            texture = new Texture2D(xDim, yDim, TextureFormat.RGB24, false);
            // Define read rect, same dimensions as the camera
            Rect rectReadRT = new Rect(0, 0, xDim, yDim);
            // Set renderTexture as active so we can read
            RenderTexture.active = renderTexture;
            // Read from renderTexture to texture
            texture.ReadPixels(rectReadRT, 0, 0);
            // Store the read pixels
            texture.Apply();
            // Clear active
            RenderTexture.active = null;
        }
        // Render actual image
        Graphics.Blit(src, dest);
    }

    // Draw the texture on the screen to see it
    void OnGUI()
    {
        // Show Texture2D
        if (showTexture && copyToTexture)
        {
            // Draw the texture on the screen to test depth visibility
            Rect rect = new Rect (0,0,xDim,yDim);
            GUI.DrawTexture (rect, texture, ScaleMode.StretchToFill, false);
        }
        // Show RenderTexture
        else if (showTexture && !copyToTexture)
        {
            // Draw the renderTexture on the screen to test depth visibility
            Rect rect = new Rect (0,0,xDim,yDim);
            GUI.DrawTexture (rect, renderTexture, ScaleMode.StretchToFill, false);
        }
    }

}
