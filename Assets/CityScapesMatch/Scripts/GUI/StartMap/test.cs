using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class test : MonoBehaviour
{

    public RectTransform rt;
    public Canvas c;
    public bool track;
	// Use this for initialization
	void Start ()
    {
	}
	
	void Update()
    {
      //  if(rt && track &&c)
     //   Debug.Log("screen" + Coordinats.RectTransformToScreenSpace(rt));
      //  Debug.Log( "canvas:" + Coordinats.RectTransformToCanvasSpaceCenterCenter(rt, c));
    }
}
