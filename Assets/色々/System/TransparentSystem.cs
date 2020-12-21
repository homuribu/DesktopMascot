using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentSystem : MonoBehaviour
{
    Renderer rend;

    void Update()
    {
        foreach (Transform child in this.transform)
        {
            rend = child.GetComponent<Renderer>();

            if (rend != null)
            {
                for (int i = 0; i < rend.materials.Length; i++)
                {
                    Material material = rend.materials[i];
                    material.color = new Color(1, 1, 1, 0.5f);
                }
            }
        }
    }
}
