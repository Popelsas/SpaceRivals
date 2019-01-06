/*
 * Позволяет автоматически удалить
 * объект со сцены если хозяин
 * этого объекта отключился
 */

using UnityEngine;
using System.Collections;
using GalaxyLib;
using System;


[RequireComponent(typeof(GalaxyNetID))]
//[ExecuteInEditMode()]
[HelpURL("https://docs.google.com/document/d/1u43Jxeq4LfcfxcCSLvWMvHALlEjTz8oxh9adJRqdVN4/edit#heading=h.nm9sidc57hm8")]
public class GalaxyDestroy : MonoBehaviour {

    [SerializeField]
    GalaxyNetID netId;

    private void OnEnable()
    {
        GalaxyEvents.OnGalaxyRoomExit += OnGalaxyRoomExit;
    }

    private void OnDisable()
    {
        GalaxyEvents.OnGalaxyRoomExit -= OnGalaxyRoomExit;
    }

    void Awake()
    {
        if (netId == null) netId = gameObject.GetComponent<GalaxyNetID>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "GalaxyDestroy", false);
    }

    void OnGalaxyRoomExit(UInt32 clientId, String nikname, ErrorCode errorCode)
    {
        if (errorCode != ErrorCode.none)
        {
            Debug.LogWarning(ErrorMessages.RU[errorCode]);
            return;
        }

        
        //if (netId.clientId == clientId)
        //{
        //    Destroy(gameObject);
        //}
    }


    // Use this for initialization
    void Start () {
    }



}
