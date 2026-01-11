using UnityEngine;
using System;

namespace CardGame.Events
{
    // 前向声明，使用CardGame命名空间中的类型
    using CardGame;
    /// <summary>
    /// 游戏事件定义
    /// </summary>
    public static class CardGameEvents
    {
        // 卡牌相关事件
        public static event Action<CardInstance> OnCardPlayed;
        public static event Action<CardInstance> OnCardDrawn;
        public static event Action<CardInstance> OnCardEntered;
        public static event Action<CardInstance> OnCardDeath;
        public static event Action<CardInstance, CardInstance> OnCardAttack;

        // 玩家相关事件
        public static event Action<Player> OnPlayerDamage;
        public static event Action<Player> OnPlayerHeal;
        public static event Action<Player> OnPlayerDeath;

        // 回合相关事件
        public static event Action<int> OnTurnStart;
        public static event Action<int> OnTurnEnd;
        public static event Action<TurnPhase> OnPhaseChange;

        // 游戏状态相关事件
        public static event Action<GameState> OnGameStateChanged;
        public static event Action<Player> OnGameEnd;

        /// <summary>
        /// 触发卡牌打出事件
        /// </summary>
        public static void TriggerCardPlayed(CardInstance card)
        {
            OnCardPlayed?.Invoke(card);
        }

        /// <summary>
        /// 触发抽牌事件
        /// </summary>
        public static void TriggerCardDrawn(CardInstance card)
        {
            OnCardDrawn?.Invoke(card);
        }

        /// <summary>
        /// 触发卡牌入场事件
        /// </summary>
        public static void TriggerCardEntered(CardInstance card)
        {
            OnCardEntered?.Invoke(card);
        }

        /// <summary>
        /// 触发卡牌死亡事件
        /// </summary>
        public static void TriggerCardDeath(CardInstance card)
        {
            OnCardDeath?.Invoke(card);
        }

        /// <summary>
        /// 触发卡牌攻击事件
        /// </summary>
        public static void TriggerCardAttack(CardInstance attacker, CardInstance target)
        {
            OnCardAttack?.Invoke(attacker, target);
        }

        /// <summary>
        /// 触发玩家受到伤害事件
        /// </summary>
        public static void TriggerPlayerDamage(Player player)
        {
            OnPlayerDamage?.Invoke(player);
        }

        /// <summary>
        /// 触发玩家恢复生命值事件
        /// </summary>
        public static void TriggerPlayerHeal(Player player)
        {
            OnPlayerHeal?.Invoke(player);
        }

        /// <summary>
        /// 触发玩家死亡事件
        /// </summary>
        public static void TriggerPlayerDeath(Player player)
        {
            OnPlayerDeath?.Invoke(player);
        }

        /// <summary>
        /// 触发回合开始事件
        /// </summary>
        public static void TriggerTurnStart(int turnNumber)
        {
            OnTurnStart?.Invoke(turnNumber);
        }

        /// <summary>
        /// 触发回合结束事件
        /// </summary>
        public static void TriggerTurnEnd(int turnNumber)
        {
            OnTurnEnd?.Invoke(turnNumber);
        }

        /// <summary>
        /// 触发阶段变更事件
        /// </summary>
        public static void TriggerPhaseChange(TurnPhase phase)
        {
            OnPhaseChange?.Invoke(phase);
        }

        /// <summary>
        /// 触发游戏状态变更事件
        /// </summary>
        public static void TriggerGameStateChanged(GameState newState)
        {
            OnGameStateChanged?.Invoke(newState);
        }

        /// <summary>
        /// 触发游戏结束事件
        /// </summary>
        public static void TriggerGameEnd(Player winner)
        {
            OnGameEnd?.Invoke(winner);
        }

        /// <summary>
        /// 清空所有事件订阅（用于游戏重置）
        /// </summary>
        public static void ClearAllEvents()
        {
            OnCardPlayed = null;
            OnCardDrawn = null;
            OnCardEntered = null;
            OnCardDeath = null;
            OnCardAttack = null;
            OnPlayerDamage = null;
            OnPlayerHeal = null;
            OnPlayerDeath = null;
            OnTurnStart = null;
            OnTurnEnd = null;
            OnPhaseChange = null;
            OnGameStateChanged = null;
            OnGameEnd = null;
        }
    }
}
