using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;
public class UIBezierMove : MonoBehaviour
{

    public void BeginMove(
        Vector3 fromPosition,
        bool isUIPos,
        GameObject target,
        int easeType,
        float speed,
        Action onEnd
        )
    {
        BeginMoveByPos(fromPosition, isUIPos, target.transform.position, easeType, speed, onEnd);
    }

    public void BeginMoveByPos(
        Vector3 fromPosition,
        bool isUIPos,
        Vector3 targetPos,
        int easeType,
        float speed,
        Action onEnd
        )
    {
        this.mEnd = onEnd;
        var parent = this.transform.parent as RectTransform;

        Vector3 fromScreenPosition;
        if (isUIPos)
        {
            fromScreenPosition = RectTransformUtility.WorldToScreenPoint(null, fromPosition);
        } else {
            fromScreenPosition = Camera.main.WorldToScreenPoint(fromPosition);
        }
        // var cameraViewPostion = Camera.main.WorldToViewportPoint(fromPosition);
        mFromLocalPosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, fromScreenPosition, null, out mFromLocalPosition);
       // Debug.LogError("cameraViewPostion" + cameraViewPostion+  "cameraFromPostion" + cameraFromPostion +  "fromPosition" + fromPosition + "fromScreenPostion " + fromScreenPostion + " fromLocalPosition: "+ fromLocalPosition);
        var toScreenPostion = RectTransformUtility.WorldToScreenPoint(null, targetPos);
        var cameraToPosition = Camera.main.WorldToScreenPoint(targetPos);
        mToLocalPosition = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, toScreenPostion, null, out mToLocalPosition);
        //Debug.LogError("cameraToPosition"+ cameraToPosition+" toScreenPostion " + toScreenPostion + " toLocalPosition: " + toLocalPosition);
        List<Vector3> listBoPint = new List<Vector3>();
      //  Vector2 range = UnityEngine.Random.insideUnitCircle;
        int factor = -1;// UnityEngine.Random.Range(0, 1) > 0.5f?1:-1;
        var length = (mFromLocalPosition + mToLocalPosition).magnitude;
        listBoPint.Add(mFromLocalPosition);
       //  listBoPint.Add(fromLocalPosition+ new Vector2(factor* length * 0.05f, factor*  length * 0.05f));
        var temp = (mFromLocalPosition - mToLocalPosition).normalized;
        var normal = new Vector2(temp.y, -temp.x)* factor;
        listBoPint.Add(mFromLocalPosition+(mToLocalPosition- mFromLocalPosition) *0.25f + normal * length * 0.05f);
        listBoPint.Add((mFromLocalPosition + mToLocalPosition)*0.5f + normal * length * 0.075f);
        listBoPint.Add(mFromLocalPosition + (mToLocalPosition - mFromLocalPosition) * 0.75f + normal * length * 0.05f);
        // listBoPint.Add((fromPosition + ToPosition) / 3f + new Vector3(range.x * 50, range.y * 50, 0));
        // listBoPint.Add((fromPosition + ToPosition) * 2 / 3f + new Vector3(range.x * 50, range.y * 50, 0));
        listBoPint.Add(mToLocalPosition);
        Vector3[] path = GeneralPath(listBoPint);
        TweenParams parms = new TweenParams();
        parms.SetSpeedBased();
        parms.SetEase((Ease)easeType);
        parms.OnStepComplete(OnReachEnd);
        parms.OnUpdate(onMoveUpdate);
        transform.localPosition = mFromLocalPosition;
        mTween = transform.DOLocalPath(path, speed, DG.Tweening.PathType.CatmullRom, DG.Tweening.PathMode.Ignore)
                               .SetAs(parms);
        // .SetOptions(false, DG.Tweening.AxisConstraint.None, DG.Tweening.AxisConstraint.None)
        // .SetLookAt(0, Vector3.forward, Vector3.up);
         transform.localScale = Vector3.one;
         mTotalLength =(mFromLocalPosition - mToLocalPosition).magnitude;
         mTween.Play();
       
    }

     private void OnReachEnd()
    {
       // Debug.LogError("OnReachEnd...."+this.gameObject.name);
        if (mEnd != null) {
            mEnd();
            mEnd = null;
        }
        Clear();
    }
    
    private void onMoveUpdate()
    {
        if (mTween != null)
        {
            if (mTotalLength > 0 && shouldUpdateScale ) {
                float currentLength = ((Vector2)transform.localPosition - mToLocalPosition).magnitude;
                float radio = currentLength / mTotalLength;
                radio = Mathf.Max(0.65f, radio);
                transform.localScale = Vector3.one * radio;
            }
           // TweenerCore<Vector3, DG.Tweening.Plugins.Core.PathCore.Path, PathOptions> tweenPath = mTween as TweenerCore<Vector3, DG.Tweening.Plugins.Core.PathCore.Path, PathOptions>;
           // float totalPercent = tweenPath.ElapsedPercentage();
           // if (totalPercent >= 1) return;
           // totalPercent = UnityEngine.Mathf.Clamp((1-totalPercent), 0, 1);
            //Debug.LogError("instanceID："+this.transform.GetInstanceID()+" scale: "+ totalPercent);
           // transform.localScale = Vector3.one*totalPercent;
           // Debug.LogError("instanceID：" + this.transform.GetInstanceID() + " scale: " + transform.localScale);
        }
      
    }
    
    public void Clear()
    {
        ClearMove();
        mEnd = null;
    }
    private  Vector3[] GeneralPath(List<Vector3> listTranformList)
    {
        List<BezierPoint> bezierPoints = new List<BezierPoint>();
        for (int i = 0; i < listTranformList.Count; i++)
        {
            Vector3 tf = listTranformList[i];
            BezierPoint point = new BezierPoint();
            point.wp = tf;
            Vector3 left = point.wp + new Vector3(5, 0, 0);
            Vector3 right = point.wp + new Vector3(-5, 0, 0);
            point.cp = new[] { left, right };
            bezierPoints.Add(point);
        }
        return CalculatePath(bezierPoints);
    }
    private class BezierPoint {
        public Vector3 wp;
        public Vector3[] cp;
    }
    private  Vector3[] CalculatePath(List<BezierPoint> bPoints)
    {
        List<Vector3> temp = new List<Vector3>();
        for (int i = 0; i < bPoints.Count - 1; i++)
        {
            BezierPoint bp = bPoints[i];
            float detail = 1;
            temp.AddRange(GetPoints(bp.wp,
                            bp.cp[1],
                            bPoints[i + 1].cp[0],
                            bPoints[i + 1].wp,
                            detail));
        }
        return temp.Distinct().ToArray();
    }

    private static List<Vector3> GetPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float detail)
    {
        //temporary list for final points on this segment
        List<Vector3> segmentPoints = new List<Vector3>();
        //multiply detail value to have at least 5-10+ iterations
        float iterations = detail * 10f;
        for (int n = 0; n <= iterations; n++)
        {
            //cannot increment i as a float
            float i = (float)n / iterations;
            float rest = (1f - i);
            //bezier formula
            Vector3 newPos = Vector3.zero;
            newPos += p0 * rest * rest * rest;
            newPos += p1 * i * 3f * rest * rest;
            newPos += p2 * 3f * i * i * rest;
            newPos += p3 * i * i * i;
            //add calculated point to segment
            segmentPoints.Add(newPos);
        }
        //return points on this segment
        return segmentPoints;
    }

    private Tween mTween = null;
    private Action mEnd = null;
    private Vector2 mToLocalPosition;
    private Vector2 mFromLocalPosition;
    private float mTotalLength;
    public bool shouldUpdateScale = true;
    public void ClearMove()
    {
        if (mTween != null)
        {
            mTween.Kill();
            mTween = null;
        }
    }
}
