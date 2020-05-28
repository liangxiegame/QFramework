using System;

namespace QFramework
{
    public interface IGameplayKitObject : IDisposable
    {
        bool EnableUpdate { get; set; }

        void GameStart();
        void GamePause();
        void GameResume();
        void GameStop();
    }

    public class GameplayKitObject : IGameplayKitObject
    {
        private bool mEnableUpdate = false;

        public virtual bool EnableUpdate
        {
            get { return mEnableUpdate; }
            set
            {
                if (value == mEnableUpdate)
                    return;

                if (value)
                    RegisterUpdate();
                else
                    RegisterFixedUpdate();
                mEnableUpdate = value;
            }
        }

        protected virtual void RegisterUpdate()
        {
            GameplayKit.RegisterOnUpdateEvent(OnUpdate);

        }

        protected virtual void UnregisterUpdate()
        {

        }

        private bool mEnableFixedUpdate = false;

        public virtual bool EnableFixedUpdate
        {
            get { return mEnableFixedUpdate; }
            set
            {
                if (value == mEnableFixedUpdate)
                    return;

                if (value)
                    RegisterFixedUpdate();
                else
                    UnregisterFixedUpdate();

                mEnableFixedUpdate = value;
            }
        }



        protected virtual void RegisterFixedUpdate()
        {
            GameplayKit.RegisterOnFixedUpdateEvent(OnFixedUpdate);

        }

        protected virtual void UnregisterFixedUpdate()
        {
            GameplayKit.UnregisterOnFixedUpdateEvent(OnFixedUpdate);

        }

        void IGameplayKitObject.GameStart()
        {
            OnGameStart();
        }

        void IGameplayKitObject.GamePause()
        {
            OnGamePause();
        }

        void IGameplayKitObject.GameResume()
        {
            OnGameResume();
        }

        void IGameplayKitObject.GameStop()
        {
            OnGameStop();
        }

        protected virtual void OnGameStart()
        {

        }

        protected virtual void OnGamePause()
        {

        }

        protected virtual void OnGameResume()
        {

        }

        protected virtual void OnGameStop()
        {

        }


        protected virtual void OnUpdate(float gameTime, float deltaTime)
        {

        }

        protected virtual void OnFixedUpdate(float gameTime, float fixedDeltaTime)
        {

        }

        public GameplayKitObject()
        {
            GameplayKit.RegisterGameplayKitObject(this);
        }

        public void Dispose()
        {
            EnableUpdate = false;

            EnableFixedUpdate = false;
            
            GameplayKit.UnregisterGameplayKitObject(this);
        }
    }
}