using System.Collections.Generic;
using UnityEngine;
using CardGame.Events;

namespace CardGame
{
    /// <summary>
    /// 玩家类
    /// 包含玩家的所有状态信息
    /// </summary>
    public class Player
    {
        // 玩家属性
        public int maxHealth = 30;
        public int currentHealth;
        public int maxMana = 10;
        public int currentMana;
        public int manaCrystals = 0; // 法力水晶数量

        // 玩家ID
        public int playerID;
        public string playerName;

        // 手牌管理器
        public HandManager handManager;

        // 牌组管理器
        public DeckManager deckManager;

        // 场上卡牌列表
        public List<CardInstance> board = new List<CardInstance>();

        // 墓地（已死亡的卡牌）
        public List<CardInstance> graveyard = new List<CardInstance>();

        // 是否已结束回合
        public bool hasEndedTurn = false;

        // 场上最大随从数量
        public int maxBoardSize = 7;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Player(int id, string name, int health = 30)
        {
            playerID = id;
            playerName = name;
            maxHealth = health;
            currentHealth = maxHealth;
            currentMana = 0;
            manaCrystals = 0;
        }

        /// <summary>
        /// 初始化玩家（创建管理器组件）
        /// </summary>
        public void Initialize(GameObject playerObject)
        {
            if (playerObject == null)
            {
                Debug.LogError("Player: 无法在空GameObject上初始化玩家！");
                return;
            }

            // 创建或获取手牌管理器
            handManager = playerObject.GetComponent<HandManager>();
            if (handManager == null)
            {
                handManager = playerObject.AddComponent<HandManager>();
            }

            // 创建或获取牌组管理器
            deckManager = playerObject.GetComponent<DeckManager>();
            if (deckManager == null)
            {
                deckManager = playerObject.AddComponent<DeckManager>();
            }
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        public void TakeDamage(int damage)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
            Debug.Log($"玩家 {playerName} 受到 {damage} 点伤害，剩余生命值: {currentHealth}/{maxHealth}");
            
            // 触发事件
            Events.CardGameEvents.TriggerPlayerDamage(this);
            
            // 检查是否死亡
            if (IsDead())
            {
                Events.CardGameEvents.TriggerPlayerDeath(this);
            }
        }

        /// <summary>
        /// 恢复生命值
        /// </summary>
        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            Debug.Log($"玩家 {playerName} 恢复 {amount} 点生命值，当前生命值: {currentHealth}/{maxHealth}");
        }

        /// <summary>
        /// 消耗法力值
        /// </summary>
        public bool ConsumeMana(int amount)
        {
            if (currentMana >= amount)
            {
                currentMana -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 增加法力水晶（回合开始时）
        /// </summary>
        public void GainManaCrystal()
        {
            if (manaCrystals < maxMana)
            {
                manaCrystals++;
            }
        }

        /// <summary>
        /// 回合开始：恢复法力值并增加法力水晶
        /// </summary>
        public void OnTurnStart()
        {
            GainManaCrystal();
            currentMana = manaCrystals;
            hasEndedTurn = false;
            Debug.Log($"玩家 {playerName} 回合开始，法力值: {currentMana}/{manaCrystals}");
        }

        /// <summary>
        /// 回合结束
        /// </summary>
        public void OnTurnEnd()
        {
            hasEndedTurn = true;
            Debug.Log($"玩家 {playerName} 回合结束");
        }

        /// <summary>
        /// 抽牌
        /// </summary>
        public CardInstance DrawCard()
        {
            if (deckManager == null)
            {
                Debug.LogError($"Player: 玩家 {playerName} 的牌组管理器未初始化！");
                return null;
            }

            CardInstance card = deckManager.DrawCard();
            if (card != null && handManager != null)
            {
                if (handManager.AddCardToHand(card))
                {
                    // 触发抽牌事件
                    Events.CardGameEvents.TriggerCardDrawn(card);
                }
            }
            return card;
        }

        /// <summary>
        /// 将卡牌放置到场上
        /// </summary>
        public bool PlaceCardOnBoard(CardInstance card)
        {
            if (card == null)
            {
                return false;
            }

            if (board.Count >= maxBoardSize)
            {
                Debug.LogWarning($"Player: 场上已满，无法放置更多卡牌！");
                return false;
            }

            board.Add(card);
            Debug.Log($"玩家 {playerName} 将 {card.cardData.cardName} 放置到场上");
            return true;
        }

        /// <summary>
        /// 从场上移除卡牌
        /// </summary>
        public bool RemoveCardFromBoard(CardInstance card)
        {
            if (card == null)
            {
                return false;
            }

            bool removed = board.Remove(card);
            if (removed)
            {
                Debug.Log($"玩家 {playerName} 从场上移除 {card.cardData.cardName}");
            }
            return removed;
        }

        /// <summary>
        /// 将卡牌送入墓地（触发死亡效果和事件）
        /// </summary>
        public void SendToGraveyard(CardInstance card)
        {
            if (card != null)
            {
                // 触发死亡效果（在移除前触发，以便效果可以访问卡牌信息）
                var effectManager = Effects.EffectManager.Instance;
                if (effectManager != null)
                {
                    effectManager.TriggerOnDeath(card);
                }
                
                // 触发死亡事件
                Events.CardGameEvents.TriggerCardDeath(card);
                
                graveyard.Add(card);
                RemoveCardFromBoard(card);
                Debug.Log($"玩家 {playerName} 将 {card.cardData.cardName} 送入墓地");
            }
        }

        /// <summary>
        /// 检查是否死亡
        /// </summary>
        public bool IsDead()
        {
            return currentHealth <= 0;
        }

        /// <summary>
        /// 获取玩家信息（用于调试）
        /// </summary>
        public string GetPlayerInfo()
        {
            return $"玩家 {playerName} (ID: {playerID})\n" +
                   $"生命值: {currentHealth}/{maxHealth}\n" +
                   $"法力值: {currentMana}/{manaCrystals}\n" +
                   $"手牌: {handManager?.HandCount ?? 0}\n" +
                   $"场上: {board.Count}/{maxBoardSize}\n" +
                   $"牌组剩余: {deckManager?.RemainingCards ?? 0}\n" +
                   $"墓地: {graveyard.Count}";
        }
    }
}
