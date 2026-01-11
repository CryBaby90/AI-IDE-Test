using UnityEngine;

namespace CardGame
{
    /// <summary>
    /// 卡牌基础数据类（ScriptableObject）
    /// 用于在编辑器中配置卡牌数据
    /// </summary>
    [CreateAssetMenu(fileName = "New Card", menuName = "Card Game/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("基础信息")]
        [Tooltip("卡牌唯一ID")]
        public string cardID;

        [Tooltip("卡牌名称")]
        public string cardName;

        [Tooltip("卡牌描述")]
        [TextArea(3, 5)]
        public string description;

        [Header("卡牌属性")]
        [Tooltip("卡牌类型")]
        public CardType cardType;

        [Tooltip("卡牌稀有度")]
        public CardRarity rarity;

        [Tooltip("卡牌费用")]
        [Range(0, 20)]
        public int cost;

        [Header("随从属性（仅随从卡牌有效）")]
        [Tooltip("攻击力")]
        [Range(0, 20)]
        public int attack;

        [Tooltip("生命值")]
        [Range(1, 30)]
        public int health;

        [Header("其他")]
        [Tooltip("卡牌图标")]
        public Sprite cardIcon;

        [Tooltip("卡牌艺术图")]
        public Sprite cardArt;

        /// <summary>
        /// 验证卡牌数据是否有效
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(cardID) || string.IsNullOrEmpty(cardName))
            {
                return false;
            }

            if (cardType == CardType.Minion && (attack < 0 || health < 1))
            {
                return false;
            }

            return true;
        }
    }
}
