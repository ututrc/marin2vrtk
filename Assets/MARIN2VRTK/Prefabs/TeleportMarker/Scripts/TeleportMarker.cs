using UnityEngine;
using System.Collections;

public class TeleportMarker : MonoBehaviour {

    public Renderer ring;
    public Renderer glow;

	public void SetColor(Color c)
    {
        ring.material.color = new Color(c.r, c.g, c.b, ring.material.color.a);
        glow.material.SetColor("_TintColor", new Color(c.r, c.g, c.b, glow.material.GetColor("_TintColor").a));
    }
}
