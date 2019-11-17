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
    public Animator animator;

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

        //Encode

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

            if (animator != null)
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
                    animator.SetBool("Walk", false);
                    animator.SetBool("Panic", false);
                    animator.SetBool("Draw", false);
                    //Walk
                    animator.SetBool("Walk", (decoder.DecodeBool(TheGrandExchange.NODEID.PLAYERANIMATORWALK, pc.PNESid)));
                    //Run
                    animator.SetBool("Panic", (decoder.DecodeBool(TheGrandExchange.NODEID.PLAYERANIMATORRUN, pc.PNESid)));
                    //Gun
                    animator.SetBool("Draw", (decoder.DecodeBool(TheGrandExchange.NODEID.PLAYERANIMATORGUN, pc.PNESid)));
                    //Shoot
                    if (decoder.DecodeBool(TheGrandExchange.NODEID.PLAYERANIMATORSHOOT, pc.PNESid))
                    {
                        animator.SetTrigger("Shoot");
                        CmdUpdateAnimation(TheGrandExchange.NODEID.PLAYERANIMATORSHOOT, pc.PNESid, 0); //reset shoot so we are not spamming it
                    }
                    //Death
                    if (decoder.DecodeBool(TheGrandExchange.NODEID.PLAYERANIMATORDEATH, pc.PNESid))
                    {
                        animator.SetTrigger("Death");
                    }
                }
                else if (animatorType == ANIMATORTYPE.AI)
                {
                    if (ai == null)
                    {
                        ai = GetComponent<AIController>();
                    }

                    //Idle
                    animator.SetBool("Walk", false);
                    animator.SetBool("Panic", false);
                    animator.SetBool("Draw", false);
                    //Walk
                    animator.SetBool("Walk", (decoder.DecodeBool(TheGrandExchange.NODEID.AIANIMATORWALK, ai.PNESid)));
                    //Panic
                    animator.SetBool("Panic", (decoder.DecodeBool(TheGrandExchange.NODEID.AIANIMATORPANIC, ai.PNESid)));
                    //Death
                    if (decoder.DecodeBool(TheGrandExchange.NODEID.AIANIMATORDEATH, ai.PNESid))
                    {
                        animator.SetTrigger("Death");
                    }
                }
            }
            else
            {
                Debug.LogWarning("Animator not set in PNES Animator Script");
            }
        }
    }
}
