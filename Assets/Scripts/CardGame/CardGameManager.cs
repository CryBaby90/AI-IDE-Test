using UnityEngine;
using System.Collections.Generic;
using CardGame.Events;

namespace CardGame
{
    /// <summary>
    /// 主游戏管理器
    /// 初始化游戏、管理游戏流程、协调各子系统
    /// </summary>
    public class CardGameManager : MonoBehaviour
    {
        [Header("玩家设置")]
        public string player1Name = "玩家1";
        public string player2Name = "玩家2";
        public int startingHealth = 30;
        public int startingHandSize = 3;

        [Header("牌组设置")]
        public List<CardData> player1Deck;
        public List<CardData> player2Deck;

        // 玩家对象
        private Player player1;
        private Player player2;

        // 管理器引用
        private TurnManager turnManager;
        private GameStateManager stateManager;
        private Effects.EffectManager effectManager;

        // 游戏对象
        private GameObject player1Object;
        private GameObject player2Object;

        private static CardGameManager instance;
        public static CardGameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CardGameManager>();
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            InitializeManagers();
        }

        void Start()
        {
            StartGame();
        }

        /// <summary>
        /// 初始化所有管理器
        /// </summary>
        private void InitializeManagers()
        {
            // 获取或创建管理器
            stateManager = GameStateManager.Instance;
            effectManager = Effects.EffectManager.Instance;

            // 创建回合管理器
            turnManager = GetComponent<TurnManager>();
            if (turnManager == null)
            {
                turnManager = gameObject.AddComponent<TurnManager>();
            }

            // 订阅回合事件
            turnManager.OnTurnStart += OnTurnStart;
            turnManager.OnTurnEnd += OnTurnEnd;
            turnManager.OnPhaseChange += OnPhaseChange;
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            Debug.Log("CardGameManager: 开始初始化游戏...");

            stateManager.ChangeState(GameState.Initializing);

            // 创建玩家对象
            player1Object = new GameObject($"Player_{player1Name}");
            player2Object = new GameObject($"Player_{player2Name}");

            // 创建玩家
            player1 = new Player(0, player1Name, startingHealth);
            player2 = new Player(1, player2Name, startingHealth);

            // 初始化玩家
            player1.Initialize(player1Object);
            player2.Initialize(player2Object);

            // 创建牌组
            if (player1Deck != null && player1Deck.Count > 0)
            {
                player1.deckManager.CreateDeck(player1Deck);
            }
            else
            {
                Debug.LogWarning("CardGameManager: 玩家1的牌组为空！");
            }

            if (player2Deck != null && player2Deck.Count > 0)
            {
                player2.deckManager.CreateDeck(player2Deck);
            }
            else
            {
                Debug.LogWarning("CardGameManager: 玩家2的牌组为空！");
            }

            // 初始抽牌
            for (int i = 0; i < startingHandSize; i++)
            {
                player1.DrawCard();
                player2.DrawCard();
            }

            // 初始化回合管理器
            turnManager.Initialize(player1, player2);

            // 进入准备阶段
            stateManager.ChangeState(GameState.Preparing);

            // 开始游戏
            stateManager.ChangeState(GameState.Playing);

            // 开始第一回合
            turnManager.StartTurn();

            Debug.Log("CardGameManager: 游戏初始化完成！");
        }

        /// <summary>
        /// 回合开始回调
        /// </summary>
        private void OnTurnStart(int turnNumber)
        {
            Debug.Log($"CardGameManager: 回合 {turnNumber} 开始");
            Events.CardGameEvents.TriggerTurnStart(turnNumber);
        }

        /// <summary>
        /// 回合结束回调
        /// </summary>
        private void OnTurnEnd(int turnNumber)
        {
            Debug.Log($"CardGameManager: 回合 {turnNumber} 结束");
            Events.CardGameEvents.TriggerTurnEnd(turnNumber);

            // 检查游戏是否结束
            CheckGameOver();
        }

        /// <summary>
        /// 阶段变更回调
        /// </summary>
        private void OnPhaseChange(TurnPhase phase)
        {
            Events.CardGameEvents.TriggerPhaseChange(phase);
        }

        /// <summary>
        /// 检查游戏是否结束
        /// </summary>
        private void CheckGameOver()
        {
            if (player1.IsDead())
            {
                EndGame(player2);
            }
            else if (player2.IsDead())
            {
                EndGame(player1);
            }
        }

        /// <summary>
        /// 结束游戏
        /// </summary>
        private void EndGame(Player winner)
        {
            stateManager.ChangeState(GameState.GameOver);
            Debug.Log($"CardGameManager: 游戏结束！获胜者: {winner.playerName}");
            Events.CardGameEvents.TriggerGameEnd(winner);
        }

        /// <summary>
        /// 获取玩家1
        /// </summary>
        public Player GetPlayer1()
        {
            return player1;
        }

        /// <summary>
        /// 获取玩家2
        /// </summary>
        public Player GetPlayer2()
        {
            return player2;
        }

        /// <summary>
        /// 获取当前玩家
        /// </summary>
        public Player GetCurrentPlayer()
        {
            return turnManager?.GetCurrentPlayer();
        }

        /// <summary>
        /// 获取回合管理器
        /// </summary>
        public TurnManager GetTurnManager()
        {
            return turnManager;
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        public void RestartGame()
        {
            // 清理
            if (player1Object != null) Destroy(player1Object);
            if (player2Object != null) Destroy(player2Object);

            // 重新开始
            StartGame();
        }

        /// <summary>
        /// 打出卡牌（完整的游戏流程）
        /// </summary>
        /// <param name="card">要打出的卡牌</param>
        /// <param name="player">打出卡牌的玩家</param>
        /// <param name="target">目标（可选，用于法术卡牌等）</param>
        /// <returns>是否成功打出</returns>
        public bool PlayCard(CardInstance card, Player player, GameObject target = null)
        {
            if (card == null || player == null)
            {
                Debug.LogWarning("CardGameManager: 卡牌或玩家为空！");
                return false;
            }

            // 检查游戏状态
            if (!stateManager.CanPerformAction())
            {
                Debug.LogWarning("CardGameManager: 当前游戏状态不允许打出卡牌！");
                return false;
            }

            // 检查回合阶段
            if (!turnManager.CanPerformAction())
            {
                Debug.LogWarning("CardGameManager: 当前回合阶段不允许打出卡牌！");
                return false;
            }

            // 检查是否是当前玩家
            Player currentPlayer = turnManager.GetCurrentPlayer();
            if (currentPlayer != player)
            {
                Debug.LogWarning("CardGameManager: 不是当前玩家的回合！");
                return false;
            }

            // 验证是否可以打出（法力值、手牌等）
            if (!player.handManager.CanPlayCard(card, player.currentMana))
            {
                return false;
            }

            // 消耗法力值
            if (!player.ConsumeMana(card.currentCost))
            {
                Debug.LogWarning($"CardGameManager: 无法消耗法力值！需要 {card.currentCost}，当前 {player.currentMana}");
                return false;
            }

            // 从手牌移除
            CardInstance playedCard = player.handManager.PlayCard(card, player.currentMana);
            if (playedCard == null)
            {
                Debug.LogError("CardGameManager: 从手牌移除卡牌失败！");
                return false;
            }

            Debug.Log($"CardGameManager: 玩家 {player.playerName} 打出卡牌 {card.cardData.cardName}");

            // 触发卡牌打出效果
            effectManager.TriggerOnPlay(playedCard, target);

            // 根据卡牌类型处理
            if (playedCard.cardData.cardType == CardType.Minion)
            {
                // 随从卡牌：放置到场上
                if (player.PlaceCardOnBoard(playedCard))
                {
                    // 触发入场效果
                    effectManager.TriggerOnEnter(playedCard);
                    Events.CardGameEvents.TriggerCardEntered(playedCard);
                }
                else
                {
                    Debug.LogWarning("CardGameManager: 场上已满，无法放置随从！");
                    // 如果场上已满，卡牌应该被丢弃或返回手牌
                    // 这里可以根据游戏规则决定
                }
            }
            else if (playedCard.cardData.cardType == CardType.Spell)
            {
                // 法术卡牌：打出后进入墓地
                player.SendToGraveyard(playedCard);
            }
            // 其他卡牌类型（装备、英雄等）可以根据需要处理

            // 触发事件
            Events.CardGameEvents.TriggerCardPlayed(playedCard);

            return true;
        }

        /// <summary>
        /// 对卡牌造成伤害（处理伤害效果和死亡检查）
        /// </summary>
        /// <param name="card">受到伤害的卡牌</param>
        /// <param name="damage">伤害值</param>
        /// <param name="owner">卡牌拥有者</param>
        public void DealDamageToCard(CardInstance card, int damage, Player owner)
        {
            if (card == null || owner == null || damage <= 0)
            {
                return;
            }

            // 触发受到伤害效果
            effectManager.TriggerOnTakeDamage(card, damage);

            // 造成伤害
            card.TakeDamage(damage);

            // 检查是否死亡
            if (card.isDead)
            {
                owner.SendToGraveyard(card);
            }
        }

        /// <summary>
        /// 对玩家造成伤害
        /// </summary>
        /// <param name="player">受到伤害的玩家</param>
        /// <param name="damage">伤害值</param>
        public void DealDamageToPlayer(Player player, int damage)
        {
            if (player == null || damage <= 0)
            {
                return;
            }

            player.TakeDamage(damage);
        }

        /// <summary>
        /// 检查并清理场上死亡的卡牌
        /// </summary>
        /// <param name="player">要检查的玩家</param>
        public void CheckAndCleanDeadCards(Player player)
        {
            if (player == null)
            {
                return;
            }

            // 创建副本以避免在迭代时修改集合
            var cardsToCheck = new List<CardInstance>(player.board);
            
            foreach (var card in cardsToCheck)
            {
                if (card.isDead)
                {
                    player.SendToGraveyard(card);
                }
            }
        }
    }
}
