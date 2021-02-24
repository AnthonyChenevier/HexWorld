using UnityEngine;
using UnityEngine.EventSystems;

/* note, Unity chooses to have "one interface for each action"
however here we are dealing with a consistent paradigm ("pinching")
which has three parts; I feel it's better to have one interface
forcing the consumer to have the three calls (no problem if empty) */

public class TwoFingerInputModule : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // of course that would be a List,
    // just one shown for simplicity in this example code

    private int _currentFirstFinger = int.MinValue;
    private int _currentSecondFinger = int.MinValue;
    private int _fingersDownCount = 0;

    private bool _hasTwoFingers = false;

    private Vector2 _positionFirst = Vector2.zero;
    private Vector2 _positionSecond = Vector2.zero;
    private float _previousDistance = 0f;
    private float _pointerDistanceDelta = 0f;

    private ITwoFingerHandler _myHandler;

    void Awake() { _myHandler = GetComponent<ITwoFingerHandler>(); }


    public void OnPointerDown(PointerEventData data)
    {
        _fingersDownCount = _fingersDownCount + 1;

        if (_currentFirstFinger == int.MinValue && _fingersDownCount == 1)
        {
            // first finger must be a pure first finger and that's that

            _currentFirstFinger = data.pointerId;
            _positionFirst = data.position;

            return;
        }

        if (_currentFirstFinger == int.MinValue || _currentSecondFinger != int.MinValue || _fingersDownCount != 2) return;
        // second finger must be a pure second finger and that's that

        _currentSecondFinger = data.pointerId;
        _positionSecond = data.position;

        FigureDelta();

        _hasTwoFingers = true;
        if (_myHandler != null)
            _myHandler.OnTwoFingerDown(new TwoFingerEventData(_positionFirst, _positionSecond, 0));
    }

    public void OnPointerUp(PointerEventData data)
    {
        _fingersDownCount = _fingersDownCount - 1;

        if (_currentFirstFinger == data.pointerId)
        {
            _currentFirstFinger = int.MinValue;
            _positionFirst = data.position;
            if (_hasTwoFingers)
            {
                _hasTwoFingers = false;
                if (_myHandler != null)
                    _myHandler.OnTwoFingerUp(new TwoFingerEventData(_positionFirst, _positionSecond, 0));
            }
        }

        if (_currentSecondFinger == data.pointerId)
        {
            _currentSecondFinger = int.MinValue;
            _positionSecond = data.position;
            if (_hasTwoFingers)
            {
                _hasTwoFingers = false;
                if (_myHandler != null)
                    _myHandler.OnTwoFingerUp(new TwoFingerEventData(_positionFirst, _positionSecond, 0));
            }
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (_currentFirstFinger == data.pointerId)
        {
            _positionFirst = data.position;
            FigureDelta();
        }

        if (_currentSecondFinger == data.pointerId)
        {
            _positionSecond = data.position;
            FigureDelta();
        }

        if (!_hasTwoFingers) return;

        if (data.pointerId != _currentFirstFinger && data.pointerId != _currentSecondFinger) return;

        if (_fingersDownCount != 2)
            return;

        if (_myHandler == null)
            return;

        _myHandler.OnTwoFingerMove(new TwoFingerEventData(_positionFirst, _positionSecond, _pointerDistanceDelta));
    }

    private void FigureDelta()
    {
        float newDistance = Vector2.Distance(_positionFirst, _positionSecond);
        _pointerDistanceDelta = newDistance - _previousDistance;
        _previousDistance = newDistance;
    }

}

public struct TwoFingerEventData
{
    public Vector2 PositionFirst;
    public Vector2 PositionSecond;
    public Vector2 PositionCentre;
    public float PointerDistanceDelta;

    public TwoFingerEventData(Vector2 positionFirst, Vector2 positionSecond,
                              float pointerDistanceDelta)
    {
        PositionFirst = positionFirst;
        PositionSecond = positionSecond;
        PositionCentre = positionFirst + (positionSecond - positionFirst) / 2f;
        PointerDistanceDelta = pointerDistanceDelta;
    }
}