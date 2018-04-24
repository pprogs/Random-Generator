using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CameraTest
{
    /// <summary>
    /// 
    /// </summary>
    public static class EventAttachedProperty
    {
        #region Closed event
        public static ICommand GetClosedCommand(DependencyObject obj) => (ICommand)obj.GetValue(ClosedCommandProperty);       
        public static void SetClosedCommand(DependencyObject obj, ICommand value) => obj.SetValue(ClosedCommandProperty, value);

        public static readonly DependencyProperty ClosedCommandProperty = DependencyProperty.RegisterAttached("ClosedCommand", typeof(ICommand), typeof(EventAttachedProperty), new PropertyMetadata(new PropertyChangedCallback(CommandProperyChanged)));

        #endregion

        #region Closing event

        public static ICommand GetClosingCommand(DependencyObject obj) => (ICommand)obj.GetValue(ClosingCommandProperty);

        public static void SetClosingCommand(DependencyObject obj, ICommand value) => obj.SetValue(ClosedCommandProperty, value);

        public static readonly DependencyProperty ClosingCommandProperty = DependencyProperty.RegisterAttached("ClosingCommand", typeof(ICommand), typeof(EventAttachedProperty), new PropertyMetadata(new PropertyChangedCallback(CommandProperyChanged)));

        #endregion

        #region Base

        static readonly Dictionary<string, EventRegEntry> _events = new Dictionary<string, EventRegEntry>();

        static void CommandProperyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string eventName = e.Property.Name.Replace("Command", "");
            EventInfo ei = d.GetType().GetEvent(eventName);

            if (ei != null)
            {
                var reg = new EventRegEntry()
                {
                    Command = (ICommand)e.NewValue,
                    EventName = eventName,
                    PropName = e.Property.Name,
                };

                MethodInfo mi = reg.GetType().GetMethod("ExecuteCommand", BindingFlags.Instance | BindingFlags.NonPublic);
                var handler = Delegate.CreateDelegate(ei.EventHandlerType, reg, mi);
                ei.AddEventHandler(d, handler);

                _events[e.Property.Name] = reg;
            }
        }

        #endregion
    }
    
    /// <summary>
    /// 
    /// </summary>
    class EventRegEntry
    {
        public string EventName { get; set; }
        public string PropName { get; set; }        
        public ICommand Command { get; set; }

        void ExecuteCommand(object sender, EventArgs e)
        {
            DependencyObject d = sender as DependencyObject;
            if (d == null)
                return;

            if (Command != null)
            {
                if (Command.CanExecute(e))
                    Command.Execute(e);
            }
        }
    }
}
