using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Generator : MonoBehaviour
{

    [SerializeField] public Transform Lobby;
    [SerializeField] private Camera cam;

    private List<Transform> sections = new List<Transform>();

    public Vector3 latestSectionEndPos;

    private const float GENERATION_DIST = 20f;

    public List<Transform> renderedSections = new List<Transform>();

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
            // Spawn new section
            SpawnLevelSection();

            // Delete old sections
            if (renderedSections.Count > 3)
            {
                Transform deleting = renderedSections[0];
                renderedSections.RemoveAt(0);
                Destroy(deleting.GameObject());
            }
        }
    }

    public void SpawnLevelSection()
    {
        // Section spawning

        Transform latestSectionTransform = SpawnLevelSection(latestSectionEndPos);
        renderedSections.Add(latestSectionTransform);

        latestSectionEndPos = latestSectionTransform.Find("SectionEnd").position;

        // Up game speed
        GameState.gameSpeed *= 1.025f;
    }
        

    private Transform SpawnLevelSection(Vector3 spawnPosition)
    {

        Transform spawning_section = sections[Random.Range(0, sections.Count)];

        Transform sectiontf = Instantiate(spawning_section, spawnPosition, Quaternion.identity);
        return sectiontf;
    }

}
