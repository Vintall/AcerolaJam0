namespace InternalAssets.Scripts.NarrativeClips.Act1
{
    public class NarrativeClip_01_01_StandUp : INarrativeClip
    {
        private INarrativeClip _nextClip;
        private INarrativeClip _prevClip;

        public void OnStart()
        {
            throw new System.NotImplementedException();
        }

        public void OnEnd()
        {
            throw new System.NotImplementedException();
        }

        public void OnUpdate()
        {
            throw new System.NotImplementedException();
        }

        public INarrativeClip NextClip => _nextClip;

        public INarrativeClip PrevClip => _prevClip;
    }
}