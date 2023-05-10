using System;

namespace OpenCvSharpWpfApp.Base.Command
{
    public class LambdaCommand : Command
    {
        private readonly Action<object> _Execute;
        private readonly Func<object, bool> _CanExecute;

        public LambdaCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _Execute = execute;
            _CanExecute = canExecute;
        }

        public override void Execute(object parameter)
        {
            if (CanExecute(parameter))
                _Execute(parameter);
        }

        public override bool CanExecute(object parameter) => _CanExecute?.Invoke(parameter) ?? true;
    }
}
