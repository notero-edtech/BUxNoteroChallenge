using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ForieroEngine;
using ForieroEngine.Extensions;

[RequireComponent(typeof(Dropdown))]
public class TTUIDropdownArrow : MonoBehaviour, IPointerUpHandler
{
    Dropdown dropdown = null;

    Transform arrowTransform;

    bool _flipped = false;

    bool flip
    {
        get { return _flipped; }
        set
        {
            _flipped = value;
            if (arrowTransform) arrowTransform.rotation = Quaternion.Euler(0, 0, _flipped ? 180 : 0);
        }
    }

    private void Awake()
    {

        Bind();
    }

    private void OnDestroy()
    {
        UnBind();
    }

    void Bind()
    {
        dropdown = GetComponent<Dropdown>();
        arrowTransform = transform.Find("Arrow");
        dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    void UnBind()
    {
        dropdown.onValueChanged.RemoveListener(OnValueChanged);
        dropdown = null;
    }

    void Update()
    {
        if (!dropdownList && flip)
        {
            flip = false;
        }
    }

    private void OnValueChanged(int v)
    {
        flip = false;
        dropdownList = null;
    }

    Transform dropdownList = null;

    public void OnPointerUp(PointerEventData eventData)
    {
        if (dropdownList)
        {
            dropdownList = null;
            flip = false;
        }
        else
        {
            this.FireAction(1, () =>
            {
                dropdownList = transform.Find("Dropdown List");
                flip = true;
            });
        }
    }
}
