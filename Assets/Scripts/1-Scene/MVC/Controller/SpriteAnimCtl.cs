using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace MVC
{

    public class SpriteAnimCtl : MonoBehaviour
    {
        [SerializeField]
        private float frameRate; // 每帧间隔

        [SerializeField]
        private Sprite[] downSprites;

        [SerializeField]
        private Sprite[] leftSprites;

        [SerializeField]
        private Sprite[] rightSprites;

        [SerializeField]
        private Sprite[] upSprites;

        private SpriteRenderer sr;
        private float timer;
        private int frameIndex;
        private Sprite[] currentAnim;
        private PlayerModel playerModel;

        // 初始化
        private Direction currentDir = (Direction)(-1);
        private bool wasMoving;
        private bool isMoving;
        private bool isPaused = false;


        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            playerModel = GetComponent<PlayerCtl>().model;
            // 默认不移动
            wasMoving = false;
            isMoving = false;
            // 默认向下
            SetDirection(Direction.Down);
            currentDir = playerModel.Direction;

        }

        private void OnEnable()
        {
            EventBus.Subscribe<EPauseChanged>(OnPauseChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EPauseChanged>(OnPauseChanged);
        }

        private void OnPauseChanged(EPauseChanged e)
        {
            isPaused = e.IsPaused;
        }

        private void Update()
        {
            // 如果被暂停（来自事件）
            if (isPaused)
                return;
            if (!isMoving)
            {
                // Idle 时显示第零帧
                sr.sprite = currentAnim[0];
                wasMoving = false;
                return;
            }
            // 播放第一帧
            if (!wasMoving && isMoving)
            {
                // 保险，避免只有一帧，第一帧为空
                frameIndex = Mathf.Min(1, currentAnim.Length - 1);
                sr.sprite = currentAnim[frameIndex];
                wasMoving = true;
                return;
            }

            // 播放帧动画
            timer += Time.deltaTime;
            if (timer >= frameRate)
            {
                timer = 0f;
                frameIndex = (frameIndex + 1) % currentAnim.Length;
                sr.sprite = currentAnim[frameIndex];
            }
        }

        public void SetDirection(Direction dir)
        {
            if (currentDir == dir)
                return;
            currentDir = dir;
            frameIndex = 0;
            timer = 0f;

            switch (dir)
            {
                case Direction.Down:
                    currentAnim = downSprites;
                    break;
                case Direction.Left:
                    currentAnim = leftSprites;
                    break;
                case Direction.Right:
                    currentAnim = rightSprites;
                    break;
                case Direction.Up:
                    currentAnim = upSprites;
                    break;
            }
        }

        public void SetMoving(bool moving)
        {
            isMoving = moving;
        }
    }
}
