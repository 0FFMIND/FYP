using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
namespace MVC
{
    public class Scene1ArrivalCtl : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;
        [SerializeField] private PlayerCtl player;
        void Start()
        {
            player.model.SetDisabled(true);
        }
    }
}

