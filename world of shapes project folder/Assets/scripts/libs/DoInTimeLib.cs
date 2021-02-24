using System;
using System.Collections;
using UnityEngine;

public static class DoInTimeLib
{

    public static Coroutine DoActionInTime(this MonoBehaviour monoBehaviour, Action action, float seconds)
    {
        return monoBehaviour.StartCoroutine(DoInSeconds(action, seconds));
    }

    public static Coroutine DoActionAndRepeat(this MonoBehaviour monoBehaviour, Action action, float seconds)
    {
        action();
        return monoBehaviour.StartCoroutine(DoInSeconds(action, seconds, seconds));
    }

    public static Coroutine DoActionInTimeRepeating(this MonoBehaviour monoBehaviour, Action action, float seconds)
    {
        return monoBehaviour.StartCoroutine(DoInSeconds(action, seconds, seconds));
    }

    public static Coroutine DoActionInTimeRepeating(this MonoBehaviour monoBehaviour, Action action, float secondsStart, float secondsRepeating)
    {
        return monoBehaviour.StartCoroutine(DoInSeconds(action, secondsStart, secondsRepeating));
    }

    public static Coroutine DoActionInNextFrame(this MonoBehaviour monoBehaviour, Action action)
    {
        return monoBehaviour.StartCoroutine(DoNextFrame(action));
    }

    private static IEnumerator DoInSeconds(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }
    private static IEnumerator DoInSeconds(Action action, float secondsStart, float secondsRepeat)
    {
        yield return DoInSeconds(action, secondsStart);
        while (true)
        {
            yield return new WaitForSeconds(secondsRepeat);
            action();
        }
    }
    private static IEnumerator DoNextFrame(Action action)
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        action();
    }




    //action with with a paramter

    public static Coroutine DoActionInTime<T>(this MonoBehaviour monoBehaviour, Action<T> action, T parameter, float seconds)
    {
        return monoBehaviour.StartCoroutine(DoInSeconds(action, parameter, seconds));
    }

    public static Coroutine DoActionAndRepeat<T>(this MonoBehaviour monoBehaviour, Action<T> action, T parameter, float seconds)
    {
        action(parameter);
        return monoBehaviour.StartCoroutine(DoInSeconds(action, parameter, seconds, seconds));
    }

    public static Coroutine DoActionInTimeRepeating<T>(this MonoBehaviour monoBehaviour, Action<T> action, T parameter, float seconds)
    {
        return monoBehaviour.StartCoroutine(DoInSeconds(action, parameter, seconds, seconds));
    }

    public static Coroutine DoActionInTimeRepeating<T>(this MonoBehaviour monoBehaviour, Action<T> action, T parameter, float secondsStart, float secondsRepeating)
    {
        return monoBehaviour.StartCoroutine(DoInSeconds(action, parameter, secondsStart, secondsRepeating));
    }

    public static Coroutine DoActionInNextFrame<T>(this MonoBehaviour monoBehaviour, Action<T> action, T parameter)
    {
        return monoBehaviour.StartCoroutine(DoNextFrame(action, parameter));
    }


    private static IEnumerator DoInSeconds<T>(Action<T> action, T parameter, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action(parameter);
    }
    private static IEnumerator DoInSeconds<T>(Action<T> action, T parameter, float secondsStart, float secondsRepeat)
    {
        yield return DoInSeconds(action, parameter, secondsStart);
        while (true)
        {
            yield return new WaitForSeconds(secondsRepeat);
            action(parameter);
        }
    }
    private static IEnumerator DoNextFrame<T>(Action<T> action, T parameter)
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        action(parameter);
    }
}
