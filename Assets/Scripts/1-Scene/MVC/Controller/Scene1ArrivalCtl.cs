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
        Finished,
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

        private PlayerCtl player;

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
                case Scene1State.Timeline:
                    EnterTimeline();
                    break;
                case Scene1State.GoToBoard:
                    EnterGoToBoard();
                    break;
                case Scene1State.InteractBoard:
                    EnterInteractBoard();
                    break;
                default:
                    break;
            }
        }

        // —— 各状态入口逻辑 ——
        private void EnterTimeline()
        {
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
                guideCtl.StartFirstDialogue(EndGoToBoard);
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
            // 禁止玩家移动
            player.model.SetDisabled(true);
            // 播放人物思考
            var emoteCtl = player.gameObject.GetComponent<PlayerEmoteCtl>();
            emoteCtl.Play(EmoteType.Thinking, 1f);
            yield return new WaitForSecondsRealtime(1.5f);
            // 播放guide
            guideCtl.StartSecondDialogue(EndSteps);
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
                .GetComponent<PlayerInteractCtl>().target;
            var target = (current as Component)?.gameObject;
            if (target != null && target.gameObject.name == "metalSign" && !interactOnce && state == Scene1State.GoToBoard)
            {
                state = Scene1State.InteractBoard;
                interactOnce = true;
                EvaluateState();
            }
        }
    }
}
