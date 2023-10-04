/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
namespace ForieroEngine.Music.MusicXML.Xsd
{
    public static partial class MusicXMLExtensions
    {
        public static key[] SelfOrDefault(this key[] keys) => keys ??= new key[1] { ((key)null).SelfOrDefault() };

        public static key SelfOrDefault(this key key)
        {
            if (key != null) return key;
            key = new key();
            key.ItemsElementName = key.ItemsElementName.Add(ItemsChoiceType10.fifths);
            key.Items = key.Items.Add<string>("0");
            key.ItemsElementName = key.ItemsElementName.Add(ItemsChoiceType10.mode);
            key.Items = key.Items.Add<string>("major");
            return key;
        }

        public static int GetStaveNumber(this key key, int defaultValue = -1) => key == null || !int.TryParse(key.number, out var temp) ? defaultValue : temp - 1;
    }
}
