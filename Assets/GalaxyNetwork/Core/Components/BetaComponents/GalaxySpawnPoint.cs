using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GalaxySpawnPoint : MonoBehaviour
{
    [ExecuteInEditMode]
    void OnDrawGizmos() //Selected
    {
        Gizmos.DrawIcon(transform.position, "GalaxySpawn", true);   ///Resources/Gizmos/GalaxySpawn.png

        Color32 col = Color.yellow;
        col.a = 90;
        Gizmos.color = col;
        Gizmos.DrawSphere(transform.position, 0.15f);
    }


    void OnDrawGizmosSelected()
    {
        Color32 col = Color.yellow;
        //col.a = 90;
        Gizmos.color = col;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }

}
