using UnityEngine;
using System;

namespace CardGame
{
    /// <summary>
    /// 回合阶段枚举
    /// </summary>
    public enum TurnPhase
    {
        DrawPhase,      // 抽牌阶段
        MainPhase,      // 主阶段
        EndPhase        // 结束阶段
    }

    /// <summary>
    /// 回合管理器
    /// 负责管理回合流程和阶段
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        // 当前回合数
        private int currentTurn = 0;

        // 当前玩家索引（0或1）
        private int currentPlayerIndex = 0;

        // 当前阶段
        private TurnPhase currentPhase = TurnPhase.DrawPhase;

        // 玩家列表
        private Player[] players = new Player[2];

        [Header("回合设置")]
        [Tooltip("每回合开始时抽牌数量")]
        public int cardsPerTurn = 1;

        // 回合事件
        public event Action<int> OnTurnStart;
        public event Action<int> OnTurnEnd;
        public event Action<TurnPhase> OnPhaseChange;
        public event Action<int, int> OnPlayerChange;

        /// <summary>
        /// 当前回合数
        /// </summary>
        public int CurrentTurn => currentTurn;

        /// <summary>
        /// 当前玩家索引
        /// </summary>
        public int CurrentPlayerIndex => currentPlayerIndex;

        /// <summary>
        /// 当前阶段
        /// </summary>
        public TurnPhase CurrentPhase => currentPhase;

        /// <summary>
        /// 获取当前玩家
        /// </summary>
        public Player GetCurrentPlayer()
        {
            if (currentPlayerIndex >= 0 && currentPlayerIndex < players.Length)
            {
                return players[currentPlayerIndex];
            }
            return null;
        }

        /// <summary>
        /// 获取对手玩家
        /// </summary>
        public Player GetOpponentPlayer()
        {
            int opponentIndex = 1 - currentPlayerIndex;
            if (opponentIndex >= 0 && opponentIndex < players.Length)
            {
                return players[opponentIndex];
            }
            return null;
        }

        /// <summary>
        /// 初始化回合管理器
        /// </summary>
        public void Initialize(Player player1, Player player2)
        {
            players[0] = player1;
            players[1] = player2;
            currentTurn = 0;
            currentPlayerIndex = 0;
            currentPhase = TurnPhase.DrawPhase;
        }

        /// <summary>
        /// 开始新回合
        /// </summary>
        public void StartTurn()
        {
            currentTurn++;
            Player currentPlayer = GetCurrentPlayer();

            if (currentPlayer == null)
            {
                Debug.LogError("TurnManager: 当前玩家为空！");
                return;
            }

            Debug.Log($"=== 回合 {currentTurn} 开始 - 玩家 {currentPlayer.playerName} ===");

            // 进入抽牌阶段
            ChangePhase(TurnPhase.DrawPhase);

            // 玩家回合开始
            currentPlayer.OnTurnStart();

            // 触发场上所有卡牌的回合开始效果
            var effectManager = Effects.EffectManager.Instance;
            if (effectManager != null)
            {
                foreach (var card in currentPlayer.board)
                {
                    if (card != null)
                    {
                        effectManager.TriggerOnTurnStart(card);
                    }
                }
            }

            // 抽牌
            for (int i = 0; i < cardsPerTurn; i++)
            {
                currentPlayer.DrawCard();
            }

            // 触发回合开始事件
            OnTurnStart?.Invoke(currentTurn);

            // 自动进入主阶段
            ChangePhase(TurnPhase.MainPhase);
        }

        /// <summary>
        /// 结束当前回合
        /// </summary>
        public void EndTurn()
        {
            Player currentPlayer = GetCurrentPlayer();

            if (currentPlayer == null)
            {
                Debug.LogError("TurnManager: 当前玩家为空！");
                return;
            }

            Debug.Log($"=== 回合 {currentTurn} 结束 - 玩家 {currentPlayer.playerName} ===");

            // 进入结束阶段
            ChangePhase(TurnPhase.EndPhase);

            // 触发场上所有卡牌的回合结束效果
            var effectManager = Effects.EffectManager.Instance;
            if (effectManager != null)
            {
                foreach (var card in currentPlayer.board)
                {
                    if (card != null)
                    {
                        effectManager.TriggerOnTurnEnd(card);
                    }
                }
            }

            // 玩家回合结束
            currentPlayer.OnTurnEnd();

            // 触发回合结束事件
            OnTurnEnd?.Invoke(currentTurn);

            // 切换到下一个玩家
            SwitchPlayer();
        }

        /// <summary>
        /// 切换玩家
        /// </summary>
        private void SwitchPlayer()
        {
            int previousPlayerIndex = currentPlayerIndex;
            currentPlayerIndex = 1 - currentPlayerIndex;

            Debug.Log($"切换到玩家 {GetCurrentPlayer()?.playerName}");

            // 触发玩家切换事件
            OnPlayerChange?.Invoke(previousPlayerIndex, currentPlayerIndex);

            // 开始新回合
            StartTurn();
        }

        /// <summary>
        /// 改变阶段
        /// </summary>
        public void ChangePhase(TurnPhase newPhase)
        {
            if (currentPhase != newPhase)
            {
                TurnPhase oldPhase = currentPhase;
                currentPhase = newPhase;
                Debug.Log($"阶段变更: {oldPhase} -> {newPhase}");
                OnPhaseChange?.Invoke(newPhase);
            }
        }

        /// <summary>
        /// 检查是否可以执行操作（在主阶段）
        /// </summary>
        public bool CanPerformAction()
        {
            return currentPhase == TurnPhase.MainPhase;
        }

        /// <summary>
        /// 获取回合信息（用于调试）
        /// </summary>
        public string GetTurnInfo()
        {
            Player currentPlayer = GetCurrentPlayer();
            return $"回合 {currentTurn} | 玩家: {currentPlayer?.playerName ?? "未知"} | 阶段: {currentPhase}";
        }
    }
}
