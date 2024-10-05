using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Battle.DiceAttackEffect;
using HarmonyLib;
using LOR_DiceSystem;
using LOR_XML;
using Mod;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

using ExtendedLoader;


/// <summary>
/// Attack Names:
/// - Blood Barrage			- blood consumption, blood 0.5x				- [ART DONE]
/// - Decapitation			- blood 2.0x								- [ART DONE]
/// - Blood Wall			- blood consumption							- 
/// - Bloody Enhancement	- self hurt, 1-3 strenght, blood 1.5x		- 
/// - Swipe					- blood 1x, SP heal							- [ART DONE]
/// - Screech				- Break enemy bar, mass attack				- 
/// - Blood Sac Burst		- Light Regen, weakness, blood consumption	- 
/// - Vampire Bite			- Break enemy bar, heal, blood 3.0x			- [ART DONE]
/// - Slumber				- Heal, blood consumption, stagger damage	- [ART DONE]
/// - Claw Fang				- blood 1x, HP heal							- [ART DONE]
/// - VampiresRampage		- Attack 9 times like a cinematic			- 
/// </summary>


///////////////////////////////// Initializer ///////////////////////////////
/*
public class IrahInitializer : ModInitializer
{
	public override void OnInitializeMod()
	{
		base.OnInitializeMod();
		Harmony harmony = new Harmony("LOR.MyTopStar.MOD");
		IrahInitializer.path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
		IrahInitializer.GetArtWorks(new DirectoryInfo(IrahInitializer.path + "/ArtWork"));

	}

	public static void GetArtWorks(DirectoryInfo dir)
	{
		if (dir.GetDirectories().Length != 0)
		{
			DirectoryInfo[] directories = dir.GetDirectories();
			for (int i = 0; i < directories.Length; i++)
			{
				IrahInitializer.GetArtWorks(directories[i]);
			}
		}
		foreach (FileInfo fileInfo in dir.GetFiles())
		{
			Texture2D texture2D = new Texture2D(2, 2);
			ImageConversion.LoadImage(texture2D, File.ReadAllBytes(fileInfo.FullName));
			Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f));
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
			IrahInitializer.ArtWorks[fileNameWithoutExtension] = value;
		}
	}

	public static string path;

	public static Dictionary<string, Sprite> ArtWorks = new Dictionary<string, Sprite>();
}
*/

///////////////////////////////// Initializer ///////////////////////////////



/////////////////////////////// Buffs / Effects /////////////////////////////

public class BattleUnitBuf_BloodOfIrah : BattleUnitBuf
{
	protected override string keywordId => "BloodOfIrah";

	protected override string keywordIconId => "BloodOfIrah";

	public BattleUnitBuf_BloodOfIrah(BattleUnitModel model)
	{
		this._owner = model;
		/*
		typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, IrahInitializer.ArtWorks["Blood"]);
		typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
		*/
		this.stack = 0;
	}

    public void Add(int add)
	{
		this.stack += add;
	}

	public static void AddBuf(BattleUnitModel unit, int stack)
	{
		BattleUnitBuf_BloodOfIrah battleUnitBuf_BloodOfIrah = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BloodOfIrah) as BattleUnitBuf_BloodOfIrah;
		if (battleUnitBuf_BloodOfIrah == null)
		{
			BattleUnitBuf_BloodOfIrah battleUnitBuf_BloodOfIrah2 = new BattleUnitBuf_BloodOfIrah(unit);
			battleUnitBuf_BloodOfIrah2.Add(stack);
			unit.bufListDetail.AddBuf(battleUnitBuf_BloodOfIrah2);
		}
		else
		{
			battleUnitBuf_BloodOfIrah.Add(stack);
		}
	}

	public static void SubBuf(BattleUnitModel unit, int percentage, int div0sub1)
	{
		BattleUnitBuf_BloodOfIrah bloodofIrah = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BloodOfIrah) as BattleUnitBuf_BloodOfIrah;
		if (div0sub1 == 0)
		{
			bloodofIrah.PercentageDeleto(unit, percentage);
		}
		else if (div0sub1 == 1)
		{
			bloodofIrah.IntegerDeleto(unit, percentage);
		}
	}

	public void IntegerDeleto(BattleUnitModel unit, int amount)
	{
		this.stack -= amount;

		if (this.stack < 0)
        {
			this.stack = 0;
        }
	}

	public void PercentageDeleto(BattleUnitModel unit, int percentage)
	{
		int value = this.stack;

		double percentasdouble = (double)percentage / 100;
		this.stack -= value * (int)percentasdouble;
	}
}

public class BattleUnitBuf_BloodOfIrahUniqueBuff : BattleUnitBuf
{
	protected override string keywordId => "BloodOfIrahUniqueBuff";

	protected override string keywordIconId => "BloodOfIrahUniqueBuff";

	public BattleUnitBuf_BloodOfIrahUniqueBuff(BattleUnitModel model)
	{
		this._owner = model;
		/*
		typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, IrahInitializer.ArtWorks["Blood"]);
		typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
		*/
		this.stack = 0;
	}

	// Haste Related.
	public override int GetSpeedDiceAdder(int speedDiceResult)
	{
		if (this._owner.IsImmune(bufType))
		{
			return base.GetSpeedDiceAdder(speedDiceResult);
		}
		return this.stack;
	}

	// Apply Haste.
	public override void OnRoundStart()
	{
		// Haste.
		if (!this._owner.IsImmune(bufType))
		{
			SingletonBehavior<DiceEffectManager>.Instance.CreateBufEffect("BufEffect_Quickness", this._owner.view);
		}
	}

	// Protection Effect.
	public override StatBonus GetStatBonus()
	{
		if (_owner.IsImmune(bufType))
		{
			return base.GetStatBonus();
		}
		return new StatBonus
		{
			dmgAdder = -this.stack
		};
	}

	// Power Effect.
	public override void BeforeRollDice(BattleDiceBehavior behavior)
	{
		behavior.ApplyDiceStatBonus(new DiceStatBonus
		{
			power = this.stack
		});
	}

	// Destroy upon roudn end to not get duplicates.
	public override void OnRoundEnd()
	{
		// Light Regen and Card Draw.
		this._owner.allyCardDetail.DrawCards(this.stack);
		this._owner.cardSlotDetail.RecoverPlayPoint(this.stack);
		Destroy();
	}





	public static void AddBuf(BattleUnitModel unit, int stack)
	{
		BattleUnitBuf_BloodOfIrahUniqueBuff battleUnitBuf_BloodOfIrahUniqueBuff = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BloodOfIrahUniqueBuff) as BattleUnitBuf_BloodOfIrahUniqueBuff;
		if (battleUnitBuf_BloodOfIrahUniqueBuff == null)
		{
			BattleUnitBuf_BloodOfIrahUniqueBuff battleUnitBuf_BloodOfIrahUniqueBuff2 = new BattleUnitBuf_BloodOfIrahUniqueBuff(unit);
			battleUnitBuf_BloodOfIrahUniqueBuff2.Add(stack);
			unit.bufListDetail.AddBuf(battleUnitBuf_BloodOfIrahUniqueBuff2);
		}
		else
		{
			battleUnitBuf_BloodOfIrahUniqueBuff.Add(stack);
		}
	}

	public void Add(int add)
	{
		this.stack += add;
	}

	public static void SubBuf(BattleUnitModel unit, int amount)
	{
		BattleUnitBuf_BloodOfIrahUniqueBuff bloodofIrahuniquebuf = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BloodOfIrahUniqueBuff) as BattleUnitBuf_BloodOfIrahUniqueBuff;
		bloodofIrahuniquebuf.IntegerDeleto(unit, amount);
	}

	public void IntegerDeleto(BattleUnitModel unit, int amount)
	{
		this.stack -= amount;

		if (this.stack < 0)
		{
			this.stack = 0;
		}
	}
}

/////////////////////////////// Buffs / Effects /////////////////////////////



/////////////////////////////// Dice Passives ///////////////////////////////

public class DiceCardAbility_BloodBarrage : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "BloodOfIrah" };
	public static string Desc = "[On Hit] Gain Blood equal x 0.5 of dice roll.";

	public override void OnSucceedAttack()
	{
		BattleUnitModel battleUnitModel = base.card?.target;
		if (battleUnitModel != null && base.owner != null && behavior != null)
		{
			int diceResultValue = (behavior.DiceResultValue + 1) / 2;

			BattleUnitBuf_BloodOfIrah.AddBuf(base.owner, diceResultValue);
		}
	}
}

public class DiceCardAbility_Decapitation : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "BloodOfIrah" };
	public static string Desc = "[On Hit] Gain Blood equal x 2.0 of dice roll.";

	public override void OnSucceedAttack()
	{
		BattleUnitModel battleUnitModel = base.card?.target;
		if (battleUnitModel != null && base.owner != null && behavior != null)
		{
			int diceResultValue = behavior.DiceResultValue * 2;

			BattleUnitBuf_BloodOfIrah.AddBuf(base.owner, diceResultValue);
		}
	}
}

public class DiceCardAbility_BloodWall : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "BloodOfIrah" };
	public static string Desc = "[On Roll Dice] Consume blood equal to 5% of total blood.";

	public override void OnRollDice()
	{
		BattleUnitModel battleUnitModel = base.card?.target;
		if (battleUnitModel != null && base.owner != null && behavior != null)
		{
			BattleUnitBuf_BloodOfIrah.SubBuf(base.owner, 5, 0);
		}
	}
}

public class DiceCardAbility_BloodyEnhancement : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "BloodOfIrah" };
	public static string Desc = "[On Hit] Gain 1 Power. Gain Blood equal x 1.5 of dice roll.";

	public override void OnSucceedAttack()
	{
		BattleUnitModel battleUnitModel = base.card?.target;
		if (battleUnitModel != null && base.owner != null && behavior != null)
		{
			int diceResultValue = behavior.DiceResultValue + (behavior.DiceResultValue / 2);

			base.owner.LoseHp(diceResultValue);
			base.owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.AllPowerUp, 1, base.owner);
			BattleUnitBuf_BloodOfIrah.AddBuf(base.owner, diceResultValue);
		}
	}
}

public class DiceCardAbility_Swipe : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "BloodOfIrah" };
	public static string Desc = "[On Hit] Gain Blood equal x 1.0 of dice roll. Recover 5 Break.";

	public override void OnSucceedAttack()
	{
		BattleUnitModel battleUnitModel = base.card?.target;
		if (battleUnitModel != null && base.owner != null && behavior != null)
		{
			int diceResultValue = behavior.DiceResultValue;

			BattleUnitBuf_BloodOfIrah.AddBuf(base.owner, diceResultValue);
			base.owner.RecoverBreakLife(5);
		}
	}
}

public class DiceCardAbility_Screech : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "BloodOfIrah" };
	public static string Desc = "[On Hit] If have 150 blood. Half the current stagger bar of the targets.";

	public override void OnSucceedAttack()
	{
		if (base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BloodOfIrah).stack >= 150)
		{
			BattleUnitModel battleUnitModel = base.card?.target;
			if (battleUnitModel != null)
			{
				int num = battleUnitModel.breakDetail.breakLife / 2;
				if (num < 1)
				{
					num = 1;
				}
				battleUnitModel.TakeBreakDamage(num, DamageType.Card_Ability, base.owner, AtkResist.None);
			}
		}
	}

	public override void OnWinParrying()
	{
		BattleUnitModel battleUnitModel = base.card?.target;
		if (battleUnitModel != null && base.owner != null && behavior != null)
		{
			int diceResultValue = behavior.DiceResultValue + (behavior.DiceResultValue / 2);

			base.owner.LoseHp(diceResultValue);
		}
	}
}

public class DiceCardAbility_BloodSacBurst : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "BloodOfIrah" };
	public static string Desc = "[On Hit] Consume 15 blood and recover a Light and Heal 3 HP; repeat until there isnt enough blood. Gain 1 Fragile for each Light Recovered.";

	public override void OnSucceedAttack()
	{
		BattleUnitModel battleUnitModel = base.card?.target;
		int maxlight = base.owner.MaxPlayPoint;
		int currentlight = base.owner.cardSlotDetail.PlayPoint;
		BattleUnitBuf_BloodOfIrah bloodofIrah = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BloodOfIrah) as BattleUnitBuf_BloodOfIrah;

		for (int i = 0; i < (maxlight - currentlight) && bloodofIrah.stack >= 15; i++)
		{
			if (battleUnitModel != null && base.owner != null && behavior != null)
			{
				int diceResultValue = behavior.DiceResultValue;

				BattleUnitBuf_BloodOfIrah.SubBuf(base.owner, 15, 1);
				base.owner.cardSlotDetail.RecoverPlayPoint(1);
				base.owner.RecoverHP(3);
				base.owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Vulnerable, 1 , base.owner);
			}
		}
	}
}

public class DiceCardAbility_VampireBite : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "BloodOfIrah" };
	public static string Desc = "[On Hit] Gain Blood equal x 3.0 of dice roll. Deal 8 Break. Heal 2 HP.";

    public override void OnSucceedAttack()
	{
		BattleUnitModel battleUnitModel = base.card?.target;
		if (battleUnitModel != null && base.owner != null && behavior != null)
		{
			int diceResultValue = behavior.DiceResultValue * 3;

			BattleUnitBuf_BloodOfIrah.AddBuf(base.owner, diceResultValue);
			battleUnitModel.TakeBreakDamage(8, DamageType.Card_Ability);
			base.owner.RecoverHP(2);
		}
	}
}

public class DiceCardAbility_ClawFang : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "BloodOfIrah" };
	public static string Desc = "[On Hit] Gain Blood equal x 1.0 of dice roll. Heal 5 Health.";

	public override void OnSucceedAttack()
	{
		BattleUnitModel battleUnitModel = base.card?.target;
		if (battleUnitModel != null && base.owner != null && behavior != null)
		{
			int diceResultValue = behavior.DiceResultValue;

			BattleUnitBuf_BloodOfIrah.AddBuf(base.owner, diceResultValue);
			base.owner.RecoverHP(5);
		}
	}
}

public class DiceCardAbility_VampiresRampage : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "BloodOfIrah" };
	public static string Desc = "[On Hit] Gain Blood equal x 1.0 of dice roll.";

	public override void OnSucceedAttack()
	{
		BattleUnitModel battleUnitModel = base.card?.target;
		if (battleUnitModel != null && base.owner != null && behavior != null)
		{
			int diceResultValue = behavior.DiceResultValue;

			BattleUnitBuf_BloodOfIrah.AddBuf(base.owner, diceResultValue);
		}
	}

    public override void OnLoseParrying()
    {
		int diceResultValue = behavior.DiceResultValue;
		base.owner.LoseHp(diceResultValue);
	}

    public override void OnDrawParrying()
	{
		int diceResultValue = behavior.DiceResultValue / 2;
		base.owner.LoseHp(diceResultValue);
	}
}


/////////////////////////////// Dice Passives ///////////////////////////////



/////////////////////////////// Page Passives ///////////////////////////////

public class DiceCardSelfAbility_ScreechCard : DiceCardSelfAbilityBase
{
    public static string Desc = "Dice on this page deal damage to yourself instead of the target.";

	public override void BeforeRollDice(BattleDiceBehavior behavior)
	{
		behavior.ApplyDiceStatBonus(new DiceStatBonus
		{
			dmgRate = -9999
		});
	}
}

public class DiceCardSelfAbility_SlumberCard : DiceCardSelfAbilityBase
{
	public static string Desc = "[On Use] Take 100 Break DMG. Heal 20 HP. Lose 30 Blood.";
	public override string[] Keywords => new string[] { "BloodOfIrah" };

	public override void OnUseCard()
	{
		base.owner.TakeBreakDamage(70, DamageType.Card_Ability);
		base.owner.RecoverHP(30);
		BattleUnitBuf_BloodOfIrah.SubBuf(base.owner, 30, 1);
	}
}

public class DiceCardSelfAbility_BloodyEnhancementCard : DiceCardSelfAbilityBase
{
	public static string Desc = "If a dice loses a clash, the next one will break. Dice on this page deal damage to yourself instead of the target.";

	public override void OnLoseParrying()
	{
		base.card.DestroyDice(DiceMatch.NextAttackDice);
	}

	public override void BeforeRollDice(BattleDiceBehavior behavior)
	{
		behavior.ApplyDiceStatBonus(new DiceStatBonus
		{
			dmgRate = -9999
		});
	}
}

public class DiceCardSelfAbility_VampiresRampageCard : DiceCardSelfAbilityBase
{
	public static string Desc = "Dices on this Page get tripled, and DMG gets halved. [On Clash Lose] Deal the DMG to self. [On Clash Draw] Deal DMG equal to half the roll to self.";

	public override void OnUseCard()
	{
		base.owner.allyCardDetail.AddNewCardToDeck(new LorId("MyTopStar", 51));

		List<BattleDiceCardModel> list = base.owner.allyCardDetail.GetAllDeck().FindAll((BattleDiceCardModel x) => x.GetID() == new LorId("MyTopStar", 51));
		if (list.Count <= 0)
		{
			return;
		}
		BattleDiceCardModel battleDiceCardModel = RandomUtil.SelectOne(list);
		foreach (BattleDiceBehavior item in battleDiceCardModel.CreateDiceCardBehaviorList())
		{
			if (item.Type == BehaviourType.Atk)
			{
				card.AddDice(item);
				card.AddDice(item);
			}
		}

		base.owner.allyCardDetail.ExhaustCardInDeck(new LorId("MyTopStar", 51));
	}

	public override void BeforeRollDice(BattleDiceBehavior behavior)
	{
		behavior.ApplyDiceStatBonus(new DiceStatBonus
		{
			dmgRate = -50,
			breakDmg = -50
		});
	}
}

/////////////////////////////// Page Passives ///////////////////////////////



///////////////////////////// Key Page Passives /////////////////////////////

public class PassiveAbility_IrahBloodManager : PassiveAbilityBase
{
	//For each 69 Blood the librarian has, gain 1 haste, power and protection.
	//The Librarian cannot get staggered. If blood equals or exceeds 250, instead they will lose 10% of their hp.
	//If less than 250 Blood, lose 25% instead of 10%. Finally consume 150 blood.


	public override void OnSucceedAttack(BattleDiceBehavior behavior)
	{
		BattleUnitBuf_BloodOfIrah.AddBuf(base.owner, 10);
	}

	public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
	{
		BattleUnitBuf_BloodOfIrah.AddBuf(base.owner, dmg);
	}

	public override void OnUnitCreated()
    {
		base.owner.personalEgoDetail.AddCard(new LorId("MyTopStar", 46));
		base.owner.personalEgoDetail.AddCard(new LorId("MyTopStar", 51));
	}

    public override void OnRoundEnd()
    {
		BattleUnitBuf_BloodOfIrah bloodofIrah = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BloodOfIrah) as BattleUnitBuf_BloodOfIrah;
		int bloodamount = bloodofIrah.stack;
		int stackdivamount = bloodamount / 150;

		if (stackdivamount > 3)
        {
			stackdivamount = 3;
        }
	}

	public override void OnRoundStart()
	{
		BattleUnitBuf_BloodOfIrah bloodofIrah = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BloodOfIrah) as BattleUnitBuf_BloodOfIrah;
		int bloodamount = bloodofIrah.stack;
		int stackdivamount = bloodamount / 150;

		if (stackdivamount > 3)
		{
			stackdivamount = 3;
		}

		BattleUnitBuf_BloodOfIrahUniqueBuff.AddBuf(base.owner, stackdivamount);
	}


	public override bool OnBreakGageZero()
    {
		BattleUnitBuf_BloodOfIrah bloodofIrah = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BloodOfIrah) as BattleUnitBuf_BloodOfIrah;
		int bloodamount = bloodofIrah.stack;
		int librarianHP = base.owner.MaxHp;

		if (bloodamount >= 500)
		{
			base.owner.breakDetail.RecoverBreak(666);
			BattleUnitBuf_BloodOfIrah.SubBuf(base.owner, 350, 1);
			base.owner.TakeDamage((librarianHP * 10) / 100);
		}
		else
		{
			base.owner.breakDetail.RecoverBreak(666);
			BattleUnitBuf_BloodOfIrah.SubBuf(base.owner, 350, 1);
			base.owner.TakeDamage((librarianHP * 25) / 100);

			//lose 1 hp for each 10 blood away from 350.
			base.owner.TakeDamage((350 - bloodamount) / 10);
		}

		owner.breakDetail.ResetGauge();
		return true;
	}
}

public class PassiveAbility_IrahBloodLight : PassiveAbilityBase
{
    public override void OnRoundStart()
    {
		base.owner.cardSlotDetail.RecoverPlayPoint(20);
		base.owner.allyCardDetail.DrawCards(20);
    }
}

///////////////////////////// Key Page Passives /////////////////////////////
