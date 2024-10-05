using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



///<summary>
/// Notes for other character attack animations:
///    - AwlofNight = HYM
///    - Mirae = DEMI
///    - Argalia = ZERK
///</summary>


/////////////////////////////// Dice Passives ///////////////////////////////

public class DiceCardAbility_Get3EmotionCoins : DiceCardAbilityBase
{
    public static string Desc = "[On Dice Roll] Gain 3 Emotion Coins.";

    public override void OnRollDice()
    {
        int count = 3;
        int count2 = base.owner.emotionDetail.CreateEmotionCoin(EmotionCoinType.Positive, count);
        base.owner.battleCardResultLog?.AddEmotionCoin(EmotionCoinType.Positive, count2);
    }
}

public class DiceCardAbility_Minus1Light : DiceCardAbilityBase
{
    public static string Desc = "[On Clash] Lose 1 Light.";

    public override void OnRollDice()
    {
        int count = base.owner.cardSlotDetail.LosePlayPoint(1);
    }
}

/////////////////////////////// Dice Passives ///////////////////////////////



/////////////////////////////// Page Passives ///////////////////////////////

public class DiceCardSelfAbility_DemiRetry : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] Immobalize the user and fully heal HP and Stagger.";

    public override void OnUseCard()
    {
        base.owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Stun, 1, base.owner);
        base.owner.RecoverHP(999);
        base.owner.breakDetail.RecoverBreak(999);
    }
}

public class DiceCardSelfAbility_DiscardHandPowerUP : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] Discard hand and all dice in page gain +4 Power.";

    public override void OnUseCard()
    {
        base.owner.allyCardDetail.DiscardACardByAbility(base.owner.allyCardDetail.GetHand());
        card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
        {
            power = 4
        });
    }
}

public class DiceCardSelfAbility_DiscardAllEnergy : DiceCardSelfAbilityBase
{
    public static string Desc = "[When Discarded] All other allies restore 1 Light.";

    public override void OnDiscard(BattleUnitModel unit, BattleDiceCardModel self)
    {
        List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList(unit.faction);
        aliveList.Remove(unit);
        foreach (BattleUnitModel item in aliveList)
        {
            item.cardSlotDetail.RecoverPlayPoint(1);
        }
    }
}

/////////////////////////////// Page Passives ///////////////////////////////



///////////////////////////// Key Page Passives /////////////////////////////

public class PassiveAbility_DemidrawRegenUnique : PassiveAbilityBase
{
    //Recover 4 energy +1 every 2 emotion levels, if at [End of Turn] all Light was used.
    //And draw 2 cards if hand is Empty at [End of Turn] +1 every 2 emotion levels.

    public override void OnRoundEnd()
    {
        int emotionLevel = base.owner.emotionDetail.EmotionLevel;
        int extra = emotionLevel / 2;

        if (extra < 3)
        {
            extra++;
        }

        int lightrecovered = 4 + extra;
        int pagesdrawn = 2 + extra;

        if (owner.cardSlotDetail.PlayPoint == 0)
        {
            owner.cardSlotDetail.RecoverPlayPoint(lightrecovered);
        }

        if (base.owner.allyCardDetail.GetHand().Count == 0)
        {
            owner.allyCardDetail.DrawCards(pagesdrawn);
        }
    }
}

public class PassiveAbility_OnesidedPowerUp : PassiveAbilityBase
{
    //If hit 3 times or more by One Sided attacks, gain 3 Power next Scene.

    private bool _trigger;
    private int _timeshit = 0;

    public override void OnRoundEnd()
    {
        if (_timeshit >= 2)
        {
            _trigger = true;
        }

        if (_trigger)
        {
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.AllPowerUp, 3, owner);
        }

        _trigger = false;
        _timeshit = 0;
    }

    public override void OnStartTargetedOneSide(BattlePlayingCardDataInUnitModel attackerCard)
    {
        base.OnStartTargetedOneSide(attackerCard);
        _timeshit++;
    }

    public override void OnEndOneSideVictim(BattlePlayingCardDataInUnitModel attackerCard)
    {
        base.OnEndOneSideVictim(attackerCard);
        _timeshit++;
    }
}

public class PassiveAbility_EmotionalDiceAdder : PassiveAbilityBase
{
    public override int SpeedDiceNumAdder()
    {
        BattleUnitModel battleUnitModel = owner;

        int emotionLevel = base.owner.emotionDetail.EmotionLevel;

        return (emotionLevel);
    }
}

///////////////////////////// Key Page Passives /////////////////////////////
