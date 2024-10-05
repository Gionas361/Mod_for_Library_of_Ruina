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

using System.Xml;
using CustomInvitation;
using Sound;
using StoryScene;

using WorkParser;
using Workshop;
using System.Collections;
using LOR_BattleUnit_UI;
using Battle.CreatureEffect;
using Unity.Mathematics;



namespace VFXs_For_Everyone
{
    public class MyTopStarInitializer : ModInitializer
    {
        public override void OnInitializeMod()
        {
            base.OnInitializeMod();
            // do the default initalization procedures
            Harmony harmony = new Harmony("Gionas361.MyTopStarInitializer");
            // set up Harmony
            MethodInfo method = typeof(MyTopStarInitializer).GetMethod("DiceEffectManager_CreateBehaviourEffect_Pre");
            harmony.Patch(typeof(DiceEffectManager).GetMethod("CreateBehaviourEffect", AccessTools.all), new HarmonyMethod(method), null, null, null, null);
            // Attaches a harmony script to the dice effect method, so that we can attach the custom effect to it at our discresion
            MyTopStarInitializer.path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            // gets the path to the mod itself.
            MyTopStarInitializer.GetUnity(new DirectoryInfo(MyTopStarInitializer.path + "/CustomEffect"));
            // this loads all of the unity assetbundles in [your mod assemblies directory]/CustomEffect.
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Name.StartsWith("DiceAttackEffect_"))
                {

                    MyTopStarInitializer.CustomEffects[type.Name.Substring("DiceAttackEffect_".Length)] = type;
                }
            }
            // this loads all custom effect types from your dll

            // TO ADD IMAGE EFFECTS YOU HAVE TO ADD EM ALL INDIVIDUALY.
            MyTopStarInitializer.AddEffect("Dream_Pierce", MyTopStarInitializer.path + "/CustomEffect/Dream_Pierce.png");
        }
        static MyTopStarInitializer()
        {
            MyTopStarInitializer.CustomEffects = new Dictionary<string, Type>();
            MyTopStarInitializer.assetBundle = new Dictionary<string, UnityEngine.AssetBundle>();
            //if you don't have this, you get an error upon loading the mod. it makes sure youre referencing objects correctly and sets up the dictionaries and such. maybe. i dont actually know but it seems pretty straightworward
        }

        //this is what checks to attach the custom effect!
        public static bool DiceEffectManager_CreateBehaviourEffect_Pre(DiceEffectManager __instance, ref DiceAttackEffect __result, string resource, float scaleFactor, BattleUnitView self, BattleUnitView target, float time = 1f)
        {
            if (resource == null)
            {
                __result = null;
                return false;
            }

            if (MyTopStarInitializer.CustomEffects.ContainsKey(resource))
            {
                //this is what happens if you ARE tryign to call a modded effect. basically just preps it to be loaded
                Type componentType = MyTopStarInitializer.CustomEffects[resource];
                DiceAttackEffect diceAttackEffect = new GameObject(resource).AddComponent(componentType) as DiceAttackEffect;
                diceAttackEffect.Initialize(self, target, 1f);
                diceAttackEffect.SetScale(scaleFactor);
                __result = diceAttackEffect;
                return false;
            }
            return true;
        }

        //this preps all of the asstebundles in the Assemblies' CustomEffect directory and adds them to a dictionary
        // if you dont know what a disctionary is, just think of lit like a list that you call with keywords instead of number indexes
        //the keyword for a specific entry is automatically the name of the file (w/o any extention) so thats how you can call it from the list later
        public static void GetUnity(DirectoryInfo dir)
        {
            if (dir.GetDirectories().Length != 0)
            {
                DirectoryInfo[] directories = dir.GetDirectories();
                for (int i = 0; i < directories.Length; i++)
                {
                    MyTopStarInitializer.GetUnity(directories[i]);
                }
            }
            foreach (System.IO.FileInfo fileInfo in dir.GetFiles())
            {


                MyTopStarInitializer.AddAssets(Path.GetFileNameWithoutExtension(fileInfo.FullName), fileInfo.FullName);
            }
        }

        //this finalizes loading all the Image VFX so that the game can use them
        public static void AddEffect(string name, string path)
        {
            byte[] array = File.ReadAllBytes(path);
            Texture2D texture2D = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture2D, array);
            MyTopStarInitializer.EffectSprites.Add(name, Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), 100f, 0U, 0));
        }

        //this finalizes loading all the assetbundles so that the game can use them
        public static void AddAssets(string name, string path)
        {
            AssetBundle value = AssetBundle.LoadFromFile(path);
            MyTopStarInitializer.assetBundle.Add(name, value);
        }

        //everything below here is just variable initalization :) don't forget or remove them though we do need em
        public static string path;
        public static Dictionary<string, AssetBundle> assetBundle;
        public static Dictionary<string, Type> CustomEffects = new Dictionary<string, Type>();
        public static Dictionary<string, Sprite> EffectSprites = new Dictionary<string, Sprite>();
    }

    

    public class DiceAttackEffect_dreamPierce : DiceAttackEffect
    {
        // Token: 0x060000F4 RID: 244 RVA: 0x00009F27 File Offset: 0x00008127
        public override void Initialize(BattleUnitView self, BattleUnitView target, float destroyTime)
        {
            this.duration = destroyTime;
            this.spr.sprite = MyTopStarInitializer.EffectSprites["Dream_Pierce"];
            base.Initialize(self, target, destroyTime);
        }

        // Token: 0x060000F5 RID: 245 RVA: 0x00009F54 File Offset: 0x00008154
        protected override void Update()
        {
            base.Update();
            this.duration -= Time.deltaTime;
            base.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, this.duration * 2f);
        }

        // Token: 0x0400004C RID: 76
        private float duration;
    }

    public class DiceAttackEffect_ddjjAuraEffect : DiceAttackEffect
    {
        public override void Initialize(BattleUnitView self, BattleUnitView target, float destroyTime)
        {
            //initializationinitalization of variables and such. boring
            base.Initialize(self, target, destroyTime);
            this._self = self.model;
            this._targetView = target;
            this._selfTransform = self.atkEffectRoot;
            this._targetTransform = target.atkEffectRoot;
        }

        protected override void Start()
        {
            UnityEngine.AssetBundle assetBundle = MyTopStarInitializer.assetBundle["ddjjaura"];
            // searches the dictionary from before for an entry called "nameOfBundle" and grabs it
            for (int i = 0; i < assetBundle.GetAllAssetNames().Length; i++)
            {
                string name = assetBundle.GetAllAssetNames()[i];
                this.theVFX = UnityEngine.Object.Instantiate<GameObject>(assetBundle.LoadAsset<GameObject>(name));
                //gets all the shit from inside the assetbundle "yourBundle"
            }

            this.theVFX.transform.parent = this._selfTransform;
            this.theVFX.transform.localPosition = new Vector3(0f, 0f, 0f);

            //setting it up to be spawned into the scene
            if (this._self.view.WorldPosition.x > this._targetView.WorldPosition.x)
            {
                //this should flip the effect if the character is facing the other way idk
                //i havent been able to test if it works yet but i copied base game code so i mean. theres no reason it Shouldnt
                this.theVFX.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            }
            this.theVFX.layer = LayerMask.NameToLayer("Effect");
            this.theVFX.SetActive(true);
            //finishes turning on the effect
        }

        //once again just initalizing variables
        private GameObject theVFX;
        private BattleUnitView _targetView;
    }



    public class BehaviourAction_ButterFlyCulmination : BehaviourActionBase
    {
        //this is the behavior action
        public override FarAreaEffect SetFarAreaAtkEffect(BattleUnitModel self)
        {
            this._self = self;
            FarAreaEffect_ButterflyEffect farAreaEffect_ButterflyEffect = new GameObject().AddComponent<FarAreaEffect_ButterflyEffect>();
            //gets the FarAreaEffect called FarAreaEffect_ButterflyEffect and makes a FarAreaEffect_ButterflyEffect out of it
            bool flag = farAreaEffect_ButterflyEffect != null;
            //catches null
            if (flag)
            {
                farAreaEffect_ButterflyEffect.Init(self, Array.Empty<object>());
                //initializesinitalizes the effect
            }
            return farAreaEffect_ButterflyEffect;
            //returns the effect. really simple. this whole function is really basic
        }
    }

    //above this comments is therthe behavior action, below it is the far area effect. the bottom one is more complex be careful!!!
    public class FarAreaEffect_ButterflyEffect : FarAreaEffect
    {
        // This should move to center of stage the user.
        public override void Init(BattleUnitModel self, params object[] args)
        {
            base.Init(self, args);
            this.OnEffectStart();
            this.flag = true;
            this.flag2 = true;
            this.state = 1;

            this.owner = self;
            UnityEngine.AssetBundle assetBundle = MyTopStarInitializer.assetBundle["ddjjaura"];

            num = 1;
            if (this.owner.view.WorldPosition.x > 0f)
            {
                num = -1;
            }

            /*--------------------------------------------SelfMade--------------------------------------------*/

            // Move to middle
            this.owner.moveDetail.Move(new Vector3(-0.001f * (float)num, 0f, 0f), 100f, true, false);
            this._elapsed = 0f;
        }

        protected override void Update()
        {
            //this controls all the timing of your effect! this also varies hugely based on what your effect is
            // using dnSpy you can go to Assembly-CSharp.dll / - / FarAreaEffect and scroll thru to see the basegame mass attack effects
            // you can look in their code if they have anything special that you wanna replicate, like camera filters or whateverwhatecver
            base.Update();
            this._elapsed += Time.deltaTime;


            if (this._elapsed <= 3)
            {
                // Slowing Down
                if (this._elapsed >= 1.1 && this._elapsed <= 1.24) this.owner.view.charAppearance.ChangeMotion((ActionDetail)0);
                // Raising Wings
                if (this._elapsed >= 1.25 && this._elapsed <= 1.5) this.owner.view.charAppearance.ChangeMotion((ActionDetail)4);
                // Opening Wings
                if (this._elapsed >= 1.51 && this._elapsed <= 2) this.owner.view.charAppearance.ChangeMotion((ActionDetail)5);
                // Changing to "Up High" sprite
                if (this._elapsed >= 2.87 && this._elapsed <= 3) this.owner.view.charAppearance.ChangeMotion((ActionDetail)19);
            }
            else
            {
                // Up High
                if (this.state == 1)
                {
                    if (this.owner.moveDetail.isArrived)
                    {
                        this.owner.moveDetail.Move(new Vector3(0f * (float)num, 400f, 0f), 850f, true, false);

                        this.state = 2;
                    }
                }
                // Down Low
                else if (this.state == 2 && this._elapsed >= 5)
                {
                    this.owner.view.charAppearance.ChangeMotion((ActionDetail)5);
                    
                    if (this.owner.moveDetail.isArrived)
                    {
                        if (this.owner.view.WorldPosition.y == 400)
                        {
                            this.owner.moveDetail.Move(new Vector3(0f * (float)num, 0f, 0f), 700f, true, false);
                            this.flag2 = false;
                        }

                        if (this.owner.view.WorldPosition.y == 0)
                        {
                            this.state = 3;
                        }
                        
                    }
                }
                // Boom
                else if (this.state == 3)
                {
                    if (this.owner.moveDetail.isArrived)
                    {
                        this.owner.view.charAppearance.ChangeMotion((ActionDetail)4);

                        AssetBundle assetBundle = MyTopStarInitializer.assetBundle["ddjjaura"];
                        //gets the bundle with the name "yourBundle" from in your assets folder
                        for (int i = 0; i < assetBundle.GetAllAssetNames().Length; i++)
                        {
                            string name = assetBundle.GetAllAssetNames()[i];
                            this.boom = UnityEngine.Object.Instantiate<GameObject>(assetBundle.LoadAsset<GameObject>(name));
                            //instantiates all the objects in the bundle
                        }
                        this.boom.transform.parent = this._self.view.atkEffectRoot;
                        this.boom.transform.localPosition = new Vector3(0f, 0f, 0f);
                        this.boom.layer = LayerMask.NameToLayer("Effect");
                        this.boom.SetActive(true);

                        this.isRunning = false;
                        this.state = 4;
                    }
                }
                // Ending
                else if (this.state == 4 && this._elapsed >= 9)
                {
                    if (this.owner.moveDetail.isArrived)
                    {
                        this.owner.view.charAppearance.ChangeMotion((ActionDetail)5);

                        this._isDoneEffect = true;

                        UnityEngine.Object.Destroy(base.gameObject);
                        UnityEngine.Object.Destroy(this.boom);
                    }
                }
            }
        }

        //from here on its just variables again
        private int state;
        private bool flag;
        private bool flag2;
        private float _elapsed = 0f;
        private int num;
        private BattleUnitModel owner;
        private GameObject boom;
        private GameObject effect;
    }
}

