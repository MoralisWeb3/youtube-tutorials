using UnityEngine;

public class PlatformToggle : MonoBehaviour
{
    public bool activeOnDesktop;
    public bool activeOnAndroid;
    public bool activeOniOS;

    void Start()
    {
        MakeActive();
    }

    public void MakeActive()
    {
#if UNITY_ANDROID
        gameObject.SetActive(activeOnAndroid);
#elif UNITY_IOS
        gameObject.SetActive(activeOniOS);
#else
        gameObject.SetActive(activeOnDesktop);
#endif
    }
}
