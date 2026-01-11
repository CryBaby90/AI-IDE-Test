using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CardGame
{
    /// <summary>
    /// 牌组管理器
    /// 负责创建牌组、洗牌、抽牌等功能
    /// </summary>
    public class DeckManager : MonoBehaviour
    {
        // 牌组列表（卡牌数据）
        private List<CardData> deckData = new List<CardData>();

        // 当前牌组（卡牌实例）
        private Queue<CardInstance> currentDeck = new Queue<CardInstance>();

        // 已抽出的卡牌（用于重新洗牌）
        private List<CardInstance> drawnCards = new List<CardInstance>();

        // 牌组大小限制
        [Header("牌组设置")]
        [Tooltip("牌组最小卡牌数量")]
        public int minDeckSize = 30;

        [Tooltip("牌组最大卡牌数量")]
        public int maxDeckSize = 60;

        /// <summary>
        /// 当前牌组剩余卡牌数量
        /// </summary>
        public int RemainingCards => currentDeck.Count;

        /// <summary>
        /// 牌组是否为空
        /// </summary>
        public bool IsEmpty => currentDeck.Count == 0;

        /// <summary>
        /// 从卡牌数据列表创建牌组
        /// </summary>
        public void CreateDeck(List<CardData> cards)
        {
            if (cards == null || cards.Count < minDeckSize)
            {
                Debug.LogError($"DeckManager: 牌组卡牌数量不足！需要至少{minDeckSize}张卡牌。");
                return;
            }

            if (cards.Count > maxDeckSize)
            {
                Debug.LogWarning($"DeckManager: 牌组卡牌数量超过限制！将只使用前{maxDeckSize}张卡牌。");
                cards = cards.Take(maxDeckSize).ToList();
            }

            deckData = new List<CardData>(cards);
            ShuffleDeck();
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        public void ShuffleDeck()
        {
            // 将已抽出的卡牌重新加入牌组
            List<CardInstance> allCards = new List<CardInstance>(currentDeck);
            allCards.AddRange(drawnCards);

            // 如果牌组为空，从deckData创建新实例
            if (allCards.Count == 0 && deckData.Count > 0)
            {
                foreach (var cardData in deckData)
                {
                    allCards.Add(new CardInstance(cardData));
                }
            }

            // Fisher-Yates洗牌算法
            for (int i = allCards.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                CardInstance temp = allCards[i];
                allCards[i] = allCards[randomIndex];
                allCards[randomIndex] = temp;
            }

            // 重新构建队列
            currentDeck = new Queue<CardInstance>(allCards);
            drawnCards.Clear();

            Debug.Log($"DeckManager: 牌组已洗牌，剩余{currentDeck.Count}张卡牌。");
        }

        /// <summary>
        /// 抽一张牌
        /// </summary>
        public CardInstance DrawCard()
        {
            if (currentDeck.Count == 0)
            {
                Debug.LogWarning("DeckManager: 牌组已空，无法抽牌！");
                return null;
            }

            CardInstance card = currentDeck.Dequeue();
            drawnCards.Add(card);
            return card;
        }

        /// <summary>
        /// 抽多张牌
        /// </summary>
        public List<CardInstance> DrawCards(int count)
        {
            List<CardInstance> cards = new List<CardInstance>();
            for (int i = 0; i < count; i++)
            {
                CardInstance card = DrawCard();
                if (card != null)
                {
                    cards.Add(card);
                }
                else
                {
                    break;
                }
            }
            return cards;
        }

        /// <summary>
        /// 将卡牌放回牌组顶部
        /// </summary>
        public void PutCardOnTop(CardInstance card)
        {
            if (card != null)
            {
                Queue<CardInstance> newDeck = new Queue<CardInstance>();
                newDeck.Enqueue(card);
                while (currentDeck.Count > 0)
                {
                    newDeck.Enqueue(currentDeck.Dequeue());
                }
                currentDeck = newDeck;
            }
        }

        /// <summary>
        /// 将卡牌放回牌组底部
        /// </summary>
        public void PutCardOnBottom(CardInstance card)
        {
            if (card != null)
            {
                currentDeck.Enqueue(card);
            }
        }

        /// <summary>
        /// 查看牌组顶部卡牌（不抽取）
        /// </summary>
        public CardInstance PeekTopCard()
        {
            if (currentDeck.Count == 0)
            {
                return null;
            }
            return currentDeck.Peek();
        }

        /// <summary>
        /// 清空牌组
        /// </summary>
        public void ClearDeck()
        {
            currentDeck.Clear();
            drawnCards.Clear();
            deckData.Clear();
        }

        /// <summary>
        /// 获取牌组信息（用于调试）
        /// </summary>
        public string GetDeckInfo()
        {
            return $"牌组剩余: {RemainingCards}张, 已抽出: {drawnCards.Count}张";
        }
    }
}
