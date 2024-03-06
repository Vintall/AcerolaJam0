namespace InternalAssets.Scripts
{
    public interface INarrativeClip
    {
        void OnStart();
        void OnEnd();
        void OnUpdate();
        INarrativeClip NextClip { get; }
        INarrativeClip PrevClip { get; }
    }
}