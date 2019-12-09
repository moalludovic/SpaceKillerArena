using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Trap
{
    public GameObject prefab;
    public float probability;
}

public class RunnerManager : MonoBehaviour
{
    public List<GameObject> tubePrefabs;
    public List<Trap> trapPrefabs;
    public List<GameObject> tubeList;
    public Transform player;
    public Transform deathZone;

    public float disparitionFrequency = 2.5f;
    private float totalProbabilities = 0;

    //private Collider 

    const float minDist = 1000;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Trap t in trapPrefabs){
            totalProbabilities += t.probability;
        }
        SpawnTubes();
        StartCoroutine("removeTube");
    }

    // Update is called once per frame
    void Update()
    {
        SpawnTubes();
    }

    //apparitions des tubes au deça d'une certaine distance
    void SpawnTubes(){
       if(Vector3.Distance(player.position, tubeList[tubeList.Count - 1].transform.Find("EndAnchor").transform.position) < minDist)
        {
            GameObject newTube = GameObject.Instantiate(tubePrefabs[0], tubeList[tubeList.Count - 1].transform.Find("EndAnchor").transform.position, tubeList[tubeList.Count - 1].transform.Find("EndAnchor").transform.rotation);
            newTube.transform.position -= (newTube.transform.Find("StartAnchor").transform.position - newTube.transform.position);

            for(int i = 0; i <  newTube.transform.Find("Obstacles").childCount;++i){
                SpawnTrap(newTube.transform.Find("Obstacles").GetChild(i));
            }

            tubeList.Add(newTube);
        }
    }

    void SpawnTrap(Transform parent){
        float sum = 0;
        float rand = UnityEngine.Random.Range(0, totalProbabilities);
        int i = -1;
        while(sum<rand)
        {
            ++i;
            sum += trapPrefabs[i].probability;
        }

        if(trapPrefabs[i].prefab != null)
        {
            GameObject newTrap = GameObject.Instantiate(trapPrefabs[i].prefab, parent);
            newTrap.transform.localRotation = Quaternion.Euler( new Vector3(0, UnityEngine.Random.Range(0, 360), 0 ));
        }
        
    }

    //suppression des tubes à le fréquence indiquée
    IEnumerator removeTube(){
        yield return new WaitForSeconds(disparitionFrequency);
        GameObject toDestroy = tubeList[0];
        //death zone
        deathZone.transform.position = tubeList[0].transform.position;
        Vector3 s = toDestroy.transform.Find("RunnerTube").transform.localScale;
        Vector3 b = toDestroy.transform.Find("RunnerTube").GetComponent<MeshFilter>().mesh.bounds.extents * 2;
        deathZone.transform.localScale = new Vector3(s.x * b.x, s.y * b.y, s.z * b.z);
        deathZone.transform.rotation = tubeList[0].transform.Find("RunnerTube").transform.rotation;
        //suppression
        tubeList.RemoveAt(0);
        GameObject.Destroy(toDestroy);
        StartCoroutine("removeTube");
    }


}
