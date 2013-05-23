using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using InTwo.Common;

namespace InTwo.Behaviours
{
    /// <summary>
    /// Defines the possible actions when the Enter key is pressed.
    /// </summary>
    public enum EnterKeyAction
    {
        /// <summary>
        /// Focus moves to the next <see cref="TextBox"/> or <see cref="PasswordBox"/> control.
        /// </summary>
        MoveNext,

        /// <summary>
        /// The specified command is executed.
        /// </summary>
        ExecuteCommand
    }

    /// <summary>
    /// Provides a behavior for handling the Enter key press to move to the next control or to
    /// execute a command.
    /// </summary>
    public class EnterKeyHandler : Behavior<Control>
    {
        /// <summary>
        /// Identifies the <see cref="P:CommandString"/> attached property.
        /// </summary>
        public static readonly DependencyProperty CommandStringProperty =
            DependencyProperty.Register(
                "CommandString",
                typeof(string),
                typeof(EnterKeyHandler),
                null);

        /// <summary>
        /// Identifies the <see cref="P:EnterKeyAction"/> attached property.
        /// </summary>
        public static readonly DependencyProperty EnterKeyActionProperty =
            DependencyProperty.RegisterAttached(
                "EnterKeyAction",
                typeof(EnterKeyAction),
                typeof(EnterKeyHandler),
                new PropertyMetadata(EnterKeyAction.MoveNext));

        /// <summary>
        /// Gets or sets the name of the command to execute.
        /// </summary>
        /// <value>The name of the command to execute.</value>
        public string CommandString
        {
            get { return GetValue(CommandStringProperty) as string; }
            set { SetValue(CommandStringProperty, value); }
        }

        /// <summary>
        /// Gets or sets the enter key action.
        /// </summary>
        /// <value>The enter key action.</value>
        public EnterKeyAction EnterKeyAction
        {
            get { return (EnterKeyAction)GetValue(EnterKeyActionProperty); }
            set { SetValue(EnterKeyActionProperty, value); }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (null != this.AssociatedObject)
            {
                this.AssociatedObject.KeyDown += this.Control_KeyDown;
            }
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (null != this.AssociatedObject)
            {
                this.AssociatedObject.KeyDown -= this.Control_KeyDown;
            }
        }

        private void Control_KeyDown(object sender, KeyEventArgs args)
        {
            if ((Key.Enter == args.Key) ||
                (0x0A == args.PlatformKeyCode))
            {
                if (this.EnterKeyAction == EnterKeyAction.MoveNext)
                {
                    this.FocusNextTextOrPasswordBox();
                }
                else
                {
                    this.ExecuteAssociatedCommand();
                }
            }
        }

        private void ExecuteAssociatedCommand()
        {
            // Locate the named command in the associated objects data context, and then
            // execute that command.
            var commandName = this.CommandString;
            if (!string.IsNullOrEmpty(commandName))
            {
                var context = this.AssociatedObject.DataContext;
                if (null != context)
                {
                    var info = (from p in context.GetType().GetProperties()
                                where
                                    p.CanRead &&
                                    p.Name == commandName &&
                                    typeof(ICommand).IsAssignableFrom(p.PropertyType)
                                select p).SingleOrDefault();
                    if (null != info)
                    {
                        var command = info.GetGetMethod().Invoke(context, new object[0])
                            as ICommand;
                        if (null != command)
                        {
                            command.Execute(null);
                        }
                    }
                }
            }
        }

        private void FocusNextTextOrPasswordBox()
        {
            // Find the next TextBox or PasswordBox.
            var target = this.AssociatedObject
                             .ElementsAfterSelf().FirstOrDefault(d => d is TextBox || d is PasswordBox) as Control;
            if (null == target)
            {
                // Nothing after this one, so try predecessors so that we loop round.
                target = this.AssociatedObject
                             .ElementsBeforeSelf().FirstOrDefault(d => d is TextBox || d is PasswordBox) as Control;
            }

            if (null != target)
            {
                target.Focus();
            }
        }
    }
}