using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    public event Action OnAttackAnimationEvent;

    public void InvokeAttackAnimationEvent()
    {
        OnAttackAnimationEvent.Invoke();
    }
}
