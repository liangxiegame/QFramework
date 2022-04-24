namespace Photon.Voice.Unity.UtilityScripts
{
    public class MicAmplifierShort : IProcessor<short>
    {
        public short AmplificationFactor { get; set; }
        public short BoostValue { get; set; }

        public short MaxBefore { get; private set; }
        public short MaxAfter { get; private set; }
        //public short MinBefore { get; private set; }
        //public short MinAfter { get; private set; }
        //public short AvgBefore { get; private set; }
        //public short AvgAfter { get; private set; }

        public bool Disabled { get; set; }


        public MicAmplifierShort(short amplificationFactor, short boostValue)
        {
            this.AmplificationFactor = amplificationFactor;
            this.BoostValue = boostValue;
        }

        public short[] Process(short[] buf)
        {
            if (this.Disabled)
            {
                return buf;
            }
            for (int i = 0; i < buf.Length; i++)
            {
                short before = buf[i];
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