using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class DummyInput : PlayerInput
{

    private void Update()
    {
        moveAmount = 0;
        moveDirection = new Vector3(0, 0, 0);
    }
}
