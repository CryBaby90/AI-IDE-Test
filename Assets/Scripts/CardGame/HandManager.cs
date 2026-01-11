using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CardGame
{
    /// <summary>
    /// 手牌管理器
    /// 负责管理手牌、出牌验证等功能
    /// </summary>
    public class HandManager : MonoBehaviour
    {
        // 手牌列表
        private List<CardInstance> hand = new List<CardInstance>();

        [Header("手牌设置")]
        [Tooltip("手牌上限")]
        public int maxHandSize = 10;

        [Tooltip("手牌超出上限时是否直接丢弃")]
        public bool discardOnOverflow = true;

        /// <summary>
        /// 当前手牌数量
        /// </summary>
        public int HandCount => hand.Count;

        /// <summary>
        /// 手牌是否已满
        /// </summary>
        public bool IsHandFull => hand.Count >= maxHandSize;

        /// <summary>
        /// 获取手牌列表（只读）
        /// </summary>
        public IReadOnlyList<CardInstance> Hand => hand.AsReadOnly();

        /// <summary>
        /// 添加卡牌到手牌
        /// </summary>
        public bool AddCardToHand(CardInstance card)
        {
            if (card == null)
            {
                Debug.LogWarning("HandManager: 尝试添加空卡牌到手牌！");
                return false;
            }

            if (IsHandFull)
            {
                if (discardOnOverflow)
                {
                    Debug.Log($"HandManager: 手牌已满，卡牌 {card.cardData.cardName} 被丢弃。");
                    return false;
                }
                else
                {
                    Debug.LogWarning($"HandManager: 手牌已满，无法添加卡牌 {card.cardData.cardName}！");
                    return false;
                }
            }

            hand.Add(card);
            Debug.Log($"HandManager: 卡牌 {card.cardData.cardName} 已加入手牌。当前手牌数: {hand.Count}");
            return true;
        }

        /// <summary>
        /// 从手牌移除卡牌
        /// </summary>
        public bool RemoveCardFromHand(CardInstance card)
        {
            if (card == null)
            {
                return false;
            }

            bool removed = hand.Remove(card);
            if (removed)
            {
                Debug.Log($"HandManager: 卡牌 {card.cardData.cardName} 已从手牌移除。当前手牌数: {hand.Count}");
            }
            return removed;
        }

        /// <summary>
        /// 通过索引移除卡牌
        /// </summary>
        public CardInstance RemoveCardAt(int index)
        {
            if (index < 0 || index >= hand.Count)
            {
                Debug.LogWarning($"HandManager: 无效的手牌索引 {index}！");
                return null;
            }

            CardInstance card = hand[index];
            hand.RemoveAt(index);
            return card;
        }

        /// <summary>
        /// 验证是否可以打出卡牌
        /// </summary>
        public bool CanPlayCard(CardInstance card, int currentMana)
        {
            if (card == null)
            {
                return false;
            }

            if (!hand.Contains(card))
            {
                Debug.LogWarning($"HandManager: 卡牌 {card.cardData.cardName} 不在手牌中！");
                return false;
            }

            if (card.currentCost > currentMana)
            {
                Debug.Log($"HandManager: 法力值不足，无法打出 {card.cardData.cardName}！需要 {card.currentCost}，当前 {currentMana}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 打出卡牌（从手牌移除）
        /// </summary>
        public CardInstance PlayCard(CardInstance card, int currentMana)
        {
            if (!CanPlayCard(card, currentMana))
            {
                return null;
            }

            RemoveCardFromHand(card);
            return card;
        }

        /// <summary>
        /// 通过索引打出卡牌
        /// </summary>
        public CardInstance PlayCardAt(int index, int currentMana)
        {
            if (index < 0 || index >= hand.Count)
            {
                return null;
            }

            CardInstance card = hand[index];
            return PlayCard(card, currentMana);
        }

        /// <summary>
        /// 对手牌进行排序
        /// </summary>
        public void SortHand(System.Comparison<CardInstance> comparison = null)
        {
            if (comparison == null)
            {
                // 默认按费用排序
                hand.Sort((a, b) => a.currentCost.CompareTo(b.currentCost));
            }
            else
            {
                hand.Sort(comparison);
            }
        }

        /// <summary>
        /// 清空手牌
        /// </summary>
        public void ClearHand()
        {
            hand.Clear();
            Debug.Log("HandManager: 手牌已清空。");
        }

        /// <summary>
        /// 获取指定索引的卡牌
        /// </summary>
        public CardInstance GetCardAt(int index)
        {
            if (index < 0 || index >= hand.Count)
            {
                return null;
            }
            return hand[index];
        }

        /// <summary>
        /// 检查手牌中是否有指定卡牌
        /// </summary>
        public bool HasCard(CardInstance card)
        {
            return hand.Contains(card);
        }

        /// <summary>
        /// 获取手牌信息（用于调试）
        /// </summary>
        public string GetHandInfo()
        {
            string info = $"手牌数量: {hand.Count}/{maxHandSize}\n";
            for (int i = 0; i < hand.Count; i++)
            {
                var card = hand[i];
                info += $"{i + 1}. {card.cardData.cardName} (费用: {card.currentCost})\n";
            }
            return info;
        }
    }
}
