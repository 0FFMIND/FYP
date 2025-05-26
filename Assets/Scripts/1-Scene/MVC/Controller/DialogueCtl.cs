using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputSystem;
using AudioSystem;
using UnityEngine.UI;

namespace MVC
{
    public enum Eact
    {
        none,
        shake,
        playHeartBeat,
        playHint,
        playBGM,
        arrowRed,
        stopBGM,
    }
    [System.Serializable]
    public struct LineMapping
    {
        [Tooltip("�� nodeID ���ڴ�ֵʱ")]
        public int nodeID;
        [Tooltip("�� index ���ڴ�ֵʱ���л�����Ӧ�� sprite")]
        public int lineIndex;
        [Tooltip("�л�ʱʹ�õ� Sprite")]
        public Sprite sprite;
        [Tooltip("������Ϊ")]
        public Eact[] eacts;
    }
    public class DialogueCtl : MonoBehaviour
    {

        [Header("��ҳ��ͷλ��")]
        public Transform arrow;
        [SerializeField] private float arrowOffset;      // �״ζ�λ������ƫ��
        [SerializeField] private int downFrames;      // �����ƶ�ʱ�ȴ�֡��
        [SerializeField] private float downDistance;   // �����ƶ�������/���ص�λ
        [SerializeField] private int upFrames;      // �����ƶ�ʱ�ȴ�֡��
        // �Ի�
        [Header("ScriptableObject �Ի���Դ")]
        [SerializeField] private ChoiceScript script;
        public DialogueView view;
        [SerializeField]
        private LineMapping[] mappings;
        [SerializeField]
        private float typeSpeed;
        [SerializeField]
        private CameraShake cameraShake;
        //
        private ChoiceModel choiceModel;
        private DialogueModel dialogueModel;
        private Sprite currentSprite;
        private int currentNodeID;
        private int index;
        private Coroutine typingCoroutine;
        private Coroutine arrowBounceCoroutine;
        private HashSet<int> visitedNodes = new HashSet<int>(); // ��¼�ѷ��ʵĽڵ� ID

        public void StartDialogue()
        {
            // ��������е�����
            arrow.gameObject.SetActive(false);
            view.Render(null, null);
            // ��ʼ�� ChoiceModel
            choiceModel = new ChoiceModel(script);
            // �趨��ʼ�ڵ����Ӧ DialogueModel
            currentNodeID = choiceModel.startNodeId;
            dialogueModel = choiceModel.GetDialogueModel(currentNodeID);
            // ˢ��index
            index = 0;
            // ע���¼�
            InputManager.Instance.OnAction += OnInputAction;
            // �Զ����ŵ�һ�仰
            NextLine();
        }
        private void OnInputAction(InputAction action)
        {
            if(action == InputAction.DialogueClick)
            {
                if (typingCoroutine != null)
                {
                    // ����ͣ
                    StopCoroutine(typingCoroutine);
                    typingCoroutine = null;
                    if(index <= dialogueModel.Lines.Length)
                    {
                        view.Render(currentSprite, dialogueModel.Lines[index - 1]);
                        // ����С��ͷ
                        PositionArrowUnderText();
                    }
                    else
                    {
                        // ������ʱ�����
                        view.Render(null, null);
                        // �ص�С��ͷ
                        arrow.gameObject.SetActive(false);
                        arrow.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
                else
                {
                    NextLine();
                }
            }
        }

        private void ShowAvailableChoices(ChoiceNode node)
        {
            var available = new List<Choice>();
            bool allBranchesDone = true;
            foreach (var choice in node.choices)
            {
                if (!visitedNodes.Contains(choice.targetNodeId))
                {
                    allBranchesDone = false;
                }
                bool unlocked = true;

                // ����� prereq���ͼ��ÿ�� prereq �Ƿ��� visitedNodes ��
                if (choice.prereqNodeIds != null)
                {
                    foreach (var req in choice.prereqNodeIds)
                    {
                        if (!visitedNodes.Contains(req))
                        {
                            unlocked = false;
                            break;
                        }
                    }
                }

                if (unlocked)
                    available.Add(choice);
            }
            if (allBranchesDone)
            {
                // ������з�֧����
                // ������һ���ڵ�
                OnChoiceSelected(node.nextNodeId);
            }
            else
            {
                view.ShowChoices(visitedNodes, node.choicesTxt, available.ToArray(), OnChoiceSelected);
            }
        }

        private void NextLine()
        {
            arrow.GetComponent<SpriteRenderer>().color = Color.white;
            // ��Ȼ��ť���������Ϊnextline
            if (dialogueModel == null || index > dialogueModel.Lines.Length)
            {
                return;
            }
            // �������
            if(index == dialogueModel.Lines.Length)
            {
                // �ص�С��ͷ
                arrow.gameObject.SetActive(false);
                //view.Render(null, null);
                //InputManager.Instance.OnAction -= OnInputAction;
                // ���������ʾѡ��
                var node = choiceModel.GetNode(currentNodeID);
                if (node != null && node.postNodeId != -1)
                {
                    // ������һ���ڵ�
                    ChoiceNode parent = choiceModel.GetNode(node.postNodeId);
                    // ���� currentNodeID & model
                    currentNodeID = parent.nodeId;
                    dialogueModel = null;

                    // ֱ����ʾ parent �ķ�֧ѡ����������ĶԻ�
                    ShowAvailableChoices(parent);
                }
                else
                {
                    if (node != null && node.choices != null && node.choices.Length > 0)
                    {
                        ShowAvailableChoices(node);
                    }
                }
                // ������ֻ��Ӧһ��
                index++;
                return;
            }
            foreach(var map in mappings)
            {
                if(index == map.lineIndex && currentNodeID == map.nodeID)
                {
                    currentSprite = map.sprite;
                    foreach(Eact eact in map.eacts)
                    {
                        if (eact != Eact.none)
                        {
                            if (eact == Eact.shake)
                            {
                                // ����camera shake��������Ч
                                AudioManager.Instance.PlaySFX("shocked");
                                cameraShake.Shake();
                            }
                            if (eact == Eact.playBGM)
                            {
                                AudioManager.Instance.PlayBGM("1-bgm");
                            }
                            if(eact == Eact.arrowRed)
                            {
                                arrow.GetComponent<SpriteRenderer>().color = Color.red;
                            }
                            if(eact == Eact.playHeartBeat)
                            {
                                AudioManager.Instance.PlaySFX("heartbeat");
                            }
                            if(eact == Eact.stopBGM)
                            {
                                AudioManager.Instance.StopBGM();
                            }
                        }
                    }

                    break;
                }
            }
            string text = dialogueModel.Lines[index];
            // ����
            typingCoroutine = StartCoroutine(TypeLines(text));
            // �ƶ�����һ��line
            index++;
        }

        private void OnChoiceSelected(int targetNodeId)
        {
            // ��ӵ����ʹ��Ľڵ�
            visitedNodes.Add(targetNodeId);
            // �л��ڵ㣬�����µ� txt
            currentNodeID = targetNodeId;
            dialogueModel = choiceModel.GetDialogueModel(currentNodeID);
            index = 0;

            view.HideChoices();
            view.Render(null, null);

            NextLine();
        }

        private IEnumerator TypeLines(string fullText)
        {
            arrow.gameObject.SetActive(false);
            view.Render(currentSprite, "");

            string displayed = "";
            var openTags = new Stack<string>();
            bool inHint = false;    // ��ǵ�ǰ�Ƿ��� hint ����

            for (int i = 0; i < fullText.Length; i++)
            {
                if (fullText[i] == '<')
                {
                    int close = fullText.IndexOf('>', i);
                    if (close != -1)
                    {
                        string rawTag = fullText.Substring(i, close - i + 1);
                        bool isCloseTag = rawTag.StartsWith("</");
                        string inner = rawTag.Trim('<', '/', '>');
                        string tagName = inner.Split(new[] { ' ', '=' }, 2)[0];

                        if (!isCloseTag && tagName == "hint")
                        {
                            // ���� hint ����
                            inHint = true;
                            AudioManager.Instance.PlaySFX("notice");
                            displayed += "<color=#FF0000>";
                            openTags.Push("color");
                        }
                        else if (isCloseTag && tagName == "hint")
                        {
                            // �뿪 hint ����
                            inHint = false;
                            displayed += "</color>";
                            if (openTags.Count > 0) openTags.Pop();
                        }
                        else
                        {
                            // ��ͨ��ǩ����
                            displayed += rawTag;
                            if (isCloseTag)
                            {
                                if (openTags.Count > 0) openTags.Pop();
                            }
                            else if (!rawTag.EndsWith("/>"))
                            {
                                int sep = rawTag.IndexOfAny(new[] { ' ', '=', '>' }, 1);
                                string name = rawTag.Substring(1, (sep > 0 ? sep : rawTag.Length - 2) - 1);
                                openTags.Push(name);
                            }
                        }

                        i = close;
                    }
                    else
                    {
                        displayed += fullText[i];
                    }

                    // ��ǩ���������Ч��������Ⱦ
                    view.Render(currentSprite, displayed);
                    yield return null;
                }
                else
                {
                    // ��ͨ����
                    displayed += fullText[i];
                    AudioManager.Instance.PlaySFX("typing");

                    // ƴ�ӱպϱ�ǩ��֤ TMP ��ȷ��Ⱦ
                    string temp = displayed;
                    foreach (var name in openTags)
                        temp += $"</{name}>";

                    view.Render(currentSprite, temp);

                    // ��ͬ�����ò�ͬ�ٶ�
                    float wait = inHint ? typeSpeed * 3f : typeSpeed;
                    yield return new WaitForSeconds(wait);
                }
            }

            // ��ɺ���ʾ��ͷ
            PositionArrowUnderText();
            typingCoroutine = null;
        }


        private void PositionArrowUnderText()
        {
            view.tmp.ForceMeshUpdate();
            Bounds b = view.tmp.textBounds;
            Vector3 localBotCenter = new Vector3(b.center.x, b.min.y, 0);
            Vector3 worldBotCenter = view.tmp.transform.TransformPoint(localBotCenter);
            Vector3 downOffset = Vector3.down * arrowOffset;
            arrow.position = new Vector3(arrow.position.x, worldBotCenter.y + downOffset.y, arrow.position.z);
            // ��ʾ��������ƫ��
            arrow.gameObject.SetActive(true);
            // ��������
            if (arrowBounceCoroutine != null) StopCoroutine(arrowBounceCoroutine);
            arrowBounceCoroutine = StartCoroutine(ArrowBounce());
        }
        private IEnumerator ArrowBounce()
        {
            // ��¼ԭʼλ��
            Vector3 original = arrow.position;
            Vector3 target = original + Vector3.down * downDistance;
            while (true)
            {
                // ƽ������
                for (int i = 0; i <= downFrames; i++)
                {
                    float t = i / (float)downFrames;  // �� 0 �� 1
                    arrow.position = Vector3.Lerp(original, target, t);
                    yield return null;
                }
                // ƽ������
                for (int i = 0; i <= upFrames; i++)
                {
                    float t = i / (float)upFrames;
                    arrow.position = Vector3.Lerp(target, original, t);
                    yield return null;
                }
            }
        }

        // ȡ������
        private void OnDestroy()
        {
            if (arrowBounceCoroutine != null)
                StopCoroutine(arrowBounceCoroutine);
            InputManager.Instance.OnAction -= OnInputAction;
        }
    }
}

