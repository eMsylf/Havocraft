using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BigCubeThingCreator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/TEEEEEEEST", false, 10)]
    public static void CreateMulticubeObject()
    {
        GameObject addedObject = new GameObject("New Multicube");
        BigCubeThingCreator creator = addedObject.AddComponent<BigCubeThingCreator>();
        Selection.activeObject = addedObject;
        creator.FillAllBlocks();
    }
#endif


    public GameObject CubePrefab;
    public PrimitiveType PrimitiveType = PrimitiveType.Cube;
    public GameObject GetPrefab()
    {
        if (CubePrefab != null)
        {
            return CubePrefab;
        }
        GameObject primitivePrefab = GameObject.CreatePrimitive(PrimitiveType);
        primitivePrefab.name = "Multicube primitive prefab";
        return primitivePrefab;
    }
    [Range(1, 100)]
    public int CubesX = 3;
    [Range(1, 100)]
    public int CubesY = 3;
    [Range(1, 100)]
    public int CubesZ = 3;

    public bool CombineMeshOnStart = false;

    public enum EFillType
    {
        SolidCore,
        AllBlocks
    }

    public EFillType FillType;

    public List<GameObject> SpawnedCubes;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(CubesX, CubesY, CubesZ));
    }

    private void Start()
    {
        if (CombineMeshOnStart)
        {
            CombineMesh();
        }
    }

    public void NewMulticube()
    {
        Debug.Log("Create multicube");
        ClearList();

        switch (FillType)
        {
            case EFillType.AllBlocks:
                FillAllBlocks();
                break;
            case EFillType.SolidCore:
                FillWithSolidCore();
                break;
            default:
                FillWithSolidCore();
                break;
        }
    }

    public void ClearList()
    {
        if (SpawnedCubes != null)
        {
            CombineMeshUndo();
            foreach (GameObject spawnedCube in SpawnedCubes)
            {
                DestroyImmediate(spawnedCube, false);
            }
            SpawnedCubes.Clear();
        }
    }

    private void FillAllBlocks()
    {
        GameObject prefab = GetPrefab();
        for (int i = 0; i < CubesX; i++)
        {
            for (int j = 0; j < CubesY; j++)
            {
                for (int k = 0; k < CubesZ; k++)
                {
                    float xOffset = i - (CubesX - 1) / 2f;
                    float yOffset = j - (CubesY - 1) / 2f;
                    float zOffset = k - (CubesZ - 1) / 2f;

                    GameObject spawnedCube = Instantiate(prefab, transform.position + new Vector3(xOffset, yOffset, zOffset), Quaternion.identity, transform);
                    SpawnedCubes.Add(spawnedCube);
                }
            }
        }
    }

    private void FillWithSolidCore()
    {
        GameObject prefab = GetPrefab();
        GameObject Core = Instantiate(prefab, transform);
        Core.name = "Core Cube";
        Core.transform.localScale = new Vector3(CubesX - 2, CubesY - 2, CubesZ - 2);
        SpawnedCubes.Add(Core);

        for (int i = 0; i < CubesX; i++)
        {
            for (int j = 0; j < CubesY; j++)
            {
                for (int k = 0; k < CubesZ; k++)
                {
                    if (i == 0 || i == CubesX -1 || j == 0 || j == CubesY - 1 || k == 0 || k == CubesZ -1)
                    {
                        float xOffset = i - (CubesX - 1) / 2f;
                        float yOffset = j - (CubesY - 1) / 2f;
                        float zOffset = k - (CubesZ - 1) / 2f;
                        GameObject spawnedCube = Instantiate(prefab, transform.position + new Vector3(xOffset, yOffset, zOffset), Quaternion.identity, transform);
                        SpawnedCubes.Add(spawnedCube);
                    }
                }
            }
        }
    }

    public void CombineMesh()
    {
        MeshFilter MeshFilterComponent = GetComponent<MeshFilter>();

        Vector3 originalPosition = transform.position;

        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.one;

        transform.position = Vector3.zero;
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = "Combined Mesh";
        MeshFilterComponent.sharedMesh = combinedMesh;
        MeshFilterComponent.sharedMesh.CombineMeshes(combine);
        gameObject.SetActive(true);

        transform.position = originalPosition;
        transform.localScale = originalScale;

        GetComponent<MeshRenderer>().sharedMaterial = GetPrefab().GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void CombineMeshUndo()
    {
        MeshFilter MeshFilterComponent = GetComponent<MeshFilter>();

        MeshFilterComponent.sharedMesh = null;

        foreach (GameObject spawnedCube in SpawnedCubes)
        {
            if (spawnedCube != null)
            {
                spawnedCube.SetActive(true);
            }
        }
    }
}
