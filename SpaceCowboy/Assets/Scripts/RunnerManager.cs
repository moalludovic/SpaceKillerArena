using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerManager : MonoBehaviour
{
    public List<GameObject> tubePrefabs;
    public List<GameObject> tubeList;
    public Transform player;
    public Transform deathZone;

    float disparitionFrequency = 2.5f;
    //private Collider 

    const float minDist = 1000;
    // Start is called before the first frame update
    void Start()
    {
        SpawnTubes();
        StartCoroutine("removeTube");
    }

    // Update is called once per frame
    void Update()
    {
        SpawnTubes();
    }

    void SpawnTubes(){
       if(Vector3.Distance(player.position, tubeList[tubeList.Count - 1].transform.Find("endAnchor").transform.position) < minDist)
        {
            GameObject newTube = GameObject.Instantiate(tubePrefabs[0], tubeList[tubeList.Count - 1].transform.Find("endAnchor").transform.position, tubeList[tubeList.Count - 1].transform.Find("endAnchor").transform.rotation);
            newTube.transform.position -= (newTube.transform.Find("startAnchor").transform.position - newTube.transform.position);
            tubeList.Add(newTube);

        }
    }

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
