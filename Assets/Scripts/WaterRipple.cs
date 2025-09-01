using System;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class WaterRipple : MonoBehaviour
{
    [Header("Ripple Settings")]
    public int textureSize = 256;
    public float damping = 0.99f;
    public float rippleStrength = 0.5f;
    private Texture2D heightMap;
    private Color[] buffer1, buffer2;
    private bool useBuffer1 = true;
    private MeshRenderer meshRenderer;

    void Start()
    {
        // Create height map texture
        heightMap = new Texture2D(textureSize, textureSize, TextureFormat.RFloat, false);
        heightMap.wrapMode = TextureWrapMode.Clamp;

        // Initialize buffers
        buffer1 = new Color[textureSize * textureSize];
        buffer2 = new Color[textureSize * textureSize];
        ClearBuffers();

        // Assign texture to material
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.SetTexture("_HeightMap", heightMap);
    }

    void Update()
    {
        SimulateRipples();
    }

    // Invoke this method when something comes into contact with the water surface
    public void AddRipple(UnityEngine.Vector3 worldPos)
    {
        UnityEngine.Vector3 localPos = transform.InverseTransformPoint(worldPos);

        float u = (localPos.x + 0.5f) * textureSize;
        float v = (localPos.z + 0.5f) * textureSize;

        int x = Mathf.FloorToInt(u * textureSize);
        int y = Mathf.FloorToInt(v * textureSize);

        if (x < 1 || x >= textureSize - 1 || y < 1 || y >= textureSize - 1)
        {
            int idx = y * textureSize + x;

            if (useBuffer1)
                buffer1[idx].r += rippleStrength;
            else
                buffer2[idx].r += rippleStrength;
        }
    }

    private void SimulateRipples()
    {
        Color[] currentBuffer = useBuffer1 ? buffer1 : buffer2;
        Color[] nextBuffer = useBuffer1 ? buffer2 : buffer1;

        for (int y = 1; y < textureSize - 1; y++)
        {
            for (int x = 1; x < textureSize - 1; x++)
            {
                int idx = y * textureSize + x;

                // Average the heights of the neighbouring pixels
                float avg =
                    currentBuffer[idx - 1].r +
                    currentBuffer[idx + 1].r +
                    currentBuffer[idx - textureSize].r +
                    currentBuffer[idx + textureSize].r;
                avg *= 0.25f;

                // Ripple effect formula
                nextBuffer[idx].r = (avg - nextBuffer[idx].r) * damping;
            }
        }

        // Upload to texture
        heightMap.SetPixels(nextBuffer);
        heightMap.Apply();

        useBuffer1 = !useBuffer1;
    }

    private void ClearBuffers()
    {
        for (int i = 0; i < buffer1.Length; i++)
        {
            buffer1[i] = Color.black;
            buffer2[i] = Color.black;
        }
        heightMap.SetPixels(buffer1);
        heightMap.Apply();
    }
}