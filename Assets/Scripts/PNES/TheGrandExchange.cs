using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheGrandExchange : MonoBehaviour
{
    public enum NODEID{ 
        ERRORNOTYPE,
        TASKLOG,
        TASKLOGCOMPLETESTATE,
        AIMODELS,
        PLAYERMODELS,
        AIANIMATORIDLE,
        AIANIMATORWALK,
        AIANIMATORPANIC,
        AIANIMATORDEATH,
        PLAYERANIMATORIDLE,
        PLAYERANIMATORWALK,
        PLAYERANIMATORGUN,
        PLAYERANIMATORRUN,
        PLAYERANIMATORSHOOT,
        PLAYERANIMATORDEATH,
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

    //IMPORTANT
    //Make this in same order as TASKIDS
    public static List<Vector3> taskWorldPositions = new List<Vector3>() {
        new Vector3(3.64f, 5.73f, -84.02f),
        new Vector3(64.4f,5.6f,-81.7f),
        new Vector3(-63.2f, 4.8f, -116.4f),
        new Vector3(-41.5f,7.6f,-62.9f),
        new Vector3(114.0f,5.07f,-136.0f),
        new Vector3(-3.73f,18.3f,-119.1f),
    };

    //Used for loading model and texture
    public enum MODELIDS
    {
        IGOR,
        SOLDIER,
        GRANNY,
        KAREN,
        SERGEI,
        IGOR2,
        SOLDIER2,
        GRANNY2,
        KAREN2,
        SERGEI2,
    };

}
