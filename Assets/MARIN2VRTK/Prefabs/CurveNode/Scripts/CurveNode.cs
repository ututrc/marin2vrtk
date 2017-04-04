using UnityEngine;
using System.Collections;

public class CurveNode : MonoBehaviour {

	public void SetColor(Color c)
    {
        GetComponent<MeshRenderer>().material.color = new Color(c.r, c.g, c.b, GetComponent<MeshRenderer>().material.color.a);
    }
}
