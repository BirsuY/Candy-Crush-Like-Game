using System.Collections.Generic;
using UnityEngine;

public class CubePool : MonoBehaviour {
    public static CubePool Instance { get; private set; }

    // Dictionary mapping a prefab to a queue of inactive cubes
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    // Get a cube from the pool. If none are available, a new one is instantiated.
    public GameObject GetCube(GameObject prefab, Vector2 position, Quaternion rotation) {
        if (!poolDictionary.ContainsKey(prefab)) {
            poolDictionary[prefab] = new Queue<GameObject>();
        }
        Queue<GameObject> objectPool = poolDictionary[prefab];

        if (objectPool.Count > 0) {
            GameObject cube = objectPool.Dequeue();
            cube.transform.position = position;
            cube.transform.rotation = rotation;
            cube.SetActive(true);
            return cube;
        }

        // Instantiate a new cube and add the CubePoolItem component to track its prefab type.
        GameObject newCube = Instantiate(prefab, position, rotation);
        if (newCube.GetComponent<CubePoolItem>() == null) {
            CubePoolItem item = newCube.AddComponent<CubePoolItem>();
            item.prefabReference = prefab;
        }
        return newCube;
    }


    // Instead of destroying a cube, return it to the pool.
    public void ReturnCube(GameObject cube) {
        CubePoolItem poolItem = cube.GetComponent<CubePoolItem>();
        if (poolItem == null) {
            Debug.LogError("Cube does not have a CubePoolItem component attached.");
            return;
        }
        GameObject prefab = poolItem.prefabReference;
        if (!poolDictionary.ContainsKey(prefab)) {
            poolDictionary[prefab] = new Queue<GameObject>();
        }
        cube.SetActive(false);
        poolDictionary[prefab].Enqueue(cube);
    }
}
