using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;

using UnityEngine;

public class CharacterController : MonoBehaviour
{

    [FoldoutGroup("Reference"), Required]
    [SerializeField] ThrowController throwController;

    void Start()
    {
    }


    void Update()
    {

    }

    public void TakeDamage(int damage)
    {

    }

    void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            throwController.ChargeThrow();
        }
    }

    void OnMouseUp()
    {
        Debug.Log("Mouse up");
        throwController.Throw();
    }




}


public class ThrowObject
{
    public ThrowType Type;
    public Sprite Sprite;
    public float CollideRadius;
    public int Amount;
    public float Damage;

    public ThrowObject(ThrowType throwType)
    {
        switch (throwType)
        {

        }
    }
}

public enum ThrowType
{
    Normal,
    Power
}
