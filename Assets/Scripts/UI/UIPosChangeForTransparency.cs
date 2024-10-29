using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPosChangeForTransparency : MonoBehaviour
{
    [SerializeField] private TransparentWindow transparentWindowScript;

    [SerializeField] private Vector3 normalPos;

    [SerializeField] private Vector3 transpPos;
    void Start()
    {
        transparentWindowScript.SetPosForTransparent += ChangePosToTransp;
        transparentWindowScript.ReturnPosFromTransparent += ReturnPos;
    }

    private void ChangePosToTransp()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = transpPos;
    }

    private void ReturnPos()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = normalPos;
    }
}
