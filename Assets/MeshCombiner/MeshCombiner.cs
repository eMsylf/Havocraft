using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MeshCombiner : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshFilter MeshFilter
    {
        get
        {
            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();
            return meshFilter;
        }
    }
    MeshRenderer meshRenderer;
    MeshRenderer MeshRenderer
    {
        get
        {
            if (meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();
            return meshRenderer;
        }
    }

    public Material Material;
    public void CombineMesh()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        Debug.Log("Meshfilters found: " + meshFilters.Length);
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = "Combined Mesh";
        MeshFilter.sharedMesh = combinedMesh;
        MeshFilter.sharedMesh.CombineMeshes(combine, true);
        if (Material == null)
            MeshRenderer.sharedMaterial = meshFilters[1].GetComponent<MeshRenderer>().sharedMaterial;
        else
            MeshRenderer.sharedMaterial = Material;
        gameObject.SetActive(true);
    }
}
