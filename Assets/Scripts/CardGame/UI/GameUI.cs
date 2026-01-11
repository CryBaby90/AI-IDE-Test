using UnityEngine;
using UnityEngine.UI;
#if UNITY_TEXTMESHPRO
using TMPro;
#endif
using CardGame.Events;

namespace CardGame.UI
{
    /// <summary>
    /// 游戏主UI
    /// 显示玩家信息、回合信息、按钮控制
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [Header("玩家信息UI")]
#if UNITY_TEXTMESHPRO
        public TextMeshProUGUI player1NameText;
        public TextMeshProUGUI player1HealthText;
        public TextMeshProUGUI player1ManaText;
        public TextMeshProUGUI player1DeckCountText;

        public TextMeshProUGUI player2NameText;
        public TextMeshProUGUI player2HealthText;
        public TextMeshProUGUI player2ManaText;
        public TextMeshProUGUI player2DeckCountText;

        [Header("回合信息UI")]
        public TextMeshProUGUI turnNumberText;
        public TextMeshProUGUI currentPlayerText;
        public TextMeshProUGUI phaseText;
#else
        public Text player1NameText;
        public Text player1HealthText;
        public Text player1ManaText;
        public Text player1DeckCountText;

        public Text player2NameText;
        public Text player2HealthText;
        public Text player2ManaText;
        public Text player2DeckCountText;

        [Header("回合信息UI")]
        public Text turnNumberText;
        public Text currentPlayerText;
        public Text phaseText;
#endif

        [Header("控制按钮")]
        public Button endTurnButton;
        public Button pauseButton;
        public Button restartButton;

        [Header("游戏状态UI")]
        public GameObject gameOverPanel;
#if UNITY_TEXTMESHPRO
        public TextMeshProUGUI winnerText;
#else
        public Text winnerText;
#endif

        // 管理器引用
        private CardGameManager gameManager;
        private TurnManager turnManager;
        private GameStateManager stateManager;

        void Start()
        {
            // 获取管理器
            gameManager = CardGameManager.Instance;
            if (gameManager != null)
            {
                turnManager = gameManager.GetTurnManager();
            }
            stateManager = GameStateManager.Instance;

            // 绑定按钮事件
            if (endTurnButton != null)
                endTurnButton.onClick.AddListener(OnEndTurnClicked);

            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseClicked);

            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);

            // 订阅事件
            CardGameEvents.OnTurnStart += OnTurnStart;
            CardGameEvents.OnTurnEnd += OnTurnEnd;
            CardGameEvents.OnPhaseChange += OnPhaseChange;
            CardGameEvents.OnPlayerDamage += OnPlayerDamage;
            CardGameEvents.OnPlayerHeal += OnPlayerHeal;
            CardGameEvents.OnGameEnd += OnGameEnd;
            CardGameEvents.OnGameStateChanged += OnGameStateChanged;

            // 初始化UI
            UpdateAllUI();
        }

        void Update()
        {
            // 定期更新UI（可以根据需要调整更新频率）
            if (Time.frameCount % 30 == 0) // 每30帧更新一次
            {
                UpdateAllUI();
            }
        }

        /// <summary>
        /// 更新所有UI
        /// </summary>
        private void UpdateAllUI()
        {
            UpdatePlayerUI();
            UpdateTurnUI();
        }

        /// <summary>
        /// 更新玩家信息UI
        /// </summary>
        private void UpdatePlayerUI()
        {
            if (gameManager == null)
                return;

            Player player1 = gameManager.GetPlayer1();
            Player player2 = gameManager.GetPlayer2();

            // 更新玩家1信息
            if (player1 != null)
            {
                if (player1NameText != null)
                    player1NameText.text = player1.playerName;

                if (player1HealthText != null)
                    player1HealthText.text = $"生命值: {player1.currentHealth}/{player1.maxHealth}";

                if (player1ManaText != null)
                    player1ManaText.text = $"法力: {player1.currentMana}/{player1.manaCrystals}";

                if (player1DeckCountText != null)
                    player1DeckCountText.text = $"牌组: {player1.deckManager?.RemainingCards ?? 0}";
            }

            // 更新玩家2信息
            if (player2 != null)
            {
                if (player2NameText != null)
                    player2NameText.text = player2.playerName;

                if (player2HealthText != null)
                    player2HealthText.text = $"生命值: {player2.currentHealth}/{player2.maxHealth}";

                if (player2ManaText != null)
                    player2ManaText.text = $"法力: {player2.currentMana}/{player2.manaCrystals}";

                if (player2DeckCountText != null)
                    player2DeckCountText.text = $"牌组: {player2.deckManager?.RemainingCards ?? 0}";
            }
        }

        /// <summary>
        /// 更新回合信息UI
        /// </summary>
        private void UpdateTurnUI()
        {
            if (turnManager == null)
                return;

            if (turnNumberText != null)
                turnNumberText.text = $"回合: {turnManager.CurrentTurn}";

            Player currentPlayer = turnManager.GetCurrentPlayer();
            if (currentPlayerText != null && currentPlayer != null)
                currentPlayerText.text = $"当前玩家: {currentPlayer.playerName}";

            if (phaseText != null)
                phaseText.text = $"阶段: {turnManager.CurrentPhase}";
        }

        /// <summary>
        /// 结束回合按钮点击
        /// </summary>
        private void OnEndTurnClicked()
        {
            if (turnManager != null && stateManager != null && stateManager.CanPerformAction())
            {
                turnManager.EndTurn();
            }
        }

        /// <summary>
        /// 暂停按钮点击
        /// </summary>
        private void OnPauseClicked()
        {
            if (stateManager != null)
            {
                if (stateManager.CurrentState == GameState.Paused)
                {
                    stateManager.ResumeGame();
                }
                else
                {
                    stateManager.PauseGame();
                }
            }
        }

        /// <summary>
        /// 重启按钮点击
        /// </summary>
        private void OnRestartClicked()
        {
            if (gameManager != null)
            {
                gameManager.RestartGame();
            }
        }

        /// <summary>
        /// 回合开始事件回调
        /// </summary>
        private void OnTurnStart(int turnNumber)
        {
            UpdateTurnUI();
        }

        /// <summary>
        /// 回合结束事件回调
        /// </summary>
        private void OnTurnEnd(int turnNumber)
        {
            UpdateTurnUI();
        }

        /// <summary>
        /// 阶段变更事件回调
        /// </summary>
        private void OnPhaseChange(TurnPhase phase)
        {
            UpdateTurnUI();
        }

        /// <summary>
        /// 玩家受到伤害事件回调
        /// </summary>
        private void OnPlayerDamage(Player player)
        {
            UpdatePlayerUI();
        }

        /// <summary>
        /// 玩家恢复生命值事件回调
        /// </summary>
        private void OnPlayerHeal(Player player)
        {
            UpdatePlayerUI();
        }

        /// <summary>
        /// 游戏结束事件回调
        /// </summary>
        private void OnGameEnd(Player winner)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            if (winnerText != null && winner != null)
            {
                winnerText.text = $"获胜者: {winner.playerName}";
            }
        }

        /// <summary>
        /// 游戏状态变更事件回调
        /// </summary>
        private void OnGameStateChanged(GameState newState)
        {
            // 根据游戏状态更新UI
            if (endTurnButton != null)
            {
                endTurnButton.interactable = (newState == GameState.Playing);
            }
        }

        void OnDestroy()
        {
            // 取消订阅事件
            CardGameEvents.OnTurnStart -= OnTurnStart;
            CardGameEvents.OnTurnEnd -= OnTurnEnd;
            CardGameEvents.OnPhaseChange -= OnPhaseChange;
            CardGameEvents.OnPlayerDamage -= OnPlayerDamage;
            CardGameEvents.OnPlayerHeal -= OnPlayerHeal;
            CardGameEvents.OnGameEnd -= OnGameEnd;
            CardGameEvents.OnGameStateChanged -= OnGameStateChanged;
        }
    }
}
