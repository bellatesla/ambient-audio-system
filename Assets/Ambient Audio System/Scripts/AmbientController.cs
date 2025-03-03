using System.Collections;
using UnityEngine;

public class AmbientController : MonoBehaviour
{

    [Header("Ambient Audio Settings")]    
    [Range(0, 1)] public float masterVolume = 1;
    public float activationDistance = 20f; // Distance to activate areas
    
    [Tooltip("This could be the player or a camera")]
    public Transform player;
    private AmbientArea[] areas;

    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        areas = Object.FindObjectsByType<AmbientArea>(FindObjectsSortMode.None);
        StartCoroutine(UpdateAreas());
    }

    private IEnumerator UpdateAreas()
    {
        while (true)
        {
            for (int i = 0; i < areas.Length; i++)
            {
                float distance = Vector3.Distance(player.position, areas[i].transform.position);
                if (distance <= activationDistance)
                {
                    areas[i].SetActive(true);
                    areas[i].UpdateAmbientAreaAudio(player.position, masterVolume);
                }
                else
                {
                    areas[i].SetActive(false);
                }

                if (i % 10 == 0) // Update 10 areas per frame
                    yield return null;
            }
            yield return null;
        }
    }
}