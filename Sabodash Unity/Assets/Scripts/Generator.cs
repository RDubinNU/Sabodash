using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor;
using UnityEngine;

public class Generator : MonoBehaviour
{

    [SerializeField] private Transform Lobby;
    [SerializeField] private Camera cam;

    private List<Transform> sections = new List<Transform>();

    private Vector3 latestSectionEndPos;

    private const float GENERATION_DIST = 20f;


    private void Awake()
    {
        string[] assetsPaths = AssetDatabase.GetAllAssetPaths();

        foreach (string assetPath in assetsPaths)
        {
            if (assetPath.Contains("Active Sections") && assetPath.Contains(".prefab"))
            {
                Transform loaded_section = (Transform)AssetDatabase.LoadAssetAtPath<Transform>(assetPath);
                sections.Add(loaded_section);
            }
        }

    }

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

        Transform spawning_section = sections[Random.Range(0, sections.Count)];

        Transform sectiontf = Instantiate(spawning_section, spawnPosition, Quaternion.identity);
        return sectiontf;
    }

}
