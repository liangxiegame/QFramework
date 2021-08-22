using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace QFramework.ILKitDemo.Tetris
{
    public class Shape
    {
        private ILControllerNode<Tetris> mControllerNode = ILControllerNode<Tetris>.Allocate();
        
        private Transform pivot;
        
        private GameManager gameManager;

        private bool isPause = false;

        private bool isSpeedup = false;

        private float timer = 0;

        private float stepTime = 0.8f;

        private int multiple = 15;

        public Transform transform;

        public GameObject gameObject;

        public Shape(Transform transform)
        {
            this.transform = transform;
            this.gameObject = transform.gameObject;
            Awake();

            Observable.EveryUpdate().Subscribe(_ => Update());
            
            gameObject.OnDestroyAsObservable()
                .Subscribe(_ =>
                {
                    mControllerNode.Recycle2Cache();
                    mControllerNode = null;
                });
        }
        
        private void Awake()
        {
            pivot = transform.Find("Pivot");
        }

        private void Update()
        {
            if (isPause) return;
            timer += Time.deltaTime;
            if (timer > stepTime)
            {
                timer = 0;
                Fall();
            }

            InputControl();
        }

        public void Init(Color color, GameManager gameManager)
        {
            foreach (Transform t in transform)
            {
                if (t.tag == "Block")
                {
                    t.GetComponent<SpriteRenderer>().color = color;
                }
            }

            this.gameManager = gameManager;
        }

        private void Fall()
        {
            var tetrisPanel = ILUIKit.GetPanel<UITetrisPanel>();
            
            Vector3 pos = transform.position;
            pos.y -= 1;
            transform.position = pos;
            if (tetrisPanel.Model.IsValidMapPosition(transform) == false)
            {
                pos.y += 1;
                transform.position = pos;
                isPause = true;
                bool isLineclear = tetrisPanel.Model.PlaceShape(this.transform);
                if (isLineclear) mControllerNode.SendCommand<PlayLineClearSoundCommand>();
                gameManager.FallDown();
                return;
            }

            mControllerNode.SendCommand<PlayDropSoundCommand>();
        }

        private void InputControl()
        {
            var tetrisPanel = ILUIKit.GetPanel<UITetrisPanel>();

            //if (isSpeedup) return;
            float h = 0;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                h = -1;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                h = 1;
            }

            if (h != 0)
            {
                Vector3 pos = transform.position;
                pos.x += h;
                transform.position = pos;
                if (tetrisPanel.Model.IsValidMapPosition(this.transform) == false)
                {
                    pos.x -= h;
                    transform.position = pos;
                }
                else
                {
                    mControllerNode.SendCommand<PlayControllerSoundCommand>();
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                transform.RotateAround(pivot.position, Vector3.forward, -90);
                if (tetrisPanel.Model.IsValidMapPosition(this.transform) == false)
                {
                    transform.RotateAround(pivot.position, Vector3.forward, 90);
                }
                else
                {
                    mControllerNode.SendCommand<PlayControllerSoundCommand>();
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                isSpeedup = true;
                stepTime /= multiple;
            }
        }

        public void Pause()
        {
            isPause = true;
        }

        public void Resume()
        {
            isPause = false;
        }
    }
}