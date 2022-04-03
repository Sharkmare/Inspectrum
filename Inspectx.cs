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
                //Fetch string from web
                string[] strarray = downloadString.Split(Environment.NewLine.ToCharArray());
                //Split string based on newlines
                Random random = new Random();
                int i = random.Next(0, strarray.Length - 1);
                //choose random option of array based on length with the last entry dropped as it will always be an empty newline due to windows filesystem adding one
                downloadString = strarray[i];

                __instance.Slot.AttachComponent<DuplicateBlock>();
                //Blocks duplication by existing on object root.

                FrooxEngine.UIX.Image image1 = __instance.Slot.GetComponentInChildren<FrooxEngine.UIX.Image>();
                FrooxEngine.UIX.Image image2 = image1.Slot[0].GetComponentInChildren<FrooxEngine.UIX.Image>();
                FrooxEngine.UIX.Image image3 = image1.Slot[1].GetComponentInChildren<FrooxEngine.UIX.Image>();
                //The above will break if the inspector hierarchy is ever modified as it expects 2 children beneath the first images slot.
                //Slot[0] is equal to GetChild 0 in Logix
                image1.Tint.Value = color.Black;

                //Redid the color definitions, thanks badhaloninja.
                color geen = new color(.5f,.2f,1f,.25f);
                color gren = new color(.2f,1f,.2f,.25f);
                color dogman = new color(1f,1f,.2f,.25f);

                color gree = color.Gray;
                gree  = gree.SetA(.25f);
                color dgree = color.Gray;
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
                //.Localuser gives us the instantiating user "ourselves"
                
                var NeosUIStyle = __instance.Slot.AttachComponent<NeosUIStyle>();


                NeosUIStyle.Color.Value = color.Black;
                NeosUIStyle.UserParentedColor.Value = color.Black;
                //This sets the regular unparented color and parented color of the base neus ui panel
                
                __result.Style = NeosUIStyle;
                __result.Color = __result.Color.SetA(0f);
                __result.RunInUpdates(3, () => {
                    __result.MarkChangeDirty();
                    __result.Title = downloadString; // Title gets set after setup finishes so this needs to run after it gets set
                    __result.Slot.PersistentSelf = false;
                    //__result.Slot.Name = downloadString; //Uncomment to allow names to also change, comment out for compatibillity with Ghosts punch and the kitchen gun
                    __result.Thickness.Value = 0.005f; //Sleeker.
                    __result.ShowHandle.Value = false; //SLEEEKER!
                });
            }
        }
    }
}