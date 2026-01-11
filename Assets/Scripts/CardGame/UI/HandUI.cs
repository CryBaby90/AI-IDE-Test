using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    /// <summary>
    /// 手牌UI管理器
    /// 管理手牌区域的显示和布局
    /// </summary>
    public class HandUI : MonoBehaviour
    {
        [Header("UI设置")]
        public GameObject cardPrefab;
        public Transform cardContainer;
        public HorizontalLayoutGroup layoutGroup;

        [Header("布局设置")]
        public float cardSpacing = 10f;
        public float cardScale = 1f;
        public bool autoLayout = true;

        // 卡牌UI列表
        private List<CardUI> cardUIs = new List<CardUI>();

        // 手牌管理器引用
        private HandManager handManager;

        void Awake()
        {
            // 如果没有指定容器，使用当前对象
            if (cardContainer == null)
            {
                cardContainer = transform;
            }

            // 设置布局组
            if (layoutGroup == null)
            {
                layoutGroup = cardContainer.GetComponent<HorizontalLayoutGroup>();
                if (layoutGroup == null)
                {
                    layoutGroup = cardContainer.gameObject.AddComponent<HorizontalLayoutGroup>();
                }
            }

            layoutGroup.spacing = cardSpacing;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        }

        /// <summary>
        /// 设置手牌管理器
        /// </summary>
        public void SetHandManager(HandManager manager)
        {
            handManager = manager;
            if (handManager != null)
            {
                UpdateHandUI();
            }
        }

        /// <summary>
        /// 更新手牌UI
        /// </summary>
        public void UpdateHandUI()
        {
            if (handManager == null)
            {
                return;
            }

            // 清除现有UI
            ClearHandUI();

            // 创建新卡牌UI
            var hand = handManager.Hand;
            foreach (var card in hand)
            {
                CreateCardUI(card);
            }

            // 更新布局
            if (autoLayout)
            {
                UpdateLayout();
            }
        }

        /// <summary>
        /// 创建卡牌UI
        /// </summary>
        private void CreateCardUI(CardInstance card)
        {
            if (cardPrefab == null)
            {
                Debug.LogWarning("HandUI: 卡牌预制体未设置！");
                return;
            }

            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            cardObj.transform.localScale = Vector3.one * cardScale;

            CardUI cardUI = cardObj.GetComponent<CardUI>();
            if (cardUI == null)
            {
                cardUI = cardObj.AddComponent<CardUI>();
            }

            cardUI.SetCardInstance(card);
            cardUI.OnCardClicked += OnCardClicked;
            cardUI.OnCardPlayed += OnCardPlayed;

            cardUIs.Add(cardUI);
        }

        /// <summary>
        /// 添加卡牌到UI
        /// </summary>
        public void AddCard(CardInstance card)
        {
            CreateCardUI(card);
            if (autoLayout)
            {
                UpdateLayout();
            }
        }

        /// <summary>
        /// 移除卡牌UI
        /// </summary>
        public void RemoveCard(CardInstance card)
        {
            CardUI cardUI = cardUIs.Find(ui => ui.cardInstance == card);
            if (cardUI != null)
            {
                cardUIs.Remove(cardUI);
                Destroy(cardUI.gameObject);
                if (autoLayout)
                {
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// 更新布局
        /// </summary>
        private void UpdateLayout()
        {
            // 可以根据手牌数量调整卡牌大小和间距
            int cardCount = cardUIs.Count;
            if (cardCount > 0)
            {
                // 计算合适的缩放
                float scale = Mathf.Clamp(1f - (cardCount - 1) * 0.05f, 0.7f, 1f);
                foreach (var cardUI in cardUIs)
                {
                    cardUI.transform.localScale = Vector3.one * scale * cardScale;
                }
            }
        }

        /// <summary>
        /// 清除所有手牌UI
        /// </summary>
        public void ClearHandUI()
        {
            foreach (var cardUI in cardUIs)
            {
                if (cardUI != null)
                {
                    Destroy(cardUI.gameObject);
                }
            }
            cardUIs.Clear();
        }

        /// <summary>
        /// 卡牌点击回调
        /// </summary>
        private void OnCardClicked(CardUI cardUI)
        {
            Debug.Log($"HandUI: 卡牌被点击: {cardUI.cardInstance?.cardData?.cardName}");
            // 这里可以添加卡牌详情显示等逻辑
        }

        /// <summary>
        /// 卡牌打出回调
        /// </summary>
        private void OnCardPlayed(CardUI cardUI)
        {
            Debug.Log($"HandUI: 卡牌被打出: {cardUI.cardInstance?.cardData?.cardName}");
            RemoveCard(cardUI.cardInstance);
        }

        /// <summary>
        /// 设置卡牌是否可交互
        /// </summary>
        public void SetCardsInteractable(bool interactable)
        {
            foreach (var cardUI in cardUIs)
            {
                if (cardUI != null)
                {
                    cardUI.SetInteractable(interactable);
                }
            }
        }

        void OnDestroy()
        {
            ClearHandUI();
        }
    }
}
