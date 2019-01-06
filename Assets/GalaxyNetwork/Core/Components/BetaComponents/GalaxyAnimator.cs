using GalaxyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(GalaxyNetID))]
public class GalaxyAnimator : MonoBehaviour {

    [Header("Синхронизируемый аниматор")]
    public Animator animator;       // Синхронизируемый аниматор    //локальный аниматор
    //public Animator sendAnimator; // Воспроизводимый локально     //управляемый аниматор

    [Header("Слой аниматора")]
    public int layer =0;

    GalaxyNetID netId;

//    public int curState = -1;       // Текущее проигрываемое состояние
//    int old_curState = -1;
//    float dur = 0;

    void Awake()
    {
        if (animator == null) animator = gameObject.GetComponent<Animator>();
        if (animator == null) Debug.LogWarning("Стандартный компонент Animator не указан.");

        netId = gameObject.GetComponent<GalaxyNetID>();
        if (netId == null) Debug.LogError("Требуемый для Аниматора компонент <GalaxyNetID> не найден.");

 //2       netId.galaxyAnimator = this;

        //netId.sendInterval
        //readAnimator.layerCount
    }

    // Update is called once per frame
    /*
    void Update () {
        if(GalaxyClient.Connection.connected && readAnimator != null)
        {

        }

        //animator.GetLayerIndex

        if (readAnimator.isActiveAndEnabled && readAnimator.runtimeAnimatorController != null)
        {
            //animator.GetAnimatorTransitionInfo(layer).
            curState = readAnimator.GetCurrentAnimatorStateInfo(layer).shortNameHash;
            if (curState != old_curState)
            {
                old_curState = curState;
                dur = readAnimator.GetCurrentAnimatorStateInfo(layer).length;// normalizedTime
                //norm = animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;

                //Debug.Log("Send to server new state> " + curState);
                
                // Управляем аниматором сетевого клиента
                //clientAnimator.Play(curState, layer);
                sendAnimator.CrossFade(curState, dur, layer);
            }
            Debug.Log(readAnimator.GetCurrentAnimatorStateInfo(layer).normalizedTime);
        }
    }
    */
}
