using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.Playables;
using Utils;

namespace MVC
{
    public enum Scene1State
    {
        None,
        Timeline,
        GoToBoard,
        InteractBoard,
        OpenMenu,
        CloseMenu,
    }

    public class Scene1ArrivalCtl : MonoBehaviour
    {
        [Header("状态机")]
        [SerializeField]
        private Scene1State state = Scene1State.Timeline;

        [SerializeField]
        private PlayableDirector director;

        [SerializeField]
        private GuideDialogCtl guideCtl;

        [SerializeField]
        private GameObject guideSteps;

        [SerializeField]
        private Vector3 initPos;

        private PlayerCtl player;

        [SerializeField]
        private GameObject metalSign;

        private bool interactOnce = false;

        private void OnEnable()
        {
            EventBus.Subscribe<EScene1ArrivalStateChange>(EvaluateState);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EScene1ArrivalStateChange>(EvaluateState);
        }

        private void Start()
        {
            // 播放bgm
            AudioManager.Instance.PlayBGM("1-bgm-1", 1);
            // 禁止player移动
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtl>();
            player.model.SetDisabled(true);
            // 按状态启动
            EvaluateState();
        }

        private void EvaluateState(EScene1ArrivalStateChange e)
        {
            state = e.State;
            EvaluateState();
        }

        private void EvaluateState()
        {
            switch (state)
            {
                case Scene1State.None:
                    EnterNone();
                    break;
                case Scene1State.Timeline:
                    EnterTimeline();
                    break;
                case Scene1State.GoToBoard:
                    EnterGoToBoard();
                    break;
                case Scene1State.InteractBoard:
                    EnterInteractBoard();
                    break;
                case Scene1State.OpenMenu:
                    EnterOpenMenu();
                    break;
                default:
                    break;
            }
        }

        // —— 各状态入口逻辑 ——

        private void EnterNone()
        {
            // 测试用
            player.model.SetDisabled(false);
        }

        private void EnterTimeline()
        {
            // 设置人物位置
            var player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = initPos;
            // 启动timeline
            if (director != null)
            {
                director.time = 0;
                director.Play();
            }
        }

        private void EnterOpenMenu()
        {
            if (guideCtl != null)
            {
                // 禁止玩家移动
                player.model.SetDisabled(true);
                guideCtl.StartDialogue("1-Scene-5.txt", EndOpenMenu);
            }
        }

        private void EndOpenMenu() {
            player.model.SetDisabled(false);
        }

        private void EnterGoToBoard()
        {
            if (guideCtl != null)
            {
                guideCtl.StartDialogue("1-Scene-3.txt", EndGoToBoard);
            }
        }

        private void EndGoToBoard()
        {
            // 玩家可以移动
            player.model.SetDisabled(false);
            // 场景中出现引导脚印
            guideSteps.SetActive(true);
        }

        private void EnterInteractBoard()
        {
            StartCoroutine(InteractBoard());
        }

        private IEnumerator InteractBoard()
        {
            // 显示物体高亮
            var outline = metalSign.GetComponentInParent<SpritesOutline>();
            outline.SetOutlineVisible(true);
            // 禁止玩家移动
            player.model.SetDisabled(true);
            // 播放人物思考
            var emoteCtl = player.gameObject.GetComponent<PlayerEmoteCtl>();
            emoteCtl.Play(EmoteType.Thinking, 1f);
            yield return new WaitForSecondsRealtime(1.5f);
            // 播放guide
            guideCtl.StartDialogue("1-Scene-4.txt", EndSteps);
        }

        private void EndSteps()
        {
            // 关闭引导脚印
            guideSteps.SetActive(false);
            // 移动人物
            player.model.SetDisabled(false);
            // 重新显示交互图标
            player.gameObject.GetComponent<PlayerInteractCtl>().Refresh();
        }

        public void EndInteractBoard(InteractCtl ctl)
        {
            ctl?.Done();

            if (state == Scene1State.InteractBoard)
            {
                state = Scene1State.OpenMenu;
                EvaluateState();
            }
        }

        private void Update()
        {
            var current = GameObject
                .FindGameObjectWithTag("Player")
                .GetComponent<PlayerInteractCtl>()
                .target;
            var target = (current as Component)?.gameObject;
            if (
                target != null
                && target.gameObject.name == "metalSign"
                && !interactOnce
                && state == Scene1State.GoToBoard
            )
            {
                metalSign = target;
                state = Scene1State.InteractBoard;
                interactOnce = true;
                EvaluateState();
            }
        }
    }
}
