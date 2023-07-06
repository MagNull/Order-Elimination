using UnityEngine;

public class AnimatedTrajectory : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private Transform _startTransform; 
    [SerializeField]
    private Transform _endTransform;

    [Header("Property names mapping")]
    [SerializeField]
    private string _offsetPropertyName;

    [Header("Settings")]
    [SerializeField]
    private float _tilingSpeed = 1;

    private int _offsetPropertyId;

    private void Reset()
    {
        _lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    private void Awake()
    {
        _offsetPropertyId = Shader.PropertyToID(_offsetPropertyName);
    }

    public void SetPoints(Vector2 start, Vector2 end)
    {
        _startTransform.position = new Vector3(start.x, start.y, 0);
        _endTransform.position = new Vector3(end.x, end.y, 0);
        //_lineRenderer.SetPositions(new[] { startV3, endV3 });
    }

    private void Update()
    {
        _lineRenderer.SetPositions(new[] { _startTransform.position, _endTransform.position });
        var currentPos = _lineRenderer.material.GetTextureOffset(_offsetPropertyId);
        var offset = Vector2.left * _tilingSpeed * Time.deltaTime;
        _lineRenderer.material.SetTextureOffset(_offsetPropertyId, currentPos + offset);//.SetTextureOffset()
    }
}
