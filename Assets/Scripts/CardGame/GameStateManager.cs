using UnityEngine;
using System;

namespace CardGame
{
    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        None,           // 无状态
        Initializing,   // 初始化中
        Preparing,      // 准备阶段
        Playing,        // 游戏中
        Paused,         // 暂停
        GameOver        // 游戏结束
    }

    /// <summary>
    /// 游戏状态管理器（单例）
    /// 管理游戏状态机和状态转换
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        private GameState currentState = GameState.None;
        private GameState previousState = GameState.None;

        // 状态变更事件
        public event Action<GameState, GameState> OnStateChanged;

        private static GameStateManager instance;
        public static GameStateManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameStateManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GameStateManager");
                        instance = go.AddComponent<GameStateManager>();
                    }
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 当前游戏状态
        /// </summary>
        public GameState CurrentState => currentState;

        /// <summary>
        /// 上一个游戏状态
        /// </summary>
        public GameState PreviousState => previousState;

        /// <summary>
        /// 改变游戏状态
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (currentState == newState)
            {
                return;
            }

            // 退出当前状态
            ExitState(currentState);

            // 保存上一个状态
            previousState = currentState;

            // 进入新状态
            currentState = newState;
            EnterState(newState);

            // 触发状态变更事件
            OnStateChanged?.Invoke(previousState, currentState);
            Debug.Log($"GameStateManager: 状态变更 {previousState} -> {currentState}");
        }

        /// <summary>
        /// 进入状态
        /// </summary>
        private void EnterState(GameState state)
        {
            switch (state)
            {
                case GameState.Initializing:
                    Debug.Log("GameStateManager: 进入初始化状态");
                    break;
                case GameState.Preparing:
                    Debug.Log("GameStateManager: 进入准备阶段");
                    break;
                case GameState.Playing:
                    Debug.Log("GameStateManager: 进入游戏状态");
                    break;
                case GameState.Paused:
                    Debug.Log("GameStateManager: 游戏暂停");
                    Time.timeScale = 0f;
                    break;
                case GameState.GameOver:
                    Debug.Log("GameStateManager: 游戏结束");
                    break;
            }
        }

        /// <summary>
        /// 退出状态
        /// </summary>
        private void ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.Paused:
                    Time.timeScale = 1f;
                    break;
            }
        }

        /// <summary>
        /// 检查是否处于指定状态
        /// </summary>
        public bool IsInState(GameState state)
        {
            return currentState == state;
        }

        /// <summary>
        /// 检查是否可以执行游戏操作
        /// </summary>
        public bool CanPerformAction()
        {
            return currentState == GameState.Playing;
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
        }

        /// <summary>
        /// 恢复游戏
        /// </summary>
        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }

        /// <summary>
        /// 重置游戏状态
        /// </summary>
        public void ResetState()
        {
            currentState = GameState.None;
            previousState = GameState.None;
        }
    }
}
