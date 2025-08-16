using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace MVC
{
    public class PlayerCtl : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        //
        private Vector2 moveInput;
        private void Update()
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            // 保证斜向速度一致
            moveInput.Normalize();
            transform.position += (Vector3)moveInput * moveSpeed * Time.deltaTime;
        }
    }
}

