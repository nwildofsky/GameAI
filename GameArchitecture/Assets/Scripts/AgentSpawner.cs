using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider))]
public class AgentSpawner : MonoBehaviour
{
    public int agentNum = 20;
    public GameObject agentToSpawn;
    GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = GameObject.Find("Agents");
        BoxCollider box = GetComponent<BoxCollider>();

        float xOffset = box.center.x + box.size.x / 2f;
        float zOffset = box.center.z + box.size.z / 2f;
        float xMin = transform.position.x - xOffset;
        float xMax = transform.position.x + xOffset;
        float zMin = transform.position.z - zOffset;
        float zMax = transform.position.z + zOffset;

        for (int i = 0; i < agentNum; i++)
        {
            Instantiate(agentToSpawn, new Vector3(Random.Range(xMin, xMax), 1, Random.Range(zMin, zMax)), Quaternion.identity);
        }
    }
}
