namespace Photon.Voice.Unity.UtilityScripts
{

    public class MicAmplifierFloat : IProcessor<float>
    {
        public float AmplificationFactor { get; set; }
        public float BoostValue { get; set; }

        public float MaxBefore { get; private set; }
        public float MaxAfter { get; private set; }
        //public float MinBefore { get; private set; }
        //public float MinAfter { get; private set; }
        //public float AvgBefore { get; private set; }
        //public float AvgAfter { get; private set; }

        public bool Disabled { get; set; }

        public MicAmplifierFloat(float amplificationFactor, float boostValue)
        {
            this.AmplificationFactor = amplificationFactor;
            this.BoostValue = boostValue;
        }

        public float[] Process(float[] buf)
        {
            if (this.Disabled)
            {
                return buf;
            }
            for (int i = 0; i < buf.Length; i++)
            {
                float before = buf[i];
                buf[i] *= this.AmplificationFactor;
                buf[i] += this.BoostValue;
                if (this.MaxBefore < before)
                {
                    this.MaxBefore = before;
                    this.MaxAfter = buf[i];
                }
            }
            return buf;
        }

        public void Dispose()
        {
        }
    }
}