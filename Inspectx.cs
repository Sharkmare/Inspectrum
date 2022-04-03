using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using BaseX;
using System.Net;
using System;

namespace Inspectx
{
    public class Inspectx : NeosMod
    {
        public override string Author => "Sharkmare";
        public override string Name => "Inspectx";
        //Built on Cyros Inspectrum
        public override string Version => "1.1.0";

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.Cyro.Inspectrum");
            harmony.PatchAll();
        }


        [HarmonyPatch(typeof(InspectorPanel), "Setup")]
        class InspectxPatcher
        {
            static void Postfix(InspectorPanel __instance, NeosPanel __result)
            {
                WebClient client = new WebClient();
                string downloadString = client.DownloadString("https://github.com/Sharkmare/SHCLD/raw/main/_inspector_title");
                var strarray = downloadString.Split(Environment.NewLine.ToCharArray());
                Random random = new Random();
                int i = random.Next(0, strarray.Length - 1);
                downloadString = strarray[i];

                __instance.Slot.AttachComponent<DuplicateBlock>();
                
                var image1 = __instance.Slot.GetComponentInChildren<FrooxEngine.UIX.Image>();
                var image2 = image1.Slot[0].GetComponentInChildren<FrooxEngine.UIX.Image>();
                var image3 = image1.Slot[1].GetComponentInChildren<FrooxEngine.UIX.Image>();
                image1.Tint.Value = color.Black;
                
                var geen = color.Black;
                geen = geen.SetR(0.5f);
                geen = geen.SetG(0.2f);
                geen = geen.SetB(1f);
                geen = geen.SetA(.25f);

                var gren = color.Black;
                gren = gren.SetR(.2f);
                gren = gren.SetG(1f);
                gren = gren.SetB(.2f);
                gren = gren.SetA(.25f);

                var dogman = color.Black;
                dogman = dogman.SetR(1f);
                dogman = dogman.SetG(1f);
                dogman = dogman.SetB(.2f);
                dogman= dogman.SetA(.25f);

                var gree = color.Gray;
                gree  = gree.SetA(.25f);
                var dgree = color.Gray;
                dgree = dgree.SetA(.5f);

                switch (downloadString)
                {
                    case string a when a.Contains("Geenz"):
                        image2.Tint.Value = geen;
                        image3.Tint.Value = gree;
                        break;

                    case string a when a.Contains("Nex"):
                        image2.Tint.Value = gren;
                        image3.Tint.Value = gree;
                        break;

                    case string a when a.Contains("Froox"):
                        image2.Tint.Value = dogman;
                        image3.Tint.Value = gree;
                        break;

                    default:
                        image2.Tint.Value = dgree;
                        image3.Tint.Value = gree;
                        break;
                }

                var doul = __instance.Slot.AttachComponent<DestroyOnUserLeave>();
                doul.TargetUser.User.Value = __result.Slot.LocalUser.ReferenceID;
                var NeosUIStyle = __instance.Slot.AttachComponent<NeosUIStyle>();


                NeosUIStyle.Color.Value = color.Black;
                NeosUIStyle.UserParentedColor.Value = color.Black;
                
                __result.Style = NeosUIStyle;
                __result.Color = __result.Color.SetA(0f);
                __result.RunInUpdates(3, () => {
                    __result.MarkChangeDirty();
                    __result.Title = downloadString; // Title gets set after setup finishes so this needs to run after it gets set
                    __result.Slot.PersistentSelf = false;
                    __result.Slot.Name = downloadString;
                    __result.Thickness.Value = 0.005f;
                    __result.ShowHandle.Value = false;
                });
            }
        }
    }
}