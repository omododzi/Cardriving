using UnityEngine;

public class MusicSwtch : MonoBehaviour
{
    public static bool music = true;
    public GameObject musicObjOn;
    public GameObject musicObjOff;

    void Start()
    {
        if (music)
        {
            musicObjOn.SetActive(true);
            musicObjOff.SetActive(false);
        }
        else
        {
            musicObjOn.SetActive(false);
            musicObjOff.SetActive(true);
        }
    }
    public void Switsh()
    {
        music = !music;
        if (music)
        {
            musicObjOn.SetActive(true);
            musicObjOff.SetActive(false);
        }
        else
        {
            musicObjOn.SetActive(false);
            musicObjOff.SetActive(true);
        }
    }
}
