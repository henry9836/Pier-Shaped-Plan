using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UITab : MonoBehaviour
{
    public bool isActive;

    public Sprite activeSprite;
    public Sprite inactiveSprite;

    public Text tabText;
    private Button tabButton;

    void Start()
    {
        tabButton = GetComponent<Button>();
    }


    void Update()
    {

    }
}
