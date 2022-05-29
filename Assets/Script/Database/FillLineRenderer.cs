using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class FillLineRenderer : MonoBehaviour
{
    public enum FillDirection
    {
        Left,
        Right,
        Up,
        Down,
        Clockwise,
        CounterClockwise
    }

    private LineRenderer _lineRenderer;

    [SerializeField] [Range(0, 1)] private float _fillAmount = 1;

    public float FillAmount
    {
        get { return _fillAmount; }
        set
        {
            _fillAmount = value;
            Build();
        }
    }

    [SerializeField] private float _radius = 100;

    public float Radius
    {
        get { return _radius; }
        set
        {
            _radius = value;
            Build();
        }
    }

    [SerializeField] [Range(0, 360)] private float _startAngle = 360;

    public float StartAngle
    {
        get { return _startAngle; }
        set
        {
            _startAngle = value;
            Build();
        }
    }

    [SerializeField] private float _maxLength = 100;

    public float MaxLength
    {
        get { return _maxLength; }
        set
        {
            _maxLength = value;
            Build();
        }
    }

    [SerializeField] private int _points = 1;

    public int Points
    {
        get { return _points; }
        set { _points = value; }
    }

    public Color color
    {
        get { return _lineRenderer.startColor; }
        set
        {
            _lineRenderer.startColor = value;
            _lineRenderer.endColor = value;
        }
    }

    public float Width
    {
        get { return _lineRenderer.widthMultiplier; }
        set { _lineRenderer.widthMultiplier = value; }
    }

    [SerializeField] private FillDirection _direction;

    public FillDirection Direction
    {
        get { return _direction; }
        set
        {
            _direction = value;
            Build();
        }
    }

    //editor only, update from inspector
    void OnValidate()
    {
        Build();
    }

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = false;

        Build();
    }

    public void Build()
    {
        if (_points < 0)
            return;

        Vector3[] pts = new Vector3[_points + 1];

        float slice;
        for (var i = 0; i < _points + 1; i++)
        {
            if (Direction == FillDirection.Left)
            {
                slice = _fillAmount / _points;

                pts[i] = new Vector3
                {
                    x = slice * i * -_maxLength,
                    y = 0,
                    z = 0
                };
            }

            if (Direction == FillDirection.Right)
            {
                slice = _fillAmount / _points;

                pts[i] = new Vector3
                {
                    x = slice * i * _maxLength,
                    y = 0,
                    z = 0
                };
            }
            else if (Direction == FillDirection.Up)
            {
                slice = _fillAmount / _points;

                pts[i] = new Vector3
                {
                    x = 0,
                    y = slice * i * _maxLength,
                    z = 0
                };
            }
            else if (Direction == FillDirection.Down)
            {
                slice = _fillAmount / _points;

                pts[i] = new Vector3
                {
                    x = 0,
                    y = slice * i * -_maxLength,
                    z = 0
                };
            }
            else if (Direction == FillDirection.Clockwise)
            {
                slice = MaxLength * _fillAmount / _points;
                pts[i] = new Vector3
                {
                    x = Mathf.Sin((_startAngle + (slice * i)) / (180) * Mathf.PI) * _radius,
                    y = Mathf.Cos((_startAngle + (slice * i)) / (180) * Mathf.PI) * _radius,
                    z = 0
                };
            }
            else if (Direction == FillDirection.CounterClockwise)
            {
                slice = -1 * MaxLength * _fillAmount / _points;
                pts[i] = new Vector3
                {
                    x = Mathf.Sin((_startAngle + (slice * i)) / (180) * Mathf.PI) * _radius,
                    y = Mathf.Cos((_startAngle + (slice * i)) / (180) * Mathf.PI) * _radius,
                    z = 0
                };
            }
        }

        if (_lineRenderer != null)
        {
            _lineRenderer.positionCount = _points + 1;
            _lineRenderer.SetPositions(pts);
        }
    }
}

