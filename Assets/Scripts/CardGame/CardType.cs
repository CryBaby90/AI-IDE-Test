using System;

namespace CardGame
{
    /// <summary>
    /// 卡牌类型枚举
    /// </summary>
    public enum CardType
    {
        Minion,     // 随从
        Spell,      // 法术
        Equipment,  // 装备
        Hero        // 英雄
    }

    /// <summary>
    /// 卡牌稀有度枚举
    /// </summary>
    public enum CardRarity
    {
        Common,     // 普通
        Rare,       // 稀有
        Epic,       // 史诗
        Legendary   // 传说
    }
}
