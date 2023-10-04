using System.Collections.Generic;
using ForieroEngine.Purchasing;
using UnityEngine;

public partial class LessonManager : MonoBehaviour
{
    [System.Serializable]
    public class GradeItem
    {
        public string id = "";
        public List<LessonItem> lessonItems = new List<LessonItem>();

        public LessonItem GetLessonById(string lessonId)
        {
            foreach (LessonItem lessonItem in lessonItems)
            {
                if (lessonItem.id == lessonId)
                {
                    return lessonItem;
                }
            }

            return null;
        }

        public bool GetPurchased()
        {
            return Store.Purchased(id);
        }

        public LessonItem GetSelectedLessonItem()
        {
            LessonItem lessonItem = GetLessonById(PlayerManager.player.GetString("SELECTED_LESSON_" + id, lessonItems[0].id));

            if (lessonItem == null)
            {
                lessonItem = GetLessonById(lessonItems[0].id);
            }

            return lessonItem;
        }

        public int GetLessonIndex(LessonItem lessonItem)
        {
            int result = -1;
            for (int i = 0; i < lessonItems.Count; i++)
            {
                if (lessonItems[i] == lessonItem)
                {
                    result = i;
                }
            }
            return result;
        }

        public LessonItem GetLastCompletedLesson()
        {
            LessonItem result = null;
            for (int i = 0; i < lessonItems.Count; i++)
            {
                if (!GetLessonCompleted(lessonItems[i]))
                {
                    int index = i - 1;
                    if (index >= 0 && index < lessonItems.Count)
                    {
                        result = lessonItems[index];
                    }
                }
            }
            return result;
        }

        public LessonItem GetNextLesson()
        {
            LessonItem result = null;
            for (int i = 0; i < lessonItems.Count; i++)
            {
                if (!GetLessonCompleted(lessonItems[i]))
                {
                    result = lessonItems[i];
                    break;
                }
            }
            return result;
        }

        public void SetSelectedLessonItem(LessonItem lessonItem)
        {
            PlayerManager.player.SetString("SELECTED_LESSON_" + id, lessonItem.id);
            if (!PlayerManager.autoSave) PlayerManager.Save();
        }

        public bool GetLessonCompleted(LessonItem lessonItem)
        {
            return PlayerManager.player.GetBool("COMPLETED_LESSON_" + id + "_" + lessonItem.id, false);
        }

        public void SetLessonCompleted(LessonItem lessonItem, bool value)
        {
            PlayerManager.player.SetBool("COMPLETED_LESSON_" + id + "_" + lessonItem.id, value);
            if (!PlayerManager.autoSave) PlayerManager.Save();
        }
    }

    [System.Serializable]
    public class LessonItem
    {
        public string id = "";
        public Sprite image;
        public string nameLangId = "";
        public string descriptionLangId = "";
        public string description = "";
    }

    public GradeItem GetGradeById(string gradeId)
    {
        foreach (GradeItem gradeItem in settings.gradeItems)
        {
            if (gradeItem.id == gradeId)
            {
                return gradeItem;
            }
        }

        return null;
    }
}
