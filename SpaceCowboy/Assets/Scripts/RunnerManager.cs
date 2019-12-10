using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct ToSpawn
{
    public GameObject prefab;
    public float prob;
}

public class RunnerManager : MonoBehaviour
{
    public List<ToSpawn> tubePrefabs;
    public List<ToSpawn> trapPrefabs;
    public List<GameObject> tubeList;
    public Transform player;
    public Transform deathZone;

    public Material mat;

    public float disparitionFrequency = 2.5f;
    public float disparitionFastFrequency = 0.5f;
    public float disparitionTimer = 0;
    public float disparitionFastDist = 100;

    private float totalTrapProb = 0;
    private float totalTubeProb = 0;

    //private Collider 

    const float spawnDist = 1000;
    // Start is called before the first frame update
    void Start()
    {
        foreach(ToSpawn t in trapPrefabs){
            totalTrapProb += t.prob;
        }
        foreach (ToSpawn t in tubePrefabs)
        {
            totalTubeProb += t.prob;
        }
        SpawnTubes();
        tubeList[0].transform.Find("RunnerTube").GetComponent<MeshRenderer>().material = mat;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnTubes();
        disparitionTimer += Time.deltaTime;
        if (tubeList.Count != 0 && (disparitionTimer > disparitionFrequency || (Vector3.Distance(player.position, tubeList[0].transform.Find("EndAnchor").transform.position) > disparitionFastDist && disparitionTimer > disparitionFastFrequency))){
            disparitionTimer = 0;
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
            //assignation material
            tubeList[0].transform.Find("RunnerTube").GetComponent<MeshRenderer>().material = mat;
        }
        if(Vector3.Distance(player.position, tubeList[0].transform.Find("EndAnchor").transform.position) > disparitionFastDist){
            mat.SetFloat("_Fade", disparitionTimer/disparitionFastFrequency);
        }
        else{
            mat.SetFloat("_Fade", disparitionTimer/disparitionFrequency);
        }
    }

    //apparitions des tubes au deça d'une certaine distance
    void SpawnTubes(){
       while(Vector3.Distance(player.position, tubeList[tubeList.Count - 1].transform.Find("EndAnchor").transform.position) < spawnDist)
        {
            GameObject newTube = GameObject.Instantiate(tubePrefabs[PonderedRandomIndex(tubePrefabs, totalTubeProb)].prefab, tubeList[tubeList.Count - 1].transform.Find("EndAnchor").transform.position, tubeList[tubeList.Count - 1].transform.Find("EndAnchor").transform.rotation);
            newTube.transform.position -= (newTube.transform.Find("StartAnchor").transform.position - newTube.transform.position);

            for(int i = 0; i <  newTube.transform.Find("Obstacles").childCount;++i){
                int index = PonderedRandomIndex(trapPrefabs, totalTrapProb);
                if (trapPrefabs[index].prefab != null)
                {
                    GameObject newTrap = GameObject.Instantiate(trapPrefabs[index].prefab, newTube.transform.Find("Obstacles").GetChild(i));
                    newTrap.transform.localRotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0));
                }
            }

            tubeList.Add(newTube);
        }
    }

    int PonderedRandomIndex(List<ToSpawn> Prefabs, float totalProb){
        float sum = 0;
        float rand = UnityEngine.Random.Range(0, totalProb);
        int i = -1;
        while(sum<rand)
        {
            ++i;
            sum += Prefabs[i].prob;
        }

        return i;
    }
}
