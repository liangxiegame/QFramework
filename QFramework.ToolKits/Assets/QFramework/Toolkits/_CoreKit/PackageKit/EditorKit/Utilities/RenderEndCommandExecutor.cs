/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;

namespace QFramework
{

    public class RenderEndCommandExecutor
    {
        // 全局的
        private static RenderEndCommandExecutor mGlobal = new RenderEndCommandExecutor();

        private Queue<System.Action> mPrivateCommands = new Queue<System.Action>();

        private Queue<System.Action> mCommands
        {
            get { return mPrivateCommands; }
        }

        public static void PushCommand(System.Action command)
        {
            mGlobal.Push(command);
        }

        public static void ExecuteCommand()
        {
            mGlobal.Execute();
        }

        public void Push(System.Action command)
        {
            mCommands.Enqueue(command);
        }

        public void Execute()
        {
            while (mCommands.Count > 0)
            {
                mCommands.Dequeue().Invoke();
            }
        }
    }
}