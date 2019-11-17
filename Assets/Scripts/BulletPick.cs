using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BulletPick : NetworkBehaviour
{
    public float UIfintimer = 2.0f;
    public float UIcurrenttimer = 0.0f;
    private float maxdistance = 5.0f;

    GameObject dispenser;

    private Decoder decoder;
    private Interaction interact;
    private float nearInteractDistance = 12f;
    private Vector3[] interactionPoints;
    private GameObject[] interactionDot;

    private GameObject playerCanvas;
    public GameObject interactionDotPrefab;
    private GameObject interactionPrompt;
    private Text interactText;
    private Image interactProgressBar;

    private bool hasInitialized;
    private PlayerController player;

    void Start()
    {
        dispenser = GameObject.Find("DispenserLocations");
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Initialize();

        if (!hasInitialized)
        {
            return;
        }

        FindNearestInteractable();


        if (this.GetComponent<PlayerController>().gameStarted == true)
        {
            if (this.GetComponent<PlayerController>().amHitman == false)
            {
                Destroy(this);
            }
        }

        //for each despencer
        for (int i = 0; i < dispenser.transform.childCount; i++)
        {
            //if in range
            if (Vector3.Distance(this.gameObject.transform.position, dispenser.transform.GetChild(i).transform.position) < maxdistance)
            {
                //if its not on cool down. inittimer = 30, currenttimer starts at 30 and goes down to 0
                //loading up: ((blah.inittimer - blah.currenttimer) / blah.inittimer)
                // or 
                //loading down: (blah.currenttimer / blah.inittimer)
                if (dispenser.transform.GetChild(i).GetComponent<bulletDispenser>().currenttimer <= 0.0f)
                {

                    if (Input.GetKey("e"))
                    {
                        //hold e for UIcurrenttimer/UIfintimer (2 seconds)
                        UIcurrenttimer += Time.deltaTime;

                        if (UIcurrenttimer >= UIfintimer)
                        {
                            //bullet despenced
                            Add(i);
                        }
                    }
                    else
                    {
                        //reset holding timer
                        UIcurrenttimer = 0.0f; 
                    }
                }
            }
        }
    }

    private void Initialize()
    {

        if (playerCanvas == null)
        {
            player = GetComponent<PlayerController>();
            playerCanvas = GameObject.Find("PlayerCanvas(Clone)");
            decoder = GetComponent<Decoder>();
            interact = GetComponent<Interaction>();
            nearInteractDistance = interact.maxDistance * 3.0f;

            // Find cubes that are encoded with task log data

            interactionDot = new GameObject[dispenser.transform.childCount];

            for (int i = 0; i < dispenser.transform.childCount; i++)
            {
                // Find chosen tasks and set them in taskID[]

                // Create UI dot for each interaction point
                interactionDot[i] = Instantiate(interactionDotPrefab);
                interactionDot[i].transform.SetParent(playerCanvas.transform);
                interactionDot[i].transform.localPosition = new Vector3(0.0f, 64f - (i * 64f), 0);
                interactionDot[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }

            interactionPrompt = playerCanvas.transform.Find("InteractionPrompt").gameObject;
            interactText = interactionPrompt.transform.Find("Text").GetComponent<Text>();
            interactProgressBar = interactionPrompt.transform.Find("Text/HoldCircleProgress").GetComponent<Image>();

            hasInitialized = true;
        }
    }

    private void FindNearestInteractable()
    {
        int curDispenser = 0;
        float nearestDispenserDist = Mathf.Infinity;

        // Draw dot on nearby interaction points
        for (int i = 0; i < dispenser.transform.childCount; i++)
        {
            float dist = Vector3.Distance(transform.position, dispenser.transform.GetChild(i).position);
            if (dist < nearestDispenserDist)
            {
                nearestDispenserDist = dist;
                curDispenser = i;
            }


            bool isComplete = decoder.DecodeBool(TheGrandExchange.NODEID.TASKLOGCOMPLETESTATE, i);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(dispenser.transform.GetChild(i).position);
            interactionDot[i].transform.position = screenPos;

            if (dist < nearInteractDistance && !isComplete && interactionDot[i].transform.position.z > 0f)
            {
                interactionDot[i].SetActive(true);
            }
            else
            {
                interactionDot[i].SetActive(false);
            }

        }

        // Draw prompt on top of interaction point when close enough and task isn't complete
        float nearestDist = Vector3.Distance(transform.position, dispenser.transform.GetChild(curDispenser).position);
        Vector3 nearestScreenPos = Camera.main.WorldToScreenPoint(dispenser.transform.GetChild(curDispenser).position);
        interactionPrompt.transform.position = nearestScreenPos;

        // Set prompt text to appropriate string based on the associated task
        interactText.text = "Collect Bullet";

        if (nearestDist < interact.maxDistance && interactionPrompt.transform.position.z > 0f)
        {
            interactionPrompt.SetActive(true);

            // Disable dot when prompt appears
            interactionDot[curDispenser].SetActive(false);
        }
        else
        {
            interactionPrompt.SetActive(false);
        }

        // Disable if hitman or completing task
        bool canPickBullet = false;

        if (dispenser.transform.GetChild(curDispenser).GetComponent<bulletDispenser>().currenttimer <= 0.0f)
        {
            canPickBullet = true;
        }

        Debug.Log(interactProgressBar.fillAmount);
        Debug.Log("Is not Hitman" + !player.amHitman);
        Debug.Log("Cannot pick up bullet" + !canPickBullet);


        if (interactProgressBar.fillAmount == 1.0f || !player.amHitman || !canPickBullet)
        {
            interactionPrompt.SetActive(false);
            for (int i = 0; i < dispenser.transform.childCount; i++)
            {
                interactionDot[i].SetActive(false);
            }
        }


        // Update interaction progress ring
        interactProgressBar.fillAmount = UIcurrenttimer / UIfintimer;

    }

    void Add(int i)
    {
        //resets timer, adds a bulet, resets holding timer
        dispenser.transform.GetChild(i).GetComponent<bulletDispenser>().currenttimer = dispenser.transform.GetChild(i).GetComponent<bulletDispenser>().inittimer;
        this.GetComponent<PewPewGun>().Bullets += 1;
        UIcurrenttimer = 0.0f;
    }
}
