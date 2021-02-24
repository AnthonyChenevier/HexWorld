/*
 * IPinchHandler - strict two sequential finger pinch Handling
 * 
 * Put this daemon ON TO the game object, with a consumer of the service.
 * (Note, as always, the "philosophy" of a glass gesture is up to you.
 * There are many, many subtle questions; eg should extra fingers block,
 * can you 'swap primary' etc etc etc - program it as you wish.)
 */

public interface ITwoFingerHandler
{
    void OnTwoFingerDown(TwoFingerEventData data);
    void OnTwoFingerUp(TwoFingerEventData data);
    void OnTwoFingerMove(TwoFingerEventData data);
}