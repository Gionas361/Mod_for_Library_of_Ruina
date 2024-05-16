using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/////////////////////////////// Dice Passives ///////////////////////////////

public class DiceCardAbility_Infinita_Recycle : DiceCardAbilityBase
{
    public static string Desc = "[On Clash Win] Recycle this Dice (Up to 5 times) and draw a card.";

    private int count;
    public override void OnSucceedAttack(BattleUnitModel target)
    {
        count++;

        if (count <= 5)
        {
            ActivateBonusAttackDice();
        }
        else
        {
            count = 0;
        }
    }

    public override void OnSucceedAttack()
    {
        owner.allyCardDetail.DrawCards(1);
    }
}

public class DiceCardAbility_2powerPerHand : DiceCardAbilityBase
{
    public static string Desc = "[On Use] Gain +2 Power per card in Hand.";

    public override void BeforeRollDice()
    {
        int count = base.owner.allyCardDetail.GetHand().Count;
        if (count > 0)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = count * 2
            });
        }
    }
}

public class DiceCardAbility_DebuffGalore : DiceCardAbilityBase
{
    public static string Desc = "[On Clash Win] Recycle this Dice (Up to 5 times) and inflict either Weak, Disarm or Binding.";

    private int count;
    public int debuffSelect;

    public override void OnSucceedAttack(BattleUnitModel target)
    {
        count++;

        if (count <= 5)
        {
            ActivateBonusAttackDice();
        }
        else
        {
            count = 0;
        }

        debuffSelect = RandomUtil.Range(0, 2);

        if (debuffSelect == 0)
        {
            card.target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Weak, 1, owner);
        }
        else if (debuffSelect == 1)
        {
            card.target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Disarm, 1, owner);
        }
        else if (debuffSelect == 2)
        {
            card.target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Binding, 1, owner);
        }
    }
}

public class DiceCardAbility_Draw1 : DiceCardAbilityBase
{
    public static string Desc = "[On Hit] Draw a card.";

    public override void OnSucceedAttack()
    {
        owner.allyCardDetail.DrawCards(1);
    }
}

public class DiceCardAbility_MDefend : DiceCardAbilityBase
{
    public static string Desc = "[On Clash Lose] Gain 4 Power for this and next Scene.";

    public override void OnLoseParrying()
    {
        owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.AllPowerUp, 4, base.owner);
    }
}

public class DiceCardAbility_MAttack : DiceCardAbilityBase
{
    public static string Desc = "[On Hit] Deal 10 stagger and Inflict 3 burn.";

    public override void OnSucceedAttack()
    {
        BattleUnitModel target = base.card.target;
        target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, 3, base.owner);
        target.TakeBreakDamage(10);
    }
}

public class DiceCardAbility_StarFall_Recycle : DiceCardAbilityBase
{
    public static string Desc = "[On Hit] Inflict 2 Burn.";

    public override void OnSucceedAttack(BattleUnitModel target)
    {
        target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, 2, base.owner);
    }
}


/////////////////////////////// Page Passives ///////////////////////////////

public class DiceCardSelfAbility_superCharged : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Battle Start] Discard a card, draw a card and recover 1 Light.";

    public override void OnStartBattle()
    {
        owner.allyCardDetail.DiscardACardLowest();
        owner.allyCardDetail.DrawCards(1);
        owner.cardSlotDetail.RecoverPlayPoint(1);
    }
}

public class DiceCardSelfAbility_Discard2Lower : DiceCardSelfAbilityBase
{
    public static string Desc = "For each Objet d'art in hand lower the Cost by 1. [On Use] It will discard the 2 lowest costing cards. Half the DMG of the dice. [On Hit] Heal 1 HP. Exhausts at end of the Scene and recover 0-2 Light.";

    //Discard 2 lowest.
    public override void OnUseCard()
    {
        owner.allyCardDetail.DiscardACardLowest();
        owner.allyCardDetail.DiscardACardLowest();
    }

    //1 Cost down per copy
    public class BattleDiceCardBuf_costDownCard : BattleDiceCardBuf
    {
        private int _count;

        public override DiceCardBufType bufType => DiceCardBufType.CostDownR;

        public BattleDiceCardBuf_costDownCard(int count)
        {
            _count = count;
        }

        public override int GetCost(int oldCost)
        {
            return oldCost - _count;
        }
    }

    public override void OnEnterCardPhase(BattleUnitModel unit, BattleDiceCardModel self)
    {
        List<BattleDiceCardModel> list = unit.allyCardDetail.GetHand().FindAll(a => a.GetOriginCost() > 3);
        self.AddBufWithoutDuplication(new BattleDiceCardBuf_costDownCard(list.Count));
    }

    //Half Damage
    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        behavior.ApplyDiceStatBonus(new DiceStatBonus
        {
            dmgRate = -50,
            breakRate = 50
        });
    }

    //1 HP on hit
    public override void OnSucceedAttack()
    {
        base.owner.RecoverHP(1);
    }

    //Exhaust on end of Scene
    public override void OnRoundEnd(BattleUnitModel unit, BattleDiceCardModel self)
    {
        unit.allyCardDetail.ExhaustACardAnywhere(self);
        owner.cardSlotDetail.RecoverPlayPoint(RandomUtil.Range(0, 2));
    }
}

public class DiceCardSelfAbility_CostDownPerObjetdArt : DiceCardSelfAbilityBase
{
    public static string Desc = "[Start of Clash] Reduce Power of all target's dice by 4. For each Objet d'art in hand lower the Cost by 1.";

    public override void OnStartParrying()
    {
        card.target?.currentDiceAction?.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
        {
            power = -4
        });
    }

    //1 Cost down per copy
    public class BattleDiceCardBuf_costDownCard : BattleDiceCardBuf
    {
        private int _count;

        public override DiceCardBufType bufType => DiceCardBufType.CostDownR;

        public BattleDiceCardBuf_costDownCard(int count)
        {
            _count = count;
        }

        public override int GetCost(int oldCost)
        {
            return oldCost - _count;
        }
    }

    public override void OnEnterCardPhase(BattleUnitModel unit, BattleDiceCardModel self)
    {
        List<BattleDiceCardModel> list = unit.allyCardDetail.GetHand().FindAll(a => a.GetOriginCost() > 3);
        self.AddBufWithoutDuplication(new BattleDiceCardBuf_costDownCard(list.Count));
    }
}

public class DiceCardSelfAbility_Discard3_Energy5 : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] It will discard the 3 lowest costing cards and regenerate 5 energy.";

    public override void OnUseCard()
    {
        owner.allyCardDetail.DiscardACardLowest();
        owner.allyCardDetail.DiscardACardLowest();
        owner.allyCardDetail.DiscardACardLowest();

        owner.cardSlotDetail.RecoverPlayPoint(5);
    }
}

public class DiceCardSelfAbility_Triple_Burn : DiceCardSelfAbilityBase
{
    public int Hit_Count = 0;
    public int BurnMount = 3;

    public static String Desc = "[On Hit] Inflict 3 Burn x Times Hit.";
    public override void OnSucceedAttack()
    {
        Hit_Count += 1;
        BurnMount = 3 * Hit_Count;


        BattleUnitModel target = base.card.target;
        if (target != null)
        {
            target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Burn, BurnMount, base.owner);
        }
    }

    public override void OnRoundEnd(BattleUnitModel unit, BattleDiceCardModel self)
    {
        Hit_Count = 0;
    }
}

public class DiceCardSelfAbility_OnDiscard1Energy : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Discard] The Card is Exhausted and the Librarian will recover 1 light and receive a stack of Burn.";

    public override void OnDiscard(BattleUnitModel unit, BattleDiceCardModel self)
    {
        owner.cardSlotDetail.RecoverPlayPoint(1);
        owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Burn, 1, base.owner);
        unit.allyCardDetail.ExhaustACardAnywhere(self);
    }
}

public class DiceCardSelfAbility_ThreeLittleStars : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] Add 2 \"Little Stars\" to your hand.";

    public override void OnUseCard()
    {
        base.owner.allyCardDetail.AddNewCard(new LorId("MyTopStar", 9));
        base.owner.allyCardDetail.AddNewCard(new LorId("MyTopStar", 9));
    }
}
