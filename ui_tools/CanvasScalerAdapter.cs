using UnityEngine;
using UnityEngine.UI;
public class CanvasScalerAdapter : MonoBehaviour {
  public void Awake() {
    var scaler = this.GetComponent<CanvasScaler>();
    if (scaler)
    {
        var resolution = scaler.referenceResolution;
        var refRadio = resolution.y / resolution.x;
        var deviceRadio = (float)Screen.height / Screen.width;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = deviceRadio >= refRadio ? 0 : 1;
    }
  }
}