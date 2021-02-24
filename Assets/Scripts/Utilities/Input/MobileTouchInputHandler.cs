using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TwoFingerInputModule))]
public class MobileTouchInputHandler : MonoBehaviour, ITwoFingerHandler, ISingleFingerHandler
{

    [Serializable]
    public class SingleFingerDownEvent : UnityEvent<Vector2> { }
    [Serializable]
    public class SingleFingerMoveEvent : UnityEvent<Vector2> { }
    [Serializable]
    public class SingleFingerUpEvent : UnityEvent<Vector2> { }
    [Serializable]
    public class TwoFingerDownEvent : UnityEvent<TwoFingerEventData> { }
    [Serializable]
    public class TwoFingerPinchEvent : UnityEvent<TwoFingerEventData> { }
    [Serializable]
    public class TwoFingerMoveEvent : UnityEvent<TwoFingerEventData> { }
    [Serializable]
    public class TwoFingerUpEvent : UnityEvent<TwoFingerEventData> { }

    public bool LogTouches;

    //required pixel distance in positions for pinch
    //higher dpi displays may make this scale
    public float PinchThreshold = 5;

    public SingleFingerDownEvent SingleFingerDown;
    public SingleFingerMoveEvent SingleFingerMove;
    public SingleFingerUpEvent SingleFingerUp;

    public TwoFingerDownEvent TwoFingerDown;
    public TwoFingerPinchEvent TwoFingerPinch;
    public TwoFingerMoveEvent TwoFingerMove;
    public TwoFingerUpEvent TwoFingerUp;



    public void OnSingleFingerDown(PointerEventData eventData)
    {
        Log(name + " OnBeginDrag");
        SingleFingerDown.Invoke(eventData.position);
    }

    public void OnSingleFingerUp(PointerEventData eventData)
    {
        Log(name + " OnEndDrag");
        SingleFingerUp.Invoke(eventData.position);
    }

    public void OnSingleFingerDrag(PointerEventData eventData)
    {
        Log(name + " OnDrag");
        SingleFingerMove.Invoke(eventData.position);
    }



    public void OnTwoFingerDown(TwoFingerEventData data)
    {
        Log(name + " OnTwoFingerDown");
        TwoFingerDown.Invoke(data);

    }

    public void OnTwoFingerUp(TwoFingerEventData data)
    {
        Log(name + " OnTwoFingerUp");
        TwoFingerUp.Invoke(data);
    }

    public void OnTwoFingerMove(TwoFingerEventData data)
    {
        if (data.PointerDistanceDelta > PinchThreshold)
        {
            Log(name + " OnTwoFingerDrag");
            TwoFingerMove.Invoke(data);
            return;
        }

        Log(name + " OnTwoFingerPinch");
        TwoFingerPinch.Invoke(data);
    }



    private void Log(string msg)
    {
        if (!LogTouches) return;
        Debug.Log("<color=#" + XKCDColors.DarkCoral.ToHex() + ">TOUCH HANDLER: </color>" + msg);
    }
}