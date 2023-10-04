using System;
using System.Collections.Generic;
using UnityEngine;

public class MSMVPFeaturesUI : MonoBehaviour
{
   public string setName = "";
   public bool apply = false;
   [Serializable] public class Feature
   {
      public RectTransform rt;
      public bool available = true;
   }

   public List<Feature> demo = new List<Feature>();
   public List<Feature> windows = new List<Feature>();
   public List<Feature> ui = new List<Feature>();

   private void Awake()
   {
      if (apply)
      {
         demo.ForEach(f => { if (f.rt) f.rt.gameObject.SetActive(f.available); });
         windows.ForEach(f => { if (f.rt) f.rt.gameObject.SetActive(f.available); });
         ui.ForEach(f => { if (f.rt) f.rt.gameObject.SetActive(f.available); });
      }
   }
}
