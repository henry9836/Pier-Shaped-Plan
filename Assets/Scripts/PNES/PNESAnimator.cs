using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PNESAnimator : NetworkBehaviour
{

    public enum ANIMATORTYPE{
        PLAYER,
        AI
    };

    public ANIMATORTYPE animatorType = ANIMATORTYPE.PLAYER;

    private Encoder encoder = null;
    private Decoder decoder = null;
    private PlayerController pc = null;
    private AIController ai = null;
    [SyncVar]
    private bool gameStarted = false;

    [Command]
    public void CmdCreateAnimator()
    {
        if (!isServer)
        {
            return;
        }

        gameStarted = true;

        if (encoder == null)
        {
            encoder = GameObject.FindGameObjectWithTag("GameController").GetComponent<Encoder>();
        }

        if (animatorType == ANIMATORTYPE.PLAYER)
        {
            if (pc == null)
            {
                pc = GetComponent<PlayerController>();
            }
            //Idle
            encoder.Encode(TheGrandExchange.NODEID.PLAYERANIMATORIDLE, pc.PNESid, 0);
            //Walk
            encoder.Encode(TheGrandExchange.NODEID.PLAYERANIMATORWALK, pc.PNESid, 0);
            //Run
            encoder.Encode(TheGrandExchange.NODEID.PLAYERANIMATORRUN, pc.PNESid, 0);
            //Gun
            encoder.Encode(TheGrandExchange.NODEID.PLAYERANIMATORGUN, pc.PNESid, 0);
            //Shoot
            encoder.Encode(TheGrandExchange.NODEID.PLAYERANIMATORSHOOT, pc.PNESid, 0);
            //Death
            encoder.Encode(TheGrandExchange.NODEID.PLAYERANIMATORDEATH, pc.PNESid, 0);
        }
        else if (animatorType == ANIMATORTYPE.AI)
        {
            if (ai == null)
            {
                ai = GetComponent<AIController>();
            }
            //Idle
            encoder.Encode(TheGrandExchange.NODEID.AIANIMATORIDLE, ai.PNESid, 0);
            //Walk
            encoder.Encode(TheGrandExchange.NODEID.AIANIMATORWALK, ai.PNESid, 0);
            //Panic
            encoder.Encode(TheGrandExchange.NODEID.AIANIMATORPANIC, ai.PNESid, 0);
            //Death
            encoder.Encode(TheGrandExchange.NODEID.AIANIMATORDEATH, ai.PNESid, 0);
        }
    }

    [Command]
    public void CmdUpdateAnimation(TheGrandExchange.NODEID node, int PNESid, int value)
    {
        if (!isServer)
        {
            return;
        }

        if (encoder == null)
        {
            encoder = GameObject.FindGameObjectWithTag("GameController").GetComponent<Encoder>();
        }

        encoder.Modify(node, PNESid, value);
    }

    private void FixedUpdate()
    {
        if (gameStarted)
        {

            if (decoder == null)
            {
                decoder = GetComponent<Decoder>();
            }

            //Run animation depending on animatorType

            if (animatorType == ANIMATORTYPE.PLAYER)
            {
                if (pc == null)
                {
                    pc = GetComponent<PlayerController>();
                }
                //Idle
                GetComponent<Animator>().SetBool("Walk", false);
                GetComponent<Animator>().SetBool("Panic", false);
                GetComponent<Animator>().SetBool("Draw", false);
                //Walk
                GetComponent<Animator>().SetBool("Walk", (decoder.DecodeBool(TheGrandExchange.NODEID.PLAYERANIMATORWALK, pc.PNESid)));
                //Run
                GetComponent<Animator>().SetBool("Panic", (decoder.DecodeBool(TheGrandExchange.NODEID.PLAYERANIMATORRUN, pc.PNESid)));
                //Gun
                GetComponent<Animator>().SetBool("Draw", (decoder.DecodeBool(TheGrandExchange.NODEID.PLAYERANIMATORGUN, pc.PNESid)));
                //Shoot
                if (decoder.DecodeBool(TheGrandExchange.NODEID.PLAYERANIMATORSHOOT, pc.PNESid))
                {
                    GetComponent<Animator>().SetTrigger("Shoot");
                }
                //Death
                if (decoder.DecodeBool(TheGrandExchange.NODEID.PLAYERANIMATORDEATH, pc.PNESid))
                {
                    GetComponent<Animator>().SetTrigger("Death");
                }
            }
            else if (animatorType == ANIMATORTYPE.AI)
            {
                if (ai == null)
                {
                    ai = GetComponent<AIController>();
                }

                Debug.Log("decoder: " + decoder.name);
                Debug.Log("Animator: " + GetComponent<Animator>().gameObject);
                Debug.Log("ai: " + ai);
                Debug.Log("PNESID: " + ai.PNESid);

                //Idle
                GetComponent<Animator>().SetBool("Walk", false);
                GetComponent<Animator>().SetBool("Panic", false);
                GetComponent<Animator>().SetBool("Gun", false);
                //Walk
                GetComponent<Animator>().SetBool("Walk", (decoder.DecodeBool(TheGrandExchange.NODEID.AIANIMATORWALK, ai.PNESid)));
                //Panic
                GetComponent<Animator>().SetBool("Panic", (decoder.DecodeBool(TheGrandExchange.NODEID.AIANIMATORPANIC, ai.PNESid)));
                //Death
                if (decoder.DecodeBool(TheGrandExchange.NODEID.AIANIMATORDEATH, ai.PNESid))
                {
                    GetComponent<Animator>().SetTrigger("Death");
                }
            }
        }
    }
}
