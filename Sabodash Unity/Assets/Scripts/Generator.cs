using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Generator : MonoBehaviour
{

    [SerializeField] private Transform section_1;
    [SerializeField] private Transform Lobby;
    [SerializeField] private Camera cam;

    private Vector3 latestSectionEndPos;

    private const float GENERATION_DIST = 20f;

    // Start is called before the first frame update
    private void Start()
    {
        latestSectionEndPos = Lobby.Find("SectionEnd").position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(cam.GetComponent<Transform>().position, latestSectionEndPos) < GENERATION_DIST)
        {
            SpawnLevelSection();
        }
    }

    private void SpawnLevelSection()
    {
        Transform latestSectionTransform = SpawnLevelSection(latestSectionEndPos);
        latestSectionEndPos = latestSectionTransform.Find("SectionEnd").position;
    }
        

    private Transform SpawnLevelSection(Vector3 spawnPosition)
    {
        Transform sectiontf = Instantiate(section_1, spawnPosition, Quaternion.identity);
        return sectiontf;
    }

}
