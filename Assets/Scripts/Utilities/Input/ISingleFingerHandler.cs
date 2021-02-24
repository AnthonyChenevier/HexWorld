/*
ISingleFingerHandler - handles strict single-finger down-up-drag

Put this daemon ON TO the game object, with a consumer of the service.

(Note - there are many, many philosophical decisions to make when
implementing touch concepts; just some issues include what happens
when other fingers touch, can you "swap out" etc. Note that, for
example, Apple vs. Android have slightly different takes on this.
If you wanted to implement slightly different "philosophy" you'd
do that in this script.)
*/


using UnityEngine.EventSystems;

public interface ISingleFingerHandler
{
    void OnSingleFingerDown(PointerEventData eventData);
    void OnSingleFingerUp(PointerEventData eventData);
    void OnSingleFingerDrag(PointerEventData eventData);
}