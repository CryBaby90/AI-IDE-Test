using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CardGame;

namespace CardGame.Effects
{
    /// <summary>
    /// 效果管理器
    /// 负责注册、执行和管理所有卡牌效果
    /// </summary>
    public class EffectManager : MonoBehaviour
    {
        // 效果注册表：卡牌实例 -> 效果列表
        private Dictionary<CardInstance, List<ICardEffect>> effectRegistry = new Dictionary<CardInstance, List<ICardEffect>>();

        // 全局效果列表（不绑定特定卡牌）
        private List<ICardEffect> globalEffects = new List<ICardEffect>();

        private static EffectManager instance;
        public static EffectManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<EffectManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("EffectManager");
                        instance = go.AddComponent<EffectManager>();
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
        /// 为卡牌注册效果
        /// </summary>
        public void RegisterEffect(CardInstance card, ICardEffect effect)
        {
            if (card == null || effect == null)
            {
                Debug.LogWarning("EffectManager: 尝试注册空卡牌或空效果！");
                return;
            }

            if (!effectRegistry.ContainsKey(card))
            {
                effectRegistry[card] = new List<ICardEffect>();
            }

            if (!effectRegistry[card].Contains(effect))
            {
                effectRegistry[card].Add(effect);
                // 按优先级排序
                effectRegistry[card] = effectRegistry[card].OrderByDescending(e => 
                {
                    if (e is CardEffectBase baseEffect)
                        return baseEffect.priority;
                    return 0;
                }).ToList();
            }
        }

        /// <summary>
        /// 移除卡牌的效果
        /// </summary>
        public void UnregisterEffect(CardInstance card, ICardEffect effect)
        {
            if (card == null || effect == null)
            {
                return;
            }

            if (effectRegistry.ContainsKey(card))
            {
                effectRegistry[card].Remove(effect);
                if (effectRegistry[card].Count == 0)
                {
                    effectRegistry.Remove(card);
                }
            }
        }

        /// <summary>
        /// 移除卡牌的所有效果
        /// </summary>
        public void UnregisterAllEffects(CardInstance card)
        {
            if (card != null && effectRegistry.ContainsKey(card))
            {
                effectRegistry.Remove(card);
            }
        }

        /// <summary>
        /// 注册全局效果
        /// </summary>
        public void RegisterGlobalEffect(ICardEffect effect)
        {
            if (effect != null && !globalEffects.Contains(effect))
            {
                globalEffects.Add(effect);
            }
        }

        /// <summary>
        /// 移除全局效果
        /// </summary>
        public void UnregisterGlobalEffect(ICardEffect effect)
        {
            globalEffects.Remove(effect);
        }

        /// <summary>
        /// 触发卡牌打出效果
        /// </summary>
        public void TriggerOnPlay(CardInstance card, GameObject target = null)
        {
            if (card == null) return;

            // 触发卡牌自身的效果
            if (effectRegistry.ContainsKey(card))
            {
                foreach (var effect in effectRegistry[card])
                {
                    effect.OnPlay(card, target);
                }
            }

            // 触发全局效果
            foreach (var effect in globalEffects)
            {
                effect.OnPlay(card, target);
            }
        }

        /// <summary>
        /// 触发卡牌入场效果
        /// </summary>
        public void TriggerOnEnter(CardInstance card)
        {
            if (card == null) return;

            if (effectRegistry.ContainsKey(card))
            {
                foreach (var effect in effectRegistry[card])
                {
                    effect.OnEnter(card);
                }
            }

            foreach (var effect in globalEffects)
            {
                effect.OnEnter(card);
            }
        }

        /// <summary>
        /// 触发卡牌死亡效果
        /// </summary>
        public void TriggerOnDeath(CardInstance card)
        {
            if (card == null) return;

            if (effectRegistry.ContainsKey(card))
            {
                foreach (var effect in effectRegistry[card])
                {
                    effect.OnDeath(card);
                }
            }

            foreach (var effect in globalEffects)
            {
                effect.OnDeath(card);
            }

            // 卡牌死亡后移除所有效果
            UnregisterAllEffects(card);
        }

        /// <summary>
        /// 触发回合开始效果
        /// </summary>
        public void TriggerOnTurnStart(CardInstance card)
        {
            if (card == null) return;

            if (effectRegistry.ContainsKey(card))
            {
                foreach (var effect in effectRegistry[card])
                {
                    effect.OnTurnStart(card);
                }
            }

            foreach (var effect in globalEffects)
            {
                effect.OnTurnStart(card);
            }
        }

        /// <summary>
        /// 触发回合结束效果
        /// </summary>
        public void TriggerOnTurnEnd(CardInstance card)
        {
            if (card == null) return;

            if (effectRegistry.ContainsKey(card))
            {
                foreach (var effect in effectRegistry[card])
                {
                    effect.OnTurnEnd(card);
                }
            }

            foreach (var effect in globalEffects)
            {
                effect.OnTurnEnd(card);
            }
        }

        /// <summary>
        /// 触发攻击效果
        /// </summary>
        public void TriggerOnAttack(CardInstance attacker, CardInstance target)
        {
            if (attacker == null) return;

            if (effectRegistry.ContainsKey(attacker))
            {
                foreach (var effect in effectRegistry[attacker])
                {
                    effect.OnAttack(attacker, target);
                }
            }

            foreach (var effect in globalEffects)
            {
                effect.OnAttack(attacker, target);
            }
        }

        /// <summary>
        /// 触发受到伤害效果
        /// </summary>
        public void TriggerOnTakeDamage(CardInstance card, int damage)
        {
            if (card == null) return;

            if (effectRegistry.ContainsKey(card))
            {
                foreach (var effect in effectRegistry[card])
                {
                    effect.OnTakeDamage(card, damage);
                }
            }

            foreach (var effect in globalEffects)
            {
                effect.OnTakeDamage(card, damage);
            }
        }

        /// <summary>
        /// 获取卡牌的所有效果
        /// </summary>
        public List<ICardEffect> GetCardEffects(CardInstance card)
        {
            if (card == null || !effectRegistry.ContainsKey(card))
            {
                return new List<ICardEffect>();
            }
            return new List<ICardEffect>(effectRegistry[card]);
        }

        /// <summary>
        /// 清空所有效果
        /// </summary>
        public void ClearAllEffects()
        {
            effectRegistry.Clear();
            globalEffects.Clear();
        }
    }
}
