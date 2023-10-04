using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using ForieroEngine;
using ForieroEngine.Extensions;
using DG.Tweening;

using PlayerPrefs = ForieroEngine.PlayerPrefs;

public partial class LessonManager : MonoBehaviour
{
    public LessonManagerSettings settings;

    public LessonInfo lessonInfo;
    public GradePurchase gradePurchase;

    public bool loadAsync = true;

    readonly int freeLessons = 1;

    public RectTransform gradesRect;
    public GameObject PREFAB_Grade;
    public float gradesDistance = 20f;

    public RectTransform lessonsRect;
    public GameObject PREFAB_Lesson;
    public float lessonsDistance = 100f;

    public ScrollRect lessonScrollRect;
    public RectTransform lessonContentRect;

    public RectTransform inRect;
    public RectTransform outRect;

    public static GradeItem selectedGradeItem;
    public static LessonItem selectedLessonItem;
    public static PlayerManager.GamePlayer gamePlayer;


    public List<Grade> grades = new List<Grade>();
    public List<Lesson> lessons = new List<Lesson>();

    void Awake()
    {
        gradePurchase.finished += PurchaseFinished;
    }

    void PurchaseFinished()
    {
        Debug.Log("PuchaseFinished");

        if (selectedGradeItem.GetPurchased())
        {
            foreach (Lesson l in lessons)
            {
                l.locked = false;
            }

            LoadLesson();
        }
    }

    void Start()
    {
        if (gamePlayer != PlayerManager.player)
        {
            selectedGradeItem = null;
            selectedLessonItem = null;
            gamePlayer = PlayerManager.player;
        }

        if (selectedGradeItem == null)
        {
            selectedGradeItem = GetGradeById(PlayerManager.player.GetString("SELECTED_GRADE", settings.gradeItems[0].id));
        }

        CreateGrades();

        DoGradeButtonsEffect();

        CreateLessons(selectedGradeItem);
    }

    public void OnGradeClick(Grade grade)
    {
        if (grade.gradeItem == selectedGradeItem)
        {
            return;
        }

        foreach (Grade g in grades)
        {
            g.backgroundImage.sprite = g.normalSprite;
        }

        selectedGradeItem = grade.gradeItem;
        selectedLessonItem = null;

        grade.backgroundImage.sprite = grade.selectedSprite;

        PlayerManager.player.SetString("SELECTED_GRADE", grade.gradeItem.id);

        if (!PlayerManager.autoSave) PlayerManager.Save();

        DoGradeButtonsEffect();

        MoveOut(() =>
        {

            CreateLessons(selectedGradeItem);

        });
    }

    public void OnLessonClick(Lesson lesson)
    {
        if (!lesson.interactable)
        {
            SM.PlayFX("lesson_decline");
            foreach (Lesson l in lessons)
            {
                if (l.lessonItem == selectedGradeItem.GetNextLesson())
                {
                    lessonContentRect.DOAnchorPos(new Vector2(-l.rectTransform.anchoredPosition.x, lessonContentRect.anchoredPosition.y), 0.3f)
                        .OnStart(() =>
                        {
                            lessonScrollRect.horizontal = false;
                        })
                        .OnComplete(() =>
                        {
                            lessonScrollRect.horizontal = true;
                        });
                }
            }
        }
        else if (selectedGradeItem.GetLessonIndex(lesson.lessonItem) >= freeLessons && !selectedGradeItem.GetPurchased())
        {

            selectedLessonItem = lesson.lessonItem;
            selectedGradeItem.SetSelectedLessonItem(selectedLessonItem);

            gradePurchase.Show();
        }
        else
        {

            selectedLessonItem = lesson.lessonItem;
            selectedGradeItem.SetSelectedLessonItem(selectedLessonItem);
            lesson.rectTransform.DOShakeScale(0.3f, 0.1f, 20, 10);

            LoadLesson();
        }
    }

    void LoadLesson()
    {
        //jen na zkousku//
        selectedGradeItem.SetLessonCompleted(selectedLessonItem, true);

        if (selectedLessonItem != null)
        {
            //Scene.LoadScene (selectedGradeItem.id + "_" + selectedLessonItem.id, loadAsync);
            Scene.LoadScene("Music School - Main", loadAsync, Color.black);
        }
    }

    public void OnLessonInfoClick(Lesson lesson)
    {
        lessonInfo.Show(null, "GRADE 1 - LESSON 1", "NOTE BLACK", "SOME TEXT HERE");
    }

    void MoveOut(System.Action finished)
    {
        if (lessons.Count == 0)
        {
            finished();
            return;
        }

        Lesson lesson = null;
        Tweener tweener = null;

        for (int i = 0; i < lessons.Count; i++)
        {
            lesson = lessons[i];
            lesson.destroying = true;
            //tweener = lesson.rectTransform.DOMove (outRect.position, 0.3f + i * 0.1f);
            tweener = lesson.rectTransform.DOScale(Vector3.zero, 0.3f);
        }

        tweener.OnComplete(() =>
        {
            foreach (Lesson l in lessons)
            {
                Destroy(l.gameObject);
            }

            lessons = new List<Lesson>();

            finished();
        });
    }

    void CreateGrades()
    {
        grades = new List<Grade>();
        for (int i = 0; i < settings.gradeItems.Count; i++)
        {
            GradeItem gradeItem = settings.gradeItems[i];

            Grade grade = (Instantiate(PREFAB_Grade, gradesRect, false) as GameObject).GetComponent<Grade>();

            grade.lessonManager = this;
            grade.gradeItem = gradeItem;
            grade.gradeText.text = (i + 1).ToString();

            Vector2 size = grade.rectTransform.GetSize();

            Vector2 andchoredPosition = new Vector2((-(settings.gradeItems.Count - 1) * (size.x + gradesDistance)) / 2f + i * (size.x + gradesDistance), 0);

            grade.rectTransform.anchoredPosition = andchoredPosition;

            if (selectedGradeItem.id == gradeItem.id)
            {
                grade.backgroundImage.sprite = grade.selectedSprite;
            }

            grades.Add(grade);
        }

        DoGradeButtonsEffect();
    }

    void DoGradeButtonsEffect()
    {
        for (int i = 0; i < grades.Count; i++)
        {
            Grade grade = grades[i];
            var shift = (selectedGradeItem.id == grade.gradeItem.id ? 20f : 0f);
            grade.rectTransform.DOAnchorPos(new Vector2(grade.rectTransform.anchoredPosition.x, shift), 0.2f);
        }
    }

    Tweener lessonContentRectTweener = null;
    Lesson nextLesson = null;
    Lesson selectedLesson = null;

    void CreateLessons(GradeItem gradeItem)
    {
        if (selectedLessonItem == null)
        {
            selectedLessonItem = gradeItem.GetSelectedLessonItem();
        }

        lessons = new List<Lesson>();

        lessonScrollRect.horizontal = false;
        lessonContentRect.position = inRect.position;

        Vector2 size = Vector2.zero;

        selectedLesson = null;
        nextLesson = null;

        for (int i = 0; i < gradeItem.lessonItems.Count; i++)
        {
            LessonItem lessonItem = gradeItem.lessonItems[i];

            Lesson lesson = (Instantiate(PREFAB_Lesson, lessonContentRect, false) as GameObject).GetComponent<Lesson>();

            bool completed = gradeItem.GetLessonCompleted(lessonItem);

            if (lessonItem == selectedLessonItem)
            {
                selectedLesson = lesson;
            }

            lesson.lessonManager = this;
            lesson.lessonItem = lessonItem;
            lesson.lessonImage.sprite = lessonItem.image;
            lesson.lessonText.text = "Lesson " + (i + 1).ToString();

            lesson.completed = completed;

            lesson.interactable = completed;

            if (i >= freeLessons && !gradeItem.GetPurchased())
            {
                lesson.locked = true;
            }
            else
            {
                lesson.locked = false;
            }

            size = lesson.rectTransform.GetSize();

            Vector2 anchoredPosition = new Vector2(i * (size.x + lessonsDistance), 0);

            lesson.rectTransform.anchoredPosition = anchoredPosition;

            lessons.Add(lesson);
        }

        for (int i = 0; i < lessons.Count; i++)
        {
            if (lessons[i] == selectedLesson)
            {
                int index = selectedLesson.completed ? i + 1 : i;
                if (index >= 0 && index < lessons.Count)
                {
                    nextLesson = lessons[index];
                    nextLesson.interactable = true;
                }
            }
        }

        selectedLesson.completedImage.rectTransform.localScale = Vector3.zero;

        lessonContentRect.SetSize(new Vector2((gradeItem.lessonItems.Count - 1) * size.x + (gradeItem.lessonItems.Count - 1) * lessonsDistance, size.y));

        if (lessonContentRectTweener != null)
        {
            lessonContentRectTweener.Kill();
        }

        lessonContentRectTweener = lessonContentRect.DOAnchorPos(new Vector2(-selectedLesson.rectTransform.anchoredPosition.x, lessonContentRect.anchoredPosition.y), 0.5f).OnComplete(() =>
        {
            if (selectedLesson.completed)
            {
                selectedLessonItem = nextLesson == null ? selectedLessonItem : nextLesson.lessonItem;
                selectedLesson.completed = true;
                selectedLesson.completedImage.rectTransform.DOScale(Vector3.one, 0.3f);
                SM.PlayFX("lesson_checkmark");

                DOVirtual.DelayedCall(1f, () =>
                {
                    if (nextLesson == null)
                    {
                        SM.PlayFX("grade_completed");
                        lessonScrollRect.horizontal = true;
                    }
                    else
                    {
                        lessonContentRectTweener = lessonContentRect.DOAnchorPos(new Vector2(-nextLesson.rectTransform.anchoredPosition.x, lessonContentRect.anchoredPosition.y), 0.5f)
                            .OnStart(() =>
                            {
                                SM.PlayFX("lesson_completed");
                            })
                            .OnComplete(() =>
                            {
                                lessonScrollRect.horizontal = true;
                            });
                    }
                });
            }
            else
            {
                lessonScrollRect.horizontal = true;
            }
        });
    }
}
