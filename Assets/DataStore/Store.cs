using DataStore.Quiz;

namespace DataStore
{
    public class Store
    {
        public ClassroomState ClassroomState;
        public QuizStore QuizStore;

        public static Store Default => m_Default ??= Empty();
        private static Store m_Default;

        private static Store Empty()
        {
            return new Store()
            {
                ClassroomState = new ClassroomState(),
                QuizStore = new QuizStore()
            };
        }

        public static void Dispose()
        {
            m_Default = null;
        }
    }
}