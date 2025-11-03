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
        MenuTutorial,

        // 等待玩家按 ESC 打开/关闭菜单的“等待态”
        AwaitMenuToggle,
        ExploreRooftop,
        ExploreCompleted,
        GoToMeadow,
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
        private TimelineDialogCtl timelineCtl;

        [SerializeField]
        private GameObject guideSteps;

        [SerializeField]
        private Vector3 initPos;

        private PlayerCtl player;

        private bool interactOnce = false;

        [SerializeField]
        private PlayerMoveSignal mover;

        private void OnEnable()
        {
            EventBus.Subscribe<EScene1ArrivalStateChange>(EvaluateState);
            EventBus.Subscribe<EJournalStatusChanged>(OnJournalChanged);
            EventBus.Subscribe<EPauseChanged>(OnPauseChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EScene1ArrivalStateChange>(EvaluateState);
            EventBus.Unsubscribe<EJournalStatusChanged>(OnJournalChanged);
            EventBus.Unsubscribe<EPauseChanged>(OnPauseChanged);
        }

        private void OnJournalChanged(EJournalStatusChanged e)
        {
            if (
                e.Key == "exploreRooftop"
                && e.NewStatus == JournalStatus.Completed
                && state == Scene1State.ExploreRooftop
            )
            {
                state = Scene1State.ExploreCompleted;
                EvaluateState();
                // 进入到下一状态
            }
            if (e.Key == "vendingMachine" && e.NewStatus == JournalStatus.Completed)
            {
                player.model.SetDisabled(true);
                // 唤醒dialog
                timelineCtl.StartFifthDialogue(() =>
                {
                    player.model.SetDisabled(false);
                    state = Scene1State.GoToMeadow;
                });
            }
        }

        private void EnterExploreCompleted()
        {
            player.model.SetDisabled(true);
            // 开始播放动画
            StartCoroutine(mover.PlayerFourthMove());
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
                case Scene1State.MenuTutorial:
                    EnterMenuTutorial();
                    break;
                case Scene1State.AwaitMenuToggle:
                    EnterAwaitMenuToggle();
                    break;
                case Scene1State.ExploreRooftop:
                    EnterExploreRooftop();
                    break;
                case Scene1State.ExploreCompleted:
                    EnterExploreCompleted();
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

        private void EnterExploreRooftop()
        {
            player.model.SetDisabled(true);
            timelineCtl.StartFourthDialogue(() =>
            {
                player.model.SetDisabled(false);
            });
        }

        // 全局暂停状态变化事件回调
        private void OnPauseChanged(EPauseChanged e)
        {
            if (state != Scene1State.AwaitMenuToggle)
            {
                return;
            }
            if (!e.IsPaused)
            {
                state = Scene1State.ExploreRooftop;
                EvaluateState();
            }
        }

        private void EnterAwaitMenuToggle() { }

        private void EnterTimeline()
        {
            // 设置人物位置
            var player = GameObject.FindGameObjectWithTag("Player");
            EventBus.Publish(new EJournalStepChanged("reachRooftop", 0, StepState.Done));
            player.transform.position = initPos;
            // 启动timeline
            if (director != null)
            {
                director.time = 0;
                director.Play();
            }
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

        private void EnterMenuTutorial()
        {
            // 禁止玩家移动
            player.model.SetDisabled(true);
            // 开始播放动画
            StartCoroutine(mover.PlayerThirdMove());
        }

        private void EnterInteractBoard()
        {
            StartCoroutine(InteractBoard());
        }

        private IEnumerator InteractBoard()
        {
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
                state = Scene1State.InteractBoard;
                interactOnce = true;
                EvaluateState();
            }
        }
    }
}
