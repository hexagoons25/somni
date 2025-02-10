using UnityEngine;

public class ConnectionRenderer : MonoBehaviour
{
    public Transform start;
    public Transform end;

    private LineRenderer line;
    private readonly int segmentCount = 10;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segmentCount;
        line.sortingLayerName = "Game";
        line.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        line.startWidth = 0.05F;
        line.endWidth = 0.05F;
    }

    void FixedUpdate()
    {
        Vector3 control = Vector3.Lerp(start.position, end.position, 0.5F);
        control.y -= 1;

        for (int i = 0; i < segmentCount; i++)
        {
            float t = Util.Remap(i, 0, segmentCount - 1, 0, 1);
            float oneMinusT = 1 - t;
            Vector3 bezierPoint = (oneMinusT * oneMinusT * start.position) + (2 * oneMinusT * t * control) + (t * t * end.position);
            bezierPoint.z = 1;
            line.SetPosition(i, bezierPoint);
        }
    }
}
