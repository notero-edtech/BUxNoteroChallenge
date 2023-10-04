/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using System.Linq;
using ForieroEngine.MIDIUnified;
using Michsky.UI.ModernUIPack;
using UnityEngine;

public class ModernUIPackDropdownMIDIPercussion : MonoBehaviour
{
   public CustomDropdown customDropdown;

   void Awake()
   {
      customDropdown.dropdownItems.Clear();
      foreach (var p in Enum.GetNames(typeof(PercussionEnum)))
      {
         customDropdown.AddNewItem();
         var item = customDropdown.dropdownItems.Last();
         item.itemName = string.Concat(p.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
      }
   }
}
