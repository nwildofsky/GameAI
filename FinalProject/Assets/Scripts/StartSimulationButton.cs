using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartSimulationButton : MonoBehaviour
{
    Transform track;
    public GameObject requiredTile;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().interactable = false;

        // Make sure the track created transfers over to the simulation scene
        track = GameObject.Find("Racetrack").transform;
        DontDestroyOnLoad(track);
    }

    // Update is called once per frame
    void Update()
    {
        // Restrict custom tracks to only have 1 start and end
        if (GameObject.FindGameObjectsWithTag(requiredTile.tag).Length == 1)
        {
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Button>().interactable = false;
        }
    }

    public void StartSimulation()
    {
        SceneManager.LoadScene("Simulation");
    }
}
