using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheGrandExchange : MonoBehaviour
{
    public enum NODEID{ 
        ERRORNOTYPE,
        TASKLOG,
        TASKLOGCOMPLETESTATE
    };

    public enum TASKIDS
    {
        EATFISH,
        GREETFISHERMAN,
        USEPAYPHONE,
        BUYNEWSPAPER,
        TIEUPBOAT,
        DROPTHEPACKAGE,
    };

    //Make this in same order as TASKIDS
    public static List<Vector3> taskWorldPositions = new List<Vector3>() {
        new Vector3(0.0f,0.6f,0.0f),
        new Vector3(3.0f,0.6f,0.0f),
        new Vector3(6.0f,0.6f,0.0f),
        new Vector3(9.0f,0.6f,0.0f),
        new Vector3(12.0f,0.6f,0.0f),
        new Vector3(15.0f,0.6f,0.0f),
        };

}
