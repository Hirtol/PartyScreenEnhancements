using System;
using HarmonyLib;
using PartyScreenEnhancements.Saving;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.TwoDimension;
using UIExtenderLib;

namespace PartyScreenEnhancements
{
    public class SubModule : MBSubModuleBase
    {
        private UIExtender _extender;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            InitialiseSprites();

            try
            {
                PartyScreenConfig.Initialize();
            }
            catch (Exception e)
            {
                FileLog.Log($"PSE Config Load Exception: {e}");
            }

            if (PartyScreenConfig.ExtraSettings.ShowVisualAdditions)
            {
                _extender = new UIExtender("PartyScreenEnhancements");
                _extender.Register();
            }

            var harmony = new Harmony("top.hirtol.patch.partyenhancements");
            harmony.PatchAll();

            UIResourceManager.UIResourceDepot.StartWatchingChangesInDepot();
        }

        protected override void OnApplicationTick(float dt)
        {
            UIResourceManager.UIResourceDepot.CheckForChanges();
        }

        protected override void OnSubModuleUnloaded()
        {
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            if (PartyScreenConfig.ExtraSettings.ShowVisualAdditions)
                _extender?.Verify();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
        }

        public void InitialiseSprites()
        {
            ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            SpriteData spriteData = UIResourceManager.SpriteData;
            SpriteData spriteData2 = new SpriteData("PSESpriteData");
            spriteData2.Load(uiresourceDepot);
            Texture item = new Texture(new EngineTexture(TaleWorlds.Engine.Texture.CreateTextureFromPath("../../Modules/PartyScreenEnhancements/GUI/SpriteSheets/pse_icons/", "pse_icons_1.png")));
            spriteData.SpriteCategories.Add("pse_icons", spriteData2.SpriteCategories["pse_icons"]);
            spriteData.SpritePartNames.Add("pse_sort_icon", spriteData2.SpritePartNames["pse_sort_icon"]);
            spriteData.SpriteNames.Add("pse_sort_icon", new SpriteGeneric("pse_sort_icon", spriteData2.SpritePartNames["pse_sort_icon"]));
            spriteData.SpritePartNames.Add("pse_empty_button", spriteData2.SpritePartNames["pse_empty_button"]);
            spriteData.SpriteNames.Add("pse_empty_button", new SpriteGeneric("pse_empty_button", spriteData2.SpritePartNames["pse_empty_button"]));
            SpriteCategory spriteCategory = spriteData.SpriteCategories["pse_icons"];
            spriteCategory.SpriteSheets.Add(item);
            spriteCategory.Load(resourceContext, uiresourceDepot);
            UIResourceManager.BrushFactory.Initialize();
        }

        public void AddSprites(string spriteSheet, int sheetId = 1)
        {

            // InitializeSprites();
            //
            // AddSprites("pse_icons");

            var test = UIResourceManager.SpriteData.SpriteCategories;

            SpriteCategory spriteCategory = UIResourceManager.SpriteData.SpriteCategories[spriteSheet];
            spriteCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
            var texture = TaleWorlds.Engine.Texture.LoadTextureFromPath($"{spriteSheet}_{sheetId}.png",
                $"{BasePath.Name}Modules/PartyScreenEnhancements/GUI/SpriteSheets/{spriteSheet}");
            texture.PreloadTexture();
            var texture2D = new TaleWorlds.TwoDimension.Texture(new EngineTexture(texture));
            UIResourceManager.SpriteData.SpriteCategories[spriteSheet].SpriteSheets[sheetId - 1] = texture2D;
        }

        private void InitializeSprites()
        {
            UIResourceManager.UIResourceDepot.AddLocation("Modules/PartyScreenEnhancements/GUI/");
            UIResourceManager.UIResourceDepot.CollectResources();
            UIResourceManager.SpriteData.Load(UIResourceManager.UIResourceDepot);
        }
    }
}