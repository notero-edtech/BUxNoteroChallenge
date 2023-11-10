namespace BU.RRTT.QuizExample.Scripts
{
    public enum QuizControllerType
    {
        FLOW,
        RESULT
    }

    public enum QuizStates
    {
        STARTER = 0,
        LOADING = 1,
        PREPARING = 2,
        COUNTIN = 3,
        QUESTION = 4,
        PRERESULT = 5,
        END = 6
    }

    public enum QuizModes
    {
        POPQUIZ,
        PRETEST,
        POSTTEST
    }
}