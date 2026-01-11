using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_TEXTMESHPRO
using TMPro;
#endif

namespace CardGame.UI
{
    /// <summary>
    /// 卡牌UI组件
    /// 显示卡牌信息、处理卡牌交互
    /// </summary>
    public class CardUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("UI引用")]
        public Image cardImage;
        public Image cardArtImage;
#if UNITY_TEXTMESHPRO
        public TextMeshProUGUI cardNameText;
        public TextMeshProUGUI cardDescriptionText;
        public TextMeshProUGUI costText;
        public TextMeshProUGUI attackText;
        public TextMeshProUGUI healthText;
#else
        public Text cardNameText;
        public Text cardDescriptionText;
        public Text costText;
        public Text attackText;
        public Text healthText;
#endif

        [Header("卡牌设置")]
        public CardInstance cardInstance;
        public bool isDraggable = true;
        public bool isPlayable = true;

        // 拖拽相关
        private Canvas canvas;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Vector2 originalPosition;
        private Transform originalParent;

        // 事件
        public System.Action<CardUI> OnCardClicked;
        public System.Action<CardUI> OnCardPlayed;
        public System.Action<CardUI> OnCardDragStart;
        public System.Action<CardUI> OnCardDragEnd;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            // 查找Canvas
            canvas = GetComponentInParent<Canvas>();
        }

        /// <summary>
        /// 设置卡牌实例
        /// </summary>
        public void SetCardInstance(CardInstance card)
        {
            cardInstance = card;
            UpdateUI();
        }

        /// <summary>
        /// 更新UI显示
        /// </summary>
        public void UpdateUI()
        {
            if (cardInstance == null || cardInstance.cardData == null)
            {
                return;
            }

            CardData data = cardInstance.cardData;

            // 更新文本
            if (cardNameText != null)
                cardNameText.text = data.cardName;

            if (cardDescriptionText != null)
                cardDescriptionText.text = data.description;

            if (costText != null)
                costText.text = cardInstance.currentCost.ToString();

            // 仅随从卡牌显示攻击力和生命值
            if (data.cardType == CardType.Minion)
            {
                if (attackText != null)
                {
                    attackText.text = cardInstance.currentAttack.ToString();
                    attackText.gameObject.SetActive(true);
                }

                if (healthText != null)
                {
                    healthText.text = cardInstance.currentHealth.ToString();
                    healthText.gameObject.SetActive(true);
                }
            }
            else
            {
                if (attackText != null)
                    attackText.gameObject.SetActive(false);

                if (healthText != null)
                    healthText.gameObject.SetActive(false);
            }

            // 更新图片
            if (cardArtImage != null && data.cardArt != null)
                cardArtImage.sprite = data.cardArt;

            if (cardImage != null && data.cardIcon != null)
                cardImage.sprite = data.cardIcon;
        }

        /// <summary>
        /// 设置是否可交互
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            isPlayable = interactable;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = interactable ? 1f : 0.5f;
                canvasGroup.blocksRaycasts = interactable;
            }
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isPlayable)
            {
                OnCardClicked?.Invoke(this);
            }
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isDraggable || !isPlayable)
            {
                return;
            }

            originalPosition = rectTransform.anchoredPosition;
            originalParent = rectTransform.parent;

            // 设置为顶层
            transform.SetParent(canvas.transform);
            transform.SetAsLastSibling();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0.6f;
                canvasGroup.blocksRaycasts = false;
            }

            OnCardDragStart?.Invoke(this);
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (!isDraggable || !isPlayable)
            {
                return;
            }

            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDraggable || !isPlayable)
            {
                return;
            }

            // 恢复位置和父对象
            rectTransform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }

            OnCardDragEnd?.Invoke(this);

            // 检查是否在可放置区域
            // 这里可以添加放置区域的检测逻辑
        }

        /// <summary>
        /// 播放卡牌动画（可选）
        /// </summary>
        public void PlayAnimation(string animationName)
        {
            // 可以添加动画组件来实现卡牌动画效果
            // 例如：高亮、缩放、旋转等
        }
    }
}
