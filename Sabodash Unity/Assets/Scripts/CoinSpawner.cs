using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject CoinPrefab;
    public int probability;
    void Start()
    {
        var random = Random.Range(0, probability);
        if(random == 0) Instantiate(CoinPrefab, transform.position, Quaternion.identity);
        Debug.Log(random);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
