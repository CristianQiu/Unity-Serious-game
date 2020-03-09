using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class that can keep track of interesting objects without the needs of making said objects singletons
/// </summary>
public static class ObjRegistry
{
    #region Public Attributes



    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    private static Dictionary<Type, MonoBehaviour> registeredObjs = new Dictionary<Type, MonoBehaviour>(32);

    #endregion

    #region Properties



    #endregion

    #region Methods

    /// <summary>
    /// Add an object of type T to the dictionary, it has to be a monobehaviour
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    public static void RegisterObj<T>(T obj) where T : MonoBehaviour
    {
        Type key = typeof(T);

        // if there is no instance of this type it add it
        if (!registeredObjs.ContainsKey(key))
            registeredObjs.Add(key, obj);
        else
            Debug.Log("There is already an instance of "+ key.ToString() +" and it won't be registered");

        return;
    }

    /// <summary>
    /// Remove an object of type T from the dictionary
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    public static void UnregisterObj<T>(T obj) where T : MonoBehaviour
    {
        Type key = typeof(T);

        bool removed = registeredObjs.Remove(key);

        // if it couln't be removed display a message
        if (!removed)
            Debug.Log("Can't unregister an instance of "+ key.ToString() +" because there is none in the dictionary");

        return;
    }

    /// <summary>
    /// Get an object from the dictionary
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetObj<T>() where T : MonoBehaviour
    {
        Type key = typeof(T);
        MonoBehaviour mono = null;

        // get the monobehaviour instance
        registeredObjs.TryGetValue(key, out mono);

        if (mono == null)
            Debug.Log("Can't get the type "+ key.ToString() +" because there is none in the dictionary");

        T obj = mono as T;
        return obj;
    }

    #endregion
}
