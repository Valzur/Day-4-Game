using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    [SerializeField] Color onColor;
    [SerializeField] Color offColor;
    Renderer renderer;
    Material materialCopy;
    [HideInInspector] public bool isInteractable = true;
    bool _isOn = false;
    [HideInInspector] public bool IsOn
    {
        get{ return _isOn;}
        set{ _isOn = value; SwitchColor();}
    }

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        materialCopy = new Material(renderer.material);
        renderer.material = materialCopy;
        materialCopy.color = offColor;
    }

    void SwitchColor()
    {
        if(materialCopy!= null) 
        materialCopy.color = IsOn? onColor : offColor;
    }

    void OnMouseDown()
    {
        if(isInteractable)
            IsOn = !IsOn;
    }
}