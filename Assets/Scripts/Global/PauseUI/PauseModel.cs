namespace MVC
{
    public class PauseModel
    {
        public bool IsPaused { get; private set; }

        public void SetPaused(bool paused)
        {
            IsPaused = paused;
            UnityEngine.Time.timeScale = paused ? 0f : 1f;
        }
    }
}