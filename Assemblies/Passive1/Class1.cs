using ExtendedLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class PassiveAbility_TwoCardsPerScene : PassiveAbilityBase
{
    public override void OnRoundEnd()
    {
        owner.allyCardDetail.DrawCards(2);
    }
}

public class PassiveAbility_Add1Golden : PassiveAbilityBase
{
    public int _roundcount = 0;
    public override void OnRoundEnd()
    {
        _roundcount += 1;
        if (_roundcount == 2)
        {
            base.owner.allyCardDetail.AddNewCard(new LorId("MyTopStar", 4));
        }
        else if (_roundcount == 4)
        {
            base.owner.allyCardDetail.AddNewCard(new LorId("MyTopStar", 3));
        }
        else if (_roundcount == 6)
        {
            base.owner.allyCardDetail.AddNewCard(new LorId("MyTopStar", 3));
            base.owner.allyCardDetail.AddNewCard(new LorId("MyTopStar", 4));
            _roundcount = 1;
        }
    }
}

public class PassiveAbility_EgoAbility : PassiveAbilityBase
{
    public override void OnUnitCreated()
    {
        if (owner.faction == Faction.Player)
        {
            owner.personalEgoDetail.AddCard(new LorId("MyTopStar", 28));
            owner.personalEgoDetail.AddCard(new LorId("MyTopStar", 29));
            owner.personalEgoDetail.AddCard(new LorId("MyTopStar", 30));
        }
    }
}
