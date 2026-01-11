using System.Collections.Generic;
using UnityEngine;

namespace CardGame
{
    /// <summary>
    /// 战场管理器
    /// 管理场上卡牌的位置和状态
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        // 场上卡牌列表（按位置排序）
        private List<CardInstance> boardCards = new List<CardInstance>();

        // 场上最大卡牌数量
        [Header("战场设置")]
        [Tooltip("场上最大随从数量")]
        public int maxBoardSize = 7;

        // 卡牌位置信息（用于UI显示）
        private Dictionary<CardInstance, int> cardPositions = new Dictionary<CardInstance, int>();

        /// <summary>
        /// 当前场上卡牌数量
        /// </summary>
        public int BoardCount => boardCards.Count;

        /// <summary>
        /// 场上是否已满
        /// </summary>
        public bool IsBoardFull => boardCards.Count >= maxBoardSize;

        /// <summary>
        /// 获取场上所有卡牌（只读）
        /// </summary>
        public IReadOnlyList<CardInstance> BoardCards => boardCards.AsReadOnly();

        /// <summary>
        /// 将卡牌放置到场上
        /// </summary>
        public bool PlaceCard(CardInstance card, int position = -1)
        {
            if (card == null)
            {
                Debug.LogWarning("BoardManager: 尝试放置空卡牌！");
                return false;
            }

            if (IsBoardFull)
            {
                Debug.LogWarning("BoardManager: 场上已满，无法放置更多卡牌！");
                return false;
            }

            if (boardCards.Contains(card))
            {
                Debug.LogWarning("BoardManager: 卡牌已在场上！");
                return false;
            }

            // 如果未指定位置，添加到末尾
            if (position < 0 || position > boardCards.Count)
            {
                position = boardCards.Count;
            }

            boardCards.Insert(position, card);
            cardPositions[card] = position;
            UpdatePositions();

            Debug.Log($"BoardManager: 卡牌 {card.cardData.cardName} 已放置到场上位置 {position}");
            return true;
        }

        /// <summary>
        /// 从场上移除卡牌
        /// </summary>
        public bool RemoveCard(CardInstance card)
        {
            if (card == null)
            {
                return false;
            }

            bool removed = boardCards.Remove(card);
            if (removed)
            {
                cardPositions.Remove(card);
                UpdatePositions();
                Debug.Log($"BoardManager: 卡牌 {card.cardData.cardName} 已从场上移除");
            }
            return removed;
        }

        /// <summary>
        /// 移动卡牌位置
        /// </summary>
        public bool MoveCard(CardInstance card, int newPosition)
        {
            if (card == null || !boardCards.Contains(card))
            {
                return false;
            }

            if (newPosition < 0 || newPosition >= boardCards.Count)
            {
                return false;
            }

            boardCards.Remove(card);
            boardCards.Insert(newPosition, card);
            UpdatePositions();

            return true;
        }

        /// <summary>
        /// 更新所有卡牌的位置索引
        /// </summary>
        private void UpdatePositions()
        {
            cardPositions.Clear();
            for (int i = 0; i < boardCards.Count; i++)
            {
                cardPositions[boardCards[i]] = i;
            }
        }

        /// <summary>
        /// 获取卡牌的位置
        /// </summary>
        public int GetCardPosition(CardInstance card)
        {
            if (cardPositions.ContainsKey(card))
            {
                return cardPositions[card];
            }
            return -1;
        }

        /// <summary>
        /// 获取指定位置的卡牌
        /// </summary>
        public CardInstance GetCardAtPosition(int position)
        {
            if (position >= 0 && position < boardCards.Count)
            {
                return boardCards[position];
            }
            return null;
        }

        /// <summary>
        /// 获取相邻的卡牌
        /// </summary>
        public List<CardInstance> GetAdjacentCards(CardInstance card)
        {
            List<CardInstance> adjacent = new List<CardInstance>();
            int position = GetCardPosition(card);

            if (position < 0)
            {
                return adjacent;
            }

            // 左侧卡牌
            if (position > 0)
            {
                adjacent.Add(boardCards[position - 1]);
            }

            // 右侧卡牌
            if (position < boardCards.Count - 1)
            {
                adjacent.Add(boardCards[position + 1]);
            }

            return adjacent;
        }

        /// <summary>
        /// 清空场上所有卡牌
        /// </summary>
        public void ClearBoard()
        {
            boardCards.Clear();
            cardPositions.Clear();
            Debug.Log("BoardManager: 场上已清空");
        }

        /// <summary>
        /// 检查场上是否有指定卡牌
        /// </summary>
        public bool HasCard(CardInstance card)
        {
            return boardCards.Contains(card);
        }

        /// <summary>
        /// 获取场上信息（用于调试）
        /// </summary>
        public string GetBoardInfo()
        {
            string info = $"场上卡牌: {boardCards.Count}/{maxBoardSize}\n";
            for (int i = 0; i < boardCards.Count; i++)
            {
                var card = boardCards[i];
                info += $"位置 {i}: {card.cardData.cardName} (攻击: {card.currentAttack}, 生命: {card.currentHealth})\n";
            }
            return info;
        }
    }
}
