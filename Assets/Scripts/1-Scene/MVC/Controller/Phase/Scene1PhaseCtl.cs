using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.Playables;
using Utils;

namespace MVC
{
    public enum Scene1Phase
    {
        None,
        Timeline,
        SignTutorial,
        MenuTutorial,
        AwaitMenuToggle,
        RooftopExplore,
        MeadowExplore,
    }

    public class Scene1PhaseCtl : MonoBehaviour
    {
        [Header("状态机")]
        [SerializeField]
        private Scene1Phase phase = Scene1Phase.Timeline;

        [Header("依赖引用")]
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

        [SerializeField]
        private PlayerMoveSignal mover;

        private PlayerCtl player;

        private Dictionary<Scene1Phase, IScene1PhaseHandler> handlers;
        private IScene1PhaseHandler currentHandler;

        private void Awake()
        {
            // 这里 new 各个阶段 Handler，把依赖传进去
            handlers = new Dictionary<Scene1Phase, IScene1PhaseHandler>
            {
                { Scene1Phase.None, new Scene1NonePhase(this) },
                { Scene1Phase.Timeline, new Scene1TimelinePhase(this) },
                { Scene1Phase.SignTutorial, new Scene1SignTutorialPhase(this) },
                { Scene1Phase.MenuTutorial, new Scene1MenuTutorialPhase(this) },
                { Scene1Phase.AwaitMenuToggle, new Scene1AwaitMenuTogglePhase(this) },
                { Scene1Phase.RooftopExplore, new Scene1RooftopExplorePhase(this) },
                { Scene1Phase.MeadowExplore, new Scene1MeadowExplorePhase(this) },
            };
        }

        private void Start()
        {
            AudioManager.Instance.PlayBGM("1-bgm-1", 1);

            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtl>();
            player.model.SetDisabled(true);

            TransitionTo(phase);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<EScene1ArrivalPhaseChange>(OnExternalPhaseChange);
            EventBus.Subscribe<EJournalStatusChanged>(OnJournalChanged);
            EventBus.Subscribe<EPauseChanged>(OnPauseChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EScene1ArrivalPhaseChange>(OnExternalPhaseChange);
            EventBus.Unsubscribe<EJournalStatusChanged>(OnJournalChanged);
            EventBus.Unsubscribe<EPauseChanged>(OnPauseChanged);
        }

        public void TransitionTo(Scene1Phase next)
        {
            phase = next;

            if (handlers.TryGetValue(phase, out var h))
            {
                currentHandler = h;
                currentHandler.Enter();
            }
        }

        private void Update()
        {
            currentHandler?.Tick();
        }

        private void OnExternalPhaseChange(EScene1ArrivalPhaseChange e) => TransitionTo(e.Phase);

        private void OnPauseChanged(EPauseChanged e) => currentHandler?.OnPauseChanged(e);

        private void OnJournalChanged(EJournalStatusChanged e)
        {
            if (e.Key == "vendingMachine" && e.NewStatus == JournalStatus.Completed)
            {
                player.model.SetDisabled(true);
                // 唤醒dialog
                timelineCtl.StartClipDialogue(Scene1DialogueId.VendingMachineEnd, () =>
                {
                    player.model.SetDisabled(false);
                });
            }
            currentHandler?.OnJournalChanged(e);
        }

        // ===== 给 Handler 用的只读访问器 =====
        public PlayableDirector Director => director;
        public GuideDialogCtl GuideCtl => guideCtl;
        public TimelineDialogCtl TimelineCtl => timelineCtl;
        public GameObject GuideSteps => guideSteps;
        public Vector3 InitPos => initPos;
        public PlayerMoveSignal Mover => mover;
        public PlayerCtl Player => player;
        public Scene1Phase CurrentPhase => phase;
    }
}
