using UnityEngine;

/// <summary>
/// Class to derive from if we wanted to include singleton functionality
/// </summary>
public class Singleton<T> : MonoBehaviour where T : Component
{
    #region Public Attributes



    #endregion

    #region Protected Attributes



    #endregion

    #region Private Attributes

    protected static T instance = null;
	
    #endregion
	
    #region Properties
	
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // try to find the object of this type
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    // it was not found so just create a new one
                    string name = typeof(T).Name;
                    GameObject obj = new GameObject(name);
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }
	
    #endregion
	
    #region MonoBehaviour Methods
	
    protected virtual void Awake()
    {
        // keep the instance through scenes
        if (instance == null)
        {
            instance = this as T;
            //DontDestroyOnLoad(gameObject);
        }
        // if instance was somehow initialized destroy the object
        else
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    private void Start () 
    {
		
    }
	
    // Update is called once per frame
    private void Update () 
    {
		
    }
	
    #endregion
	
    #region Methods

	

    #endregion
}
