using LOR_DiceSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


/////////////////////////////// Dice Passives ///////////////////////////////

public class DiceCardAbility_BonusFollowup : DiceCardAbilityBase
{
    public static string Desc = "[On Clash Win] Deal bonus Damage equal to dice roll.";

    public override void OnWinParrying()
    {
        BattleUnitModel battleUnitModel = base.card?.target;
        if (battleUnitModel != null && base.owner != null && behavior != null)
        {
            int diceResultValue = behavior.DiceResultValue;
            battleUnitModel.TakeDamage(diceResultValue, DamageType.Card_Ability, base.owner);
            battleUnitModel.TakeDamage(diceResultValue, DamageType.Card_Ability, base.owner);
        }
    }
}

/////////////////////////////// Dice Passives ///////////////////////////////



/////////////////////////////// Page Passives ///////////////////////////////

public class DiceCardSelfAbility_PweredArrow : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] If Speed is 1, all dice on this page gain +3 Power.";

    public override void OnUseCard()
    {
        if (card.speedDiceResultValue == 1)
        {
            card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 3
            });
        }
    }
}

public class DiceCardSelfAbility_1stSymphony : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] Give every Librarian 1 Strenght.";

    public override void OnUseCard()
    {
        List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(base.owner.faction);
        foreach (BattleUnitModel item in aliveList)
        {
            item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1, item);
        }
    }
}

public class DiceCardSelfAbility_2ndSymphony : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] Give every Librarian 1 Break Protection.";

    public override void OnUseCard()
    {
        List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(base.owner.faction);
        foreach (BattleUnitModel item in aliveList)
        {
            item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BreakProtection, 1, item);
        }
    }
}

public class DiceCardSelfAbility_3rdSymphony : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] Give every Librarian 1 Protection.";

    public override void OnUseCard()
    {
        List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(base.owner.faction);
        foreach (BattleUnitModel item in aliveList)
        {
            item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Protection, 1, item);
        }
    }
}

public class DiceCardSelfAbility_4thSymphony : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] Give every Librarian 1 Endurance.";

    public override void OnUseCard()
    {
        List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(base.owner.faction);
        foreach (BattleUnitModel item in aliveList)
        {
            item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 1, item);
        }
    }
}

public class DiceCardSelfAbility_5thSymphony : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] Give every Librarian 1 Power.";

    public override void OnUseCard()
    {
        List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(base.owner.faction);
        foreach (BattleUnitModel item in aliveList)
        {
            item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.AllPowerUp, 1, item);
        }
    }
}

public class DiceCardSelfAbility_WeaknessExploit : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] Offensive dice on this page change to the types the target is weakest to.";

    public override void OnUseCard()
    {
        if (card.target == null)
        {
            return;
        }
        List<BehaviourDetail> list = new List<BehaviourDetail>();
        int resistValue = GetResistValue(BehaviourDetail.Slash);
        int resistValue2 = GetResistValue(BehaviourDetail.Penetrate);
        int resistValue3 = GetResistValue(BehaviourDetail.Hit);
        int num = resistValue;
        if (resistValue2 > num)
        {
            num = resistValue2;
        }
        if (resistValue3 > num)
        {
            num = resistValue3;
        }
        if (num == resistValue)
        {
            list.Add(BehaviourDetail.Slash);
        }
        if (num == resistValue2)
        {
            list.Add(BehaviourDetail.Penetrate);
        }
        if (num == resistValue3)
        {
            list.Add(BehaviourDetail.Hit);
        }
        BehaviourDetail detail = RandomUtil.SelectOne(list);
        foreach (BattleDiceBehavior diceBehavior in card.GetDiceBehaviorList())
        {
            if (IsAttackDice(diceBehavior.behaviourInCard.Detail))
            {
                diceBehavior.behaviourInCard = diceBehavior.behaviourInCard.Copy();
                diceBehavior.behaviourInCard.Detail = detail;
            }
        }
    }

    private int GetResistValue(BehaviourDetail detail)
    {
        float resist = (0f + BookModel.GetResistRate(card.target.Book.GetResistHP(detail)) + BookModel.GetResistRate(card.target.Book.GetResistBP(detail))) * 10f;
        int lolol = (int)resist;

        return (lolol);
    }
}

/////////////////////////////// Page Passives ///////////////////////////////



///////////////////////////// Key Page Passives /////////////////////////////

public class PassiveAbility_ReyOrderCardUnique : PassiveAbilityBase
{
    //If no cards were used this Scene recover 3 Light.
    //Upon reaching and using the 5th Symphony, gives every Librarian 3 Power, Endurance and Haste, and restores 3 Light.
    //Plus it heals every Librarian by 9 HP.

    public int CardSequence;
    public int Card;

    public override void OnUnitCreated()
    {
        CardSequence = 0;
        //Card starts at 22 bcs thats the first of the 5 cards.
        Card = 22;
    }

    public override void OnRoundStart()
    {
        base.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, CardSequence, base.owner);
    }

    public override void OnRoundEnd()
    {
        if (owner.cardHistory.GetCurrentRoundCardList(Singleton<StageController>.Instance.RoundTurn).Count <= 0)
        {
            owner.cardSlotDetail.RecoverPlayPoint(3);
        }

        base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_burn).Destroy();
    }

    public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
    {
        if (curCard.card.GetID() == new LorId("MyTopStar", Card + CardSequence))
        {
            //Moving to the next card
            CardSequence++;

            if (curCard.card.GetID() == new LorId("MyTopStar", 26) && (Card + CardSequence == 27))
            {
                //Ressetting Variables and giving buffs.
                CardSequence = 0;
                Card = 22;

                List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(base.owner.faction);
                foreach (BattleUnitModel item in aliveList)
                {
                    item.cardSlotDetail.RecoverPlayPoint(3);
                    item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 3, item);
                    item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 3, item);
                    item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, 3, item);
                }
            }
        }
    }

/*
        if (CardSequence == 0)
        {
            if (curCard.card.GetID() == new LorId("MyTopStar", 22))
            {
                CardSequence = 1;
            }
        }
        else if (CardSequence == 1)
        {
            if (curCard.card.GetID() == new LorId("MyTopStar", 23))
            {
                CardSequence = 2;
            }
        }
        else if (CardSequence == 2)
        {
            if (curCard.card.GetID() == new LorId("MyTopStar", 24))
            {
                CardSequence = 3;
            }
        }
        else if (CardSequence == 3)
        {
            if (curCard.card.GetID() == new LorId("MyTopStar", 25))
            {
                CardSequence = 4;
            }
        }
        else if (CardSequence == 4)
        {
            if (curCard.card.GetID() == new LorId("MyTopStar", 26))
            {
                CardSequence = 0;

                List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(base.owner.faction);
                foreach (BattleUnitModel item in aliveList)
                {
                    item.cardSlotDetail.RecoverPlayPoint(3);
                    item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 3, item);
                    item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 3, item);
                    item.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, 3, item);
                }
            }
        }

        base.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, CardSequence, base.owner);
    }
*/
}

///////////////////////////// Key Page Passives /////////////////////////////
