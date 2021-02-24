/* note, Unity chooses to have "one interface for each action"
however here we are dealing with a consistent paradigm ("dragging")
which has three parts; I feel it's better to have one interface
forcing the consumer to have the three calls (no problem if empty) */


using UnityEngine;
using UnityEngine.EventSystems;

public class OneFingerInputModule : MonoBehaviour,
                IPointerDownHandler, IPointerUpHandler, IDragHandler

{
    private ISingleFingerHandler _myHandler = null;
    // of course that would be a List,
    // just one shown for simplicity in this example code

    private int currentSingleFinger = int.MinValue;
    private int kountFingersDown = 0;
    private bool _hasFinger;

    void Awake()
    {
        _myHandler = GetComponent<ISingleFingerHandler>();
        // of course, you may prefer this to search the whole scene,
        // just this gameobject shown here for simplicity
        // alternately it's a very good approach to have consumers register
        // for it. to do so just add a register function to the interface.
    }

    public void OnPointerDown(PointerEventData data)
    {
        kountFingersDown = kountFingersDown + 1;

        if (!_hasFinger && kountFingersDown == 1)
        {
            _hasFinger = true;
            currentSingleFinger = data.pointerId;
            if (_myHandler != null) _myHandler.OnSingleFingerDown(data);
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        kountFingersDown = kountFingersDown - 1;

        if (currentSingleFinger == data.pointerId)
        {
            currentSingleFinger = int.MinValue;
            _hasFinger = false;
            if (_myHandler != null) _myHandler.OnSingleFingerUp(data);
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (currentSingleFinger != data.pointerId || kountFingersDown != 1)
            return;

        if (_myHandler == null)
            return;

        _myHandler.OnSingleFingerDrag(data);
    }

}