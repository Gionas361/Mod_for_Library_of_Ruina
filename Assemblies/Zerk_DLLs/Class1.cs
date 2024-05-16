using LOR_DiceSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


/////////////////////////////// Dice Passives ///////////////////////////////

public class DiceCardAbility_DragonRoarStagger : DiceCardAbilityBase
{
    public static string Desc = "[On Clash Win] Stagger the opponent.";

    public override void OnSucceedAttack()
    {
        BattleUnitModel battleUnitModel = base.card?.target;
        if (battleUnitModel != null)
        {
            int num = battleUnitModel.breakDetail.breakGauge;
            if (num < 1)
            {
                num = 1;
            }
            battleUnitModel.TakeBreakDamage(num, DamageType.Card_Ability, base.owner, AtkResist.None);
        }
    }
}

public class DiceCardAbility_BurnBothBy7 : DiceCardAbilityBase
{
    public static string Desc = "[On Clash Win] Inflict 7 stacks of Burn on opponent and the Librarian.";

    public override void OnWinParrying()
    {
        base.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, 7, base.owner);
        card.target?.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, 7, base.owner);
    }
}

public class DiceCardAbility_BurnBoth2n1 : DiceCardAbilityBase
{
    public static string Desc = "[On Clash Win] Inflict 2 stacks of Burn on opponent and 1 stack to the Librarian.";

    public override void OnWinParrying()
    {
        base.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, 1, base.owner);
        card.target?.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, 2, base.owner);
    }
}

/////////////////////////////// Dice Passives ///////////////////////////////



/////////////////////////////// Page Passives ///////////////////////////////

public class DiceCardSelfAbility_CashoutEngineSkill : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] Restore light equal to the amount of pages used since last time this card was used.";
}

public class DiceCardSelfAbility_HpForLight : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] Lose 10% of your HP and recover 4 Light.";

    public override void OnUseCard()
    {
        int num = base.owner.MaxHp / 10;

        base.owner.TakeDamage(num, DamageType.Passive, base.owner);
        base.owner.cardSlotDetail.RecoverPlayPoint(4);
    }
}

public class DiceCardSelfAbility_RecoverStaggerToDiceValue : DiceCardSelfAbilityBase
{
    public static string Desc = "Recover Stagger amount equal to the value of all the dice in this page.";

    public override void OnSucceedAttack(BattleDiceBehavior behavior)
    {
        BattleUnitModel battleUnitModel = base.card?.target;
        if (battleUnitModel != null && base.owner != null && behavior != null)
        {
            int diceResultValue = behavior.DiceResultValue;
            base.owner.breakDetail.RecoverBreak(diceResultValue);
        }
    }
}

public class DiceCardSelfAbility_ExpendableEngine : DiceCardSelfAbilityBase
{
    public static string Desc = "Dice in this page will gain 3 Power for each \"Mana Stack\".";

    public override void OnStartParrying()
    {
        card?.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
        {
            power = 3
        });
    }
}

/////////////////////////////// Page Passives ///////////////////////////////



///////////////////////////// Key Page Passives /////////////////////////////

public class PassiveAbility_ZerkEveryUnique : PassiveAbilityBase
{
    //For each card used since last time "Cashout Engine" was used increase a invisible count.
    //Once the card "Cashout Engine" is used restore light equal to the count.

    //Discard hand at end of Scene and draw 5 cards.

    public int CardsUsed;

    public override void OnUnitCreated()
    {
        CardsUsed = 0;

        if (owner.faction == Faction.Player)
        {
            owner.personalEgoDetail.AddCard(new LorId("MyTopStar", 40));
        }
    }

    public override void OnRoundStart()
    {
        base.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, CardsUsed, base.owner);
    }

    public override void OnRoundEnd()
    {
        owner.allyCardDetail.DiscardACardByAbility(base.owner.allyCardDetail.GetHand());
        owner.allyCardDetail.DrawCards(5);

        base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_burn).Destroy();
    }

    public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
    {
        CardsUsed++;

        if (curCard.card.GetID() == new LorId("MyTopStar", 33))
        {
            base.owner.cardSlotDetail.RecoverPlayPoint(CardsUsed);
            
            CardsUsed = 0;
        }

        if (curCard.card.GetID() == new LorId("MyTopStar", 40))
        {
            owner.currentDiceAction.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 3 * CardsUsed
            });
        }
    }
}

public class PassiveAbility_ZerkPassiveHeal : PassiveAbilityBase
{
    public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
    {
        if (curCard.GetOriginalDiceBehaviorList().FindAll((DiceBehaviour x) => x.Type == BehaviourType.Def).Count != 0)
        {
            int healmount = RandomUtil.Range(1, 8) * curCard.GetOriginalDiceBehaviorList().FindAll((DiceBehaviour x) => x.Type == BehaviourType.Def).Count;

            owner.battleCardResultLog?.SetPassiveAbility(this);
            base.owner.RecoverHP(healmount);
        }
    }
}

///////////////////////////// Key Page Passives /////////////////////////////
