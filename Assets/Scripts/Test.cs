using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayBGM("1-bgm");
    }
}
