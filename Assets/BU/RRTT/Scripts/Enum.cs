namespace BU.RRTT.Scripts
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
        POP_QUIZ,
        PRE_TEST,
        POST_TEST
    }
}