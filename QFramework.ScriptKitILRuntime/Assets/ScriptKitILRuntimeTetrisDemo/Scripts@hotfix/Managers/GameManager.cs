using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace QFramework.ILKitDemo.Tetris
{
    public class GameManager
    {
        private bool isPause = true; //游戏是否暂停

        private Shape currentShape = null;

        private Transform blockHolder;

        public GameObject[] shapes;

        public Color[] colors;

        public Transform transform;

        ResLoader mResLoader = ResLoader.Allocate();

        private ILControllerNode<Tetris> mControllerNode = ILControllerNode<Tetris>.Allocate();

        public GameManager(Transform parentTransform)
        {

            transform = parentTransform.Find("Ctrl");

            Observable.EveryUpdate().Subscribe(_ => Update()).AddTo(transform.gameObject);

            transform.OnDestroyAsObservable()
                .Subscribe(_ =>
                {
                    mControllerNode.Recycle2Cache();
                    mControllerNode = null;
                });

            blockHolder = transform.Find("BlockHolder");


            shapes = new GameObject[7];

            mResLoader.LoadSync("Shape-1").LogInfo();

            shapes[0] = mResLoader.LoadSync<GameObject>("Shape-1");
            shapes[1] = mResLoader.LoadSync<GameObject>("Shape-2");
            shapes[2] = mResLoader.LoadSync<GameObject>("Shape-3");
            shapes[3] = mResLoader.LoadSync<GameObject>("Shape-4");
            shapes[4] = mResLoader.LoadSync<GameObject>("Shape-5");
            shapes[5] = mResLoader.LoadSync<GameObject>("Shape-6");
            shapes[6] = mResLoader.LoadSync<GameObject>("Shape-7");


            colors = new Color[14];
            ColorUtility.TryParseHtmlString("#F3DE75", out colors[0]);
            ColorUtility.TryParseHtmlString("#4DD5B0", out colors[1]);
            ColorUtility.TryParseHtmlString("#ED954A", out colors[2]);
            ColorUtility.TryParseHtmlString("#DC6555", out colors[3]);
            ColorUtility.TryParseHtmlString("#98DC55", out colors[4]);
            ColorUtility.TryParseHtmlString("#545454", out colors[5]);
            ColorUtility.TryParseHtmlString("#5CBEE4", out colors[6]);
            ColorUtility.TryParseHtmlString("#4DD5B0", out colors[7]);
            ColorUtility.TryParseHtmlString("#ED954A", out colors[8]);
            ColorUtility.TryParseHtmlString("#98DC55", out colors[9]);
            ColorUtility.TryParseHtmlString("#DC6555", out colors[10]);
            ColorUtility.TryParseHtmlString("#5CBEE4", out colors[11]);
            ColorUtility.TryParseHtmlString("#43BA9A", out colors[12]);
            ColorUtility.TryParseHtmlString("#59CB86", out colors[13]);
        }


        // Update is called once per frame
        void Update()
        {
            if (isPause) return;
            if (currentShape == null)
            {
                SpawnShape();
            }
        }

        public void ClearShape()
        {
            if (currentShape != null)
            {
                Object.Destroy(currentShape.gameObject);
                currentShape = null;
            }
        }

        public void StartGame()
        {
            isPause = false;
            if (currentShape != null)
                currentShape.Resume();
        }

        public void PauseGame()
        {
            isPause = true;
            if (currentShape != null)
                currentShape.Pause();
        }

        void SpawnShape()
        {
            int index = Random.Range(0, shapes.Length);
            int indexColor = Random.Range(0, colors.Length);
            currentShape = new Shape(GameObject.Instantiate(shapes[index]).transform);
            currentShape.transform.parent = blockHolder;
            currentShape.Init(colors[indexColor], this);
        }

        //方块落下来了
        public void FallDown()
        {
            currentShape = null;
            
            var tetrisPanel = ILUIKit.GetPanel<UITetrisPanel>();

            var gameModel = mControllerNode.GetModel<IGameModel>();
            if (gameModel.isDataUpdate)
            {
                    
                tetrisPanel.UpdateGameUI(gameModel.Score.Value, gameModel.HighScore.Value);
            }

            foreach (Transform t in blockHolder)
            {
                if (t.childCount <= 1)
                {
                    Object.Destroy(t.gameObject);
                }
            }

            if (gameModel.IsGameOver())
            {
                PauseGame();
                tetrisPanel.ShowGameOverUI(gameModel.Score.Value);
            }
        }
    }
}