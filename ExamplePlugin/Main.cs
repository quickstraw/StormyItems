using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace HaloItem
{
    //This is an example plugin that can be put in BepInEx/plugins/ExamplePlugin/ExamplePlugin.dll to test out.
    //It's a small plugin that adds a relatively simple item to the game, and gives you that item whenever you press F2.

    //This attribute specifies that we have a dependency on R2API, as we're using it to add our item to the game.
    //You don't need this if you're not using R2API in your plugin, it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency(R2API.R2API.PluginGUID)]

    //This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    //We will be using 2 modules from R2API: ItemAPI to add our item and LanguageAPI to add our language tokens.
    [R2APISubmoduleDependency(nameof(ItemAPI), nameof(LanguageAPI), nameof(RecalculateStatsAPI))]

    //This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class CrackedHalo : BaseUnityPlugin
    {
        public static PluginInfo PInfo { get; private set; }

        //The Plugin GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config).
        //If we see this PluginGUID as it is on thunderstore, we will deprecate this mod. Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Quickstraw";
        public const string PluginName = "CrackedHaloItem";
        public const string PluginVersion = "0.0.1";

        //We need our item definition to persist through our functions, and therefore make it a class field.
        private static ItemDef myItemDef;

        private bool providingBuff;
        private static BuffDef buffDef;

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            //Init our logging class so that we can properly log for debugging
            Log.Init(Logger);
            PInfo = Info;
            Assets.Init();

            //First let's define our item
            myItemDef = ScriptableObject.CreateInstance<ItemDef>();

            // Language Tokens, check AddTokens() below.
            myItemDef.name = "CRACKED_HALO_NAME";
            myItemDef.nameToken = "CRACKED_HALO_NAME";
            myItemDef.pickupToken = "CRACKED_HALO_PICKUP";
            myItemDef.descriptionToken = "CRACKED_HALO_DESC";
            myItemDef.loreToken = "CRACKED_HALO_LORE";

            //The tier determines what rarity the item is:
            //Tier1=white, Tier2=green, Tier3=red, Lunar=Lunar, Boss=yellow,
            //and finally NoTier is generally used for helper items, like the tonic affliction
            myItemDef.tier = ItemTier.Tier1;

            //You can create your own icons and prefabs through assetbundles, but to keep this boilerplate brief, we'll be using question marks.
            myItemDef.pickupIconSprite = Assets.mainBundle.LoadAsset<Sprite>("Assets/Import/cracked_halo_icon/CrackedHaloIcon.png");
            //myItemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");
            myItemDef.pickupModelPrefab = Assets.mainBundle.LoadAsset<GameObject>("Assets/Import/cracked_halo/crackedhalo.prefab");

            //Can remove determines if a shrine of order, or a printer can take this item, generally true, except for NoTier items.
            myItemDef.canRemove = true;

            //Hidden means that there will be no pickup notification,
            //and it won't appear in the inventory at the top of the screen.
            //This is useful for certain noTier helper items, such as the DrizzlePlayerHelper.
            myItemDef.hidden = false;

            //Now let's turn the tokens we made into actual strings for the game:
            AddTokens();

            //You can add your own display rules here, where the first argument passed are the default display rules: the ones used when no specific display rules for a character are found.
            //For this example, we are omitting them, as they are quite a pain to set up without tools like ItemDisplayPlacementHelper
            var displayRules = new ItemDisplayRuleDict(null);

            //Then finally add it to R2API
            ItemAPI.Add(new CustomItem(myItemDef, displayRules));

            //Add associated buff
            buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.iconSprite = myItemDef.pickupIconSprite;
            buffDef.name = "Cracked Halo";
            buffDef.isDebuff = false;
            buffDef.canStack = false;
            buffDef.buffColor = new Color(246, 255, 71);
            ContentAddition.AddBuffDef(buffDef);

            //But now we have defined an item, but it doesn't do anything yet. So we'll need to define that ourselves.
            RecalculateStatsAPI.GetStatCoefficients += OnGetStatCoefficients;

            // This line of log will appear in the bepinex console when the Awake method is done.
            Log.LogInfo(nameof(Awake) + " done.");
        }

        private static void OnGetStatCoefficients(CharacterBody body, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int haloCount = body.inventory.GetItemCount(myItemDef);
            if (haloCount > 0 && !body.characterMotor.isGrounded)
            {
                args.moveSpeedMultAdd += (0.20f * haloCount);
            }
        }

        //This function adds the tokens from the item using LanguageAPI, the comments in here are a style guide, but is very opiniated. Make your own judgements!
        private void AddTokens()
        {
            //The Name should be self explanatory
            LanguageAPI.Add("CRACKED_HALO_NAME", "Cracked Halo");

            //The Pickup is the short text that appears when you first pick this up. This text should be short and to the point, numbers are generally ommited.
            LanguageAPI.Add("CRACKED_HALO_PICKUP", "Increases movement speed when in the air.");

            //The Description is where you put the actual numbers and give an advanced description.
            LanguageAPI.Add("CRACKED_HALO_DESC", "Being in the air increases your movement speed by <style=cIsUtility>20%</style> <style=cStack>(+20% per stack)</style>.");

            //The Lore is, well, flavor. You can write pretty much whatever you want here.
            LanguageAPI.Add("CRACKED_HALO_LORE", "A fallen angels misfortune is your gain.");
        }

        //The Update() method is run on every frame of the game.
        private void Update()
        {
            //This if statement checks if the player has currently pressed F2.
            if (Input.GetKeyDown(KeyCode.F2))
            {
                //Get the player body to use a position:	
                var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                //And then drop our defined item in front of the player.

                Log.LogInfo($"Player pressed F2. Spawning our custom item at coordinates {transform.position}");
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(myItemDef.itemIndex), transform.position, transform.forward * 20f);
            }
        }

        private void FixedUpdate()
        {
            //ProvideBuff();
        }

        private void ProvideBuff()
        {
            CharacterBody currChar = PlayerCharacterMasterController.instances[0].master.GetBody();
            int haloCount = currChar.inventory.GetItemCount(myItemDef.itemIndex);

            if (haloCount > 0 && !currChar.characterMotor.isGrounded)
            {
                currChar.AddBuff(buffDef);
            }

            if (currChar.HasBuff(buffDef))
            {
                if (haloCount <= 0 || currChar.characterMotor.isGrounded)
                {
                    currChar.RemoveBuff(buffDef);
                }
            }
        }
    }
}
