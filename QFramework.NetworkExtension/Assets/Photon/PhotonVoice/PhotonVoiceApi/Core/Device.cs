#if (UNITY_IOS && !UNITY_EDITOR) || __IOS__
#define DLL_IMPORT_INTERNAL
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Photon.Voice
{
    public struct DeviceInfo
    {
        // used internally for Default property creation
        private DeviceInfo(bool isDefault, int idInt, string idString, string name)
        {
            IsDefault = isDefault;
            IDInt = idInt;
            IDString = idString;
            Name = name;
            useStringID = false;
        }

        // numeric id
        public DeviceInfo(int id, string name)
        {
            IsDefault = false;
            IDInt = id;
            IDString = "";
            Name = name;
            useStringID = false;
        }

        // string id
        public DeviceInfo(string id, string name)
        {
            IsDefault = false;
            IDInt = 0;
            IDString = id;
            Name = name;
            useStringID = true;
        }

        // name is id (Unity Microphone and WebCamTexture APIs)
        public DeviceInfo(string name)
        {
            IsDefault = false;
            IDInt = 0;
            IDString = name;
            Name = name;
            useStringID = true;
        }

        public bool IsDefault { get; private set; }
        public int IDInt { get; private set; }
        public string IDString { get; private set; }
        public string Name { get; private set; }
        private bool useStringID;

        public static bool operator ==(DeviceInfo d1, DeviceInfo d2)
        {
            return d1.Equals(d2);
        }
        public static bool operator !=(DeviceInfo d1, DeviceInfo d2)
        {
            return !d1.Equals(d2);
        }

        // trivial implementation to avoid warnings CS0660 and CS0661 about missing overrides when == and != defined 
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (useStringID)
            {
                return (Name == null ? "" : Name) + (IDString == null || IDString == Name ? "" : " (" + IDString.Substring(0, Math.Min(10, IDString.Length)) + ")");
            }
            else
            {
                return string.Format("{0} ({1})", Name, IDInt);
            }
        }

        // default device id may differ on different platform, use this platform value instead of Default.Int
        public static readonly DeviceInfo Default = new DeviceInfo(true, -128, "", "[Default]");
    }

    public interface IDeviceEnumerator : IDisposable, IEnumerable<DeviceInfo>
    {
        bool IsSupported { get; }
        void Refresh();
        string Error { get; }
    }

    public abstract class DeviceEnumeratorBase : IDeviceEnumerator
    {
        protected List<DeviceInfo> devices = new List<DeviceInfo>();
        protected ILogger logger;
        public DeviceEnumeratorBase(ILogger logger)
        {
            this.logger = logger;
        }
        public virtual bool IsSupported => true;
        public virtual string Error { get; protected set; }

        public IEnumerator<DeviceInfo> GetEnumerator()
        {
            return devices.GetEnumerator();
        }

        public abstract void Refresh();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public abstract void Dispose();
    }

    internal class DeviceEnumeratorNotSupported : DeviceEnumeratorBase
    {
        public override bool IsSupported => false;

        string message;
        public DeviceEnumeratorNotSupported(ILogger logger, string message) : base(logger)
        {
            this.message = message;
        }

        public override void Refresh()
        {
        }

        public override string Error { get { return message; } }

        public override void Dispose()
        {
        }
    }

    internal class AudioInEnumeratorNotSupported : DeviceEnumeratorNotSupported
    {
        public AudioInEnumeratorNotSupported(ILogger logger)
            : base(logger, "Current platform is not supported by audio input DeviceEnumerator.")
        {
        }
    }

    internal class VideoInEnumeratorNotSupported : DeviceEnumeratorNotSupported
    {
        public VideoInEnumeratorNotSupported(ILogger logger)
            : base(logger, "Current platform is not supported by video capture DeviceEnumerator.")
        {
        }
    }

    public interface IAudioInChangeNotifier : IDisposable
    {
        bool IsSupported { get; }
        string Error { get; }
    }

    public class AudioInChangeNotifierNotSupported : IAudioInChangeNotifier
    {
        public bool IsSupported => false;

        public AudioInChangeNotifierNotSupported(Action callback, ILogger logger)
        {
        }

        public string Error { get { return "Current platform " + "is not supported by AudioInChangeNotifier."; } }

        public void Dispose()
        {
        }
    }
}
