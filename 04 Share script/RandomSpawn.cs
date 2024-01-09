using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
 
    public GameObject SpawnArea;
    public GameObject CreateArea;

    public void MoveToCreateArea()
    {
        transform.SetPositionAndRotation(CreateArea.transform.position, CreateArea.transform.rotation);
    }

    public void MoveToSpawnArea()
    {
        transform.SetPositionAndRotation(SpawnArea.transform.position, SpawnArea.transform.rotation);
    }
}
