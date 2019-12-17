/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;

namespace BindKit.Commands
{
    public class RelayCommand : CommandBase
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public RelayCommand(Action execute) : this(execute, null)
        {
        }

        public RelayCommand(Action execute, bool keepStrongRef) : this(execute, null, keepStrongRef)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute, bool keepStrongRef = false)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = keepStrongRef ? execute : execute.AsWeak();

            if (canExecute != null)
                this.canExecute = keepStrongRef ? canExecute : canExecute.AsWeak();
        }

        public override bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute();
        }

        public override void Execute(object parameter)
        {
            if (this.CanExecute(parameter) && this.execute != null)
                this.execute();
        }
    }

    public class RelayCommand<T> : CommandBase
    {
        private readonly Action<T> execute;
        private readonly Func<bool> canExecute;

        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, bool keepStrongRef) : this(execute, null, keepStrongRef)
        {
        }

        public RelayCommand(Action<T> execute, Func<bool> canExecute, bool keepStrongRef = false)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = keepStrongRef ? execute : execute.AsWeak();

            if (canExecute != null)
                this.canExecute = keepStrongRef ? canExecute : canExecute.AsWeak();
        }

        public override bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute();
        }

        public override void Execute(object parameter)
        {
            if (this.CanExecute(parameter) && this.execute != null)
                this.execute((T)parameter);
        }
    }
}

