using UnityEngine;
using UnityEngine.UI;

namespace FontAwesome
{
    public static class FontAwesomeExtensions
    {
        public static string ToFontAwesomeString(this FontAwesomeIconEnum icon)
        {
#if NETFX_CORE
		    byte[] b = System.BitConverter.GetBytes((int)icon);
		    return System.Text.Encoding.Unicode.GetString(b, 0, b.Length);
#else
            return System.Text.Encoding.Unicode.GetString(System.BitConverter.GetBytes((int)icon));
#endif
        }
    }
}
