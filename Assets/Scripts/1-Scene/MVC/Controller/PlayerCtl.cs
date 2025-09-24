using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Utils;

namespace MVC
{
    public class PlayerCtl : MonoBehaviour
    {
        // 所有模块共享的唯一 PlayerModel
        public PlayerModel model = new PlayerModel();
    }
}
