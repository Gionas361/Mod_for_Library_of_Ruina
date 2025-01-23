# Mod_for_Library_of_Ruina

This mod uses Original Character *(OC's)* made by me and my friends, I used their OC's with their consent and i have the permission to use them. Not only that, through out the process of making them I constantly asked them if what I was making was ok as to not stray from their view of who and what their OC's are.

Starlight and Zerk are both mine and were made from my imagination for which I needed no permision to use.

Modding for Library of Ruina *(LoR)* is very absurd, theres close to no documentation and the documentation there is doesnt come close to encompasing everything you need to know, for that reason you will have to use the wiki for the game which has the code for most effects, passives, dice effects, page effects, scripts and other things. Eventually you will hit a hard wall where you will have to use 0Harmony to make more incredible stuff.

0Harmony is very complicated. Due to this I will not be explaining it. This is made even harder due to the severe lack of documentation for Modding Library of Ruina as a whole. The only way you are learning everything I did, you would have to make deep dives into the channel in the discord sever for modding. Or you will have to outright ask for help. The problem with modding for LoR is that basically everything is very nieche. Most of the coding is something you only do bcs you want to make something VERY specific.

If you wish to download this Mod, [CLICK ME](https://steamcommunity.com/sharedfiles/filedetails/?id=3247414691) to get sent to the Steam Page.


## The Structure Directory:

* [Resource](Resource/):
    - [CharacterSkin](Resource/CharacterSkin/)
        - Here, you will make folders with a file named ModInfo.Xml that specifies the pivot points of the head and body in reference to the images of the sprites for that character's Key Page in the folder ClothCustom.

    - [CombatPageArtwork](Resource/CombatPageArtwork/)
        - Here, you will put the images for the Pages *(Attack Cards)*.

    - [Effects](Resource/Effects/)
        - This would only matter if you are using the 0Harmony package for making special effects. 

    - [MDPs](Resource/MDPs/)
        - This is a personal folder which includes the RAW files of the art of most if not all the art I made in Medibang.

    - [MotionSound](Resource/MotionSound/)
        - Here you put the SFX for when a character equiped with the Key Page will make when moving in-game.

    - [Reference](Resource/Reference/)
        - This is another personal folder made to store images I used as reference for the characters in my mod. 

    - [StoryBgm](Resource/StoryBgm/)
        - In the game you can make small stories, akin to a Visual Novel for which this folder serves to story the Background Music for the story.

    - [StoryBgSprite](Resource/StoryBgSprite/)
        - Here the Background Sprites for the story is stored.

    - [StoryStanding](Resource/StoryStanding/)
        - Here you would story the sprite of Characters that will appear in the small story.


## The Data Directory:

* [Data](Data/):
    - [StoryEffect](Data/StoryEffect/)
        - Im not sure about this one as i didnt use it and cannot infer from the name to accuretly tell you what it is.
    
    - [StoryText](Data/StoryText/)
        - Here the dialog or narration of the stories is stored.
    
    - [BookStory](Data/BookStory.xml)
        - In game you "burn" books to receive the Pages and Key Pages *(akin to gacha style, it makes it so that theres replay value in beating a stage multiple times as you might and will not receive all the Pages with only 1~3 books.)*, these books can have text which you can read in game. This file stores the text you want that book to have.

    - [CardDropTable](Data/CardDropTable.xml)
        - This file stores the Pages that can be received from burning the book.
        
    - [CardInfo](Data/CardInfo.xml)
        - Pages have a LOT of values that need to be stored, this file stores those values for EACH Page made. The most notable values stored are:
            - **Name:** Name of the card.
            - **Arwork:** The image used for the card.
            - **Rarity:** The rarity of the card which changes the amount that can be obtain from burning the Book.
            - **Option:** This is used to make E.G.O pages which are given to the character in a different Hand and may need to recharge if its set to "EgoPersonal" if its set to "Personal" it wont need to recharge.
            - **Range:** The disctance of the attack, there are 4 type of ranges for a card:
                - **Near:** It will go into clashing distance.
                - **Range:** It will always act at the start of the scene *(round)* and when losing a clash the dice of the opponent doesnt break unlike in Near clashes.
                - **FarAreaEach:** Acts at the start of the scene and each dice on the page will hit a individual dice in the page of EVERY opponent that is targetable.
                - **FarArea:** Acts at the start of the scene and each dice on the page will roll against every dice of the opponents page, thus your page rolls that dice and the opponent will roll all the dice in the page and sum them to determine the final value.
            - **Cost:** This is the amount of Light *(energy)* the Page will consume when used.
            - **Script:** This is an aditional effect that you can put on the page to do. For example you can make it draw 1 card when the page is used.
            - **BehaviourList:** It contains the data for each dice, these next variables are the values that matter for each dice:
                - **Min:** Bare minimum value a dice can roll.
                - **Dice:** Absolute maximum value a dice can roll 
                - **Type:** If its a Atk *(Attack)* dice, Def *(Defence)* dice, or a Standby *(Counter)* dice.
                - **Detail:** If its an Atk dice it can be either Pierce, Slash or Hit. If its a Def dice it can be a Guard or Evade dice. And if its an Standby dice it can be any of the Atk or Def dice types.
                - **Motion:** This is the sprite of the Character Key Page that will be used when the dice wins the clash. It can be any one of these: G *(Guard)*, J *(Slash)*, E *(Evade)*, H *(Hit)*, F *(Fire/Ranged)*, Z *(Pierce)*.
                - **EffectRes:** These is aditional VFX that are either from the base game or custom made with 0Harmony.
                - **Script:** This is an aditional effect that you can put on the dice, for it to do. For example you can make it draw 1 card if the dice hits the enemy.
            - **Chapter:** This is the category in which the page will appear in game. This category being the chapter of the Story in which it would be obtained.
            - **Priority:** The priority in which an enemy character will use this page, the higher the priority the earlier the page will be used. For example, lets say the character has 4 light and it has a page that consumes 3 light and has 100 priority, and 2 pages that consume 2 light and 50 priority. The character, instead of using the 2 pages that consume the 4 light, instead it will use the single page that consumes 3 light bcs it has higher priority.
            - **MaxCooltmeForEgo:** If the Option value is used and set to EgoPersonal, this is the amount of Emotion Points you will need to obtain for the Page to recharge.

    - [Combat_Dialog](Data/Combat_Dialog.xml)
        - Here the dialog for the Key Pages is stored.
        
    - [Deck_Enemy](Data/Deck_Enemy.xml)
        - This is the deck of cards the Key Page contains for the enemy character that have this Key Page equiped will have in the combat stage.
        
    - [Dropbook](Data/Dropbook.xml)
        - This stores the Key Pages that can be obtained when burning the book.
        
    - [EnemyUnitInfo](Data/EnemyUnitInfo.xml)
        - This is where an enemy is made and you give it all the info needed to be added into a stage. Thus the stage will reference the ID of the enemy and get the info from this file. The important values of this file are:
            - **Name:** The name of the enemy.
            - **FaceType:** If it will use the head tyoe of the Key Page or not.
            - **MinHeight:** The minimum height of the enemy when spawned in the game. 
            - **MaxHeight:** The maximum height of the enemy when spawned in the game.
            - **Gender:** If its make or female *(this matters in some cases if the Key Page skin has different sprites for female or male version)*
            - **Retreat:** If when defeated in the fight will die or escape.
            - **BookId:** The ID of the Key Page this enemy character will use in combat.
            - **DropBonus:** The bonus amount of books obtained from defeating this enemy.
            - **DropTable:** The books that this enemy will drop when defeated.
            - **AiScript:** Although I didnt use it, while deep diving in the discord server I saw many instances of what this is. Essentially this references the code that will control this enemy character when in combat. This is very useful for thematic fights that require the character to follow some patter that isnt easily achieveable with just using the priority variable in the CardInfo file.
        
    - [EquipPage_Enemy](Data/EquipPage_Enemy.xml)
        - This contains the values of the Key Pages an enemy can have equiped. *(Important Values will be on EquipPage_Librarian)*
        
    - [EquipPage_Librarian](Data/EquipPage_Librarian.xml)
        - This contains the values of the Key Pages you can obtain from burning a book. You have to make one for the enemy and for the player because if you gave the player the same stats as some bosses, the numbers would increase too much. Anyways, here are the important values of this file:
            - **Name:** Name of the Key Page.
            - **BookIcon:** Althogh I didnt use it and I dont know how to use it, it probable just means to the logo the book will have.
            - **EquipEffect:** This contains base stats and resistances of the Key Page.
                - **HP:** Base HP of the Key Page.
                - **Break:** Base Break of the Key Page
                - **SpeedMin:** The base minimum this Key Pages speed dice will roll.
                - **Speed:** The base maximum this Key Pages speed dice will roll.
                - **SResist:** The resistance to Slash damage towards HP.
                - **PResist:** The resistance to Pierce damage towards HP.
                - **HResist:** The resistance to Hit damage towards HP.
                - **SBResist:** The resistance to Slash damage towards Break.
                - **PBResist:** The resistance to Pierce damage towards Break.
                - **HBResist:** The resistance to Hit damage towards Break.
                - **MaxPlayPoint:** The base cuantity of Light the Key Page has.
                - **StartPlayPoint:** The amount of Light that is charged when the First wave begins.
                - **AddedStartDraw:** How many aditional cards are drawn at the 
                - **Passive:** This are passives that do not rely on Pages being used, these abilities are always active and checking if the conditions set are fulfield, for example. U could make it so that at the start of each round the Librarian gets +1 Power, or if any Page is used, the dices on that page gain +2 more power or something.
            - **Rarity:** The rarity of the page, the rarer the less amount u can obtain when the book is burned. Key Pages do not refresh when all of them are obtained unlike regular Pages.
            - **CharacterSkin:** The name of the Skin that will be used in the Key Page.
            - **CharacterSkinType:** If the skin is a skin from the base game or if its custom made in the mod.
            - **SkinGender:** If this skin has a specific gender in mind in the art.
            - **Chapter:** This is the category in which the Key Page will appear in game. This category being the chapter of the Story in which it would be obtained.
            - **RangeType:** If it can equip Ranged, Near or Both.
            - **RandomFace:** If the skin has a head and hair in the artwork sprites, set to False. If it doesnt have a head in the sprite work, set it to True.
            - **SpeedDiceNum:** Base amount of speed die the character will have *(Default is 1 [Dont set it to 0])*.
            - **SuccessionPossibleNumber:** The amount of abilities the Key Page can inherit and contain.

    - [PassiveList](Data/PassiveList.xml)
        - Contains the information of the Passives a Key Page can equip. The important values are:
            - **Rarity:** The rarity of the ability.
            - **CanGivePassive:** If this passive can be inherited by other Key Pages.
            - **Cost:** The cost to inherit this passive.
            - **Script:** The code that will be used for this passive.
            - **Name:** The name of the passive.
            - **Desc:** The description of this passive inside of the game.

    - [StageInfo](Data/StageInfo.xml)
        - This contains the information of every stage in the mod, these stages are the level where you fight the enemies you codded previously. The important values are:
            - **Name:** Name of the stage.
            - **Wave:** This is a wave of enemies *(Max 3)*.
                - **Unit:** This is an enemy, you will write the ID of the enemy you want added.
                - **Formation:** The formation in which the enemies are positioned.
                - **FormationType:** I dont know this but i infer that this refers to if the Formation is custom made or not *(I dont know how you would make one)* 
                - **AvailibleUnit:** The amount of characters the player can use on this wave.
            - **FloorNum:** The number of teams the player can use *(Max 3)*.
            - **Chapter:** The chapter in which this stage would normally be fought in.
            - **Invitation:** What type of invitation this is.
                - **BookValue:** This will take into account the value of each book and number, if the minimum is reached. The stage can be fought.
                    - **Value:** The value of the books.
                    - **Num:** The number of books needed.
                - **BookRecipe:** This will need specific books to fight the stage.
                    - **Book:** The ID of the book in the recipe *(Max 3)*.
            - **Condition:** This will determine the max level of emotion the enemy characters can reach.
                - **Level:** The max number of emotion *(Max 5)*.
            - **Story:** The Visual Novel sections that will be played during the reception of the enemies.
                - **Start:** The story that will be played when the Reception starts.
                - **End:** The story that will be played when the Reception ends.


## The Assemblies Directory:

* [Assemblies](Assemblies/):
    - Here are the DLL's of all the code in the mod. It also contains the Visual Studio 2019 projects with the code for ease of access.

    - [@DDJJ_DLLs](Assemblies/@DDJJ_DLL's/Class1.cs):
        - This contains the Key Page Passives, Page Passives and Dice Passives for **DDJJ**.
    
    - [@DDJJEGO_DLLs](Assemblies/@DDJJEGO_DLLs/Class1.cs):
        - This contains the Key Page Passives, Page Passives and Dice Passives for **DDJJEGO**.
    
    - [@Jijikan_DLLs](Assemblies/@Jijikan_DLLs/Class1.cs):
        - This contains the Key Page Passives, Page Passives and Dice Passives for **Jijikan**.

    - [@Yaren_DLLs](Assemblies/@Yaren_DLLs/Class1.cs):
        - This contains the Key Page Passives, Page Passives and Dice Passives for **Yaren**.
    
    - [Demi-DLLs](Assemblies/Demi-DLLs/Class1.cs):
        - This contains the Key Page Passives, Page Passives and Dice Passives for **Demi**.
    
    - [Gionas_DLLs](Assemblies/Gionas_DLLs/Class1.cs):
        - This contains the Key Page Passives, Page Passives and Dice Passives for **Gionas**.
    
    - [Irah_DLLs](Assemblies/Irah_DLLs/Class1.cs):
        - This contains the Key Page Passives, Page Passives and Dice Passives for **Irah**.

    - [RenDLLs](Assemblies/RenDLLs/Class1.cs):
        - This contains the Key Page Passives, Page Passives and Dice Passives for **Ren**.
    
    - [Starlight's_DLLs](Assemblies/Passive1/Class1.cs):
        - This contains the Key Page Passives, Page Passives and Dice Passives for **Starlight** and **Noveau Starlight**.
    
    - [VFXs_For_Everyone](Assemblies/VFXs_For_Everyone/Class1.cs):
        - Although empty, it was meant to have the Visual Effects for every attack of the characters, but it was too confusing and lacked enough documentation to do in a realistic time frame. Not only that the discord lacks message logs relating to this.

    - [Zerk_DLLs](Assemblies/Zerk_DLLs/Class1.cs):
        - This contains the Key Page Passives, Page Passives and Dice Passives for **Zerk**.