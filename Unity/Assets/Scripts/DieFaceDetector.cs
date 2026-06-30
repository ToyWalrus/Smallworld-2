using System;
using UnityEngine;

public class DieFaceDetector : MonoBehaviour
{
    public Rigidbody die;
    public Action<int> onDieSettled;

    void OnTriggerStay(Collider other)
    {
        var obj = other.gameObject;
        var dieIsAtRest = die.angularVelocity == Vector3.zero && die.linearVelocity == Vector3.zero;

        if (obj.layer != LayerMask.NameToLayer("DieFace") || !dieIsAtRest)
        {
            return;
        }

        onDieSettled?.Invoke(int.Parse(obj.name));
    }
}
