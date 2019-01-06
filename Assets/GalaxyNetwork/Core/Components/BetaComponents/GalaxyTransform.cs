using UnityEngine;
//using UnityEditor; //
using System.Collections;
using GalaxyLib;
using System;

[RequireComponent(typeof(GalaxyNetID))]
[ExecuteInEditMode()]
[HelpURL("https://docs.google.com/document/d/1u43Jxeq4LfcfxcCSLvWMvHALlEjTz8oxh9adJRqdVN4/edit#heading=h.xmgsmoiaxwyc")]
public class GalaxyTransform : MonoBehaviour {

    GalaxyNetID netId;

    public GalaxyTransformMode transformMode = GalaxyTransformMode.all;

    float t=0;

    //[Header("Частота отсыла (1/sendRate=раз в сек.)")]
    [Header("Частота отсыла (раз в сек.)")]
    public float sendRate = 10.0f;

	[Header("Порог срабатывания")]
	public float snapThreshold = 0.001f;

	// Для Порога срабатывания
	private Vector3 old_position = new Vector3();
	private Quaternion old_rotation = new Quaternion();

    private void OnEnable()
    {
//        GalaxyEvents.OnGalaxyTransformPosition += OnGalaxyTransformPosition;
//        GalaxyEvents.OnGalaxyTransformRotation += OnGalaxyTransformRotation;
        //GalaxyEvents.OnGalaxyRoomExit += OnGalaxyRoomExit;
    }

    private void OnDisable()
    {
//        GalaxyEvents.OnGalaxyTransformPosition -= OnGalaxyTransformPosition;
//        GalaxyEvents.OnGalaxyTransformRotation -= OnGalaxyTransformRotation;
        //GalaxyEvents.OnGalaxyRoomEnter -= OnGalaxyRoomExit;
    }

    void Awake()
    {
        if (netId == null) netId = gameObject.GetComponent<GalaxyNetID>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "GalaxyTransform.png", false);
    }

    // Сервер прислал новую позицию
    /*
    void OnGalaxyTransformPosition(GGPlayer player, Vector3 position)
    {
        if(!player.isMy)
        {
            if(netId.owner_id == player.clientId)
                gameObject.transform.position = position;
        }
    }
    */

    // Сервер прислал новый поворот
    /*
    void OnGalaxyTransformRotation(GGPlayer player, Quaternion rotation)
    {
        if (!player.isMy)
        {
            if (netId.owner_id == player.clientId)
                gameObject.transform.rotation = rotation;
        }
    }
    */

    // Use this for initialization
    void Start () {
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        if (netId.isMy)
        {
            t += Time.deltaTime;
            if (t > (1 / sendRate) || (sendRate == 0))
            {
				if (transformMode == GalaxyTransformMode.all || transformMode == GalaxyTransformMode.position) {
					if((Math.Abs(old_position.x) > snapThreshold) || (Math.Abs(old_position.y) >  snapThreshold) || (Math.Abs(old_position.z) >  snapThreshold))
					{
                        GalaxyNetwork.SendOperation.Transform.MyPosition (gameObject.transform.position);
						old_position = transform.position;
					}
				}
				if (transformMode == GalaxyTransformMode.all || transformMode == GalaxyTransformMode.rotation) {
					if ((Math.Abs (old_rotation.x) > snapThreshold) || (Math.Abs (old_rotation.y) > snapThreshold) || (Math.Abs (old_rotation.z) > snapThreshold) | (Math.Abs (old_rotation.w) > snapThreshold)) {
                        GalaxyNetwork.SendOperation.Transform.MyRotation (gameObject.transform.rotation);
						old_rotation = transform.rotation;
					}
				}
                t = 0;
            }
        }

    }

    public enum GalaxyTransformMode
    {
        none,
        position,
        rotation,
        all
    }


}
