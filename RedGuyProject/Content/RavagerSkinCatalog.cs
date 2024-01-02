using System;

namespace RedGuyMod
{
    internal static class RavagerSkinCatalog
    {
        internal static RavagerSkinDef[] skinDefs = new RavagerSkinDef[0];

        internal static void AddSkin(RavagerSkinDef skinDef)
        {
            Array.Resize(ref skinDefs, skinDefs.Length + 1);

            int index = skinDefs.Length - 1;
            skinDefs[index] = skinDef;
        }

        internal static RavagerSkinDef GetSkin(int index)
        {
            return skinDefs[index];
        }

        internal static RavagerSkinDef GetSkin(string nameToken)
        {
            for (int i = 0; i < skinDefs.Length; i++)
            {
                if (skinDefs[i] && skinDefs[i].nameToken == nameToken) return skinDefs[i];
            }
            return skinDefs[0];
        }
    }
}