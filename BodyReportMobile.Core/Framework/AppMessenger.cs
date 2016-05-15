using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace BodyReportMobile.Core.Framework
{
    [DebuggerStepThrough]
    public class AppMessenger
    {
        private readonly object _recipientLocker = new object();
        
        private class WeakAction
        {
            public object Recipient = null;
            public int Token = 0;
        }

        [DebuggerStepThrough]
        private class WeakFuntion<T, TResult> : WeakAction
        {
            public Func<T, TResult> Func = null;
            /// <summary>
            /// Executes the action. This only happens if the action's owner
            /// is still alive. The action's parameter is set to default(T).
            /// </summary>
            public void Execute()
            {
                Execute(default(T));
            }

            /// <summary>
            /// Executes the action. This only happens if the action's owner
            /// is still alive.
            /// </summary>
            /// <param name="parameter">A parameter to be passed to the action.</param>
            public TResult Execute(T parameter)
            {
                if (parameter != null)
                {
                    return Func(parameter);
                }
                else
                    return default(TResult);
            }
        }

        [DebuggerStepThrough]
        private class WeakAction<T> : WeakAction
        {
            public Action<T> Action = null;
            
            /// <summary>
            /// Executes the action. This only happens if the action's owner
            /// is still alive. The action's parameter is set to default(T).
            /// </summary>
            public void Execute()
            {
                Execute(default(T));
            }

            /// <summary>
            /// Executes the action. This only happens if the action's owner
            /// is still alive.
            /// </summary>
            /// <param name="parameter">A parameter to be passed to the action.</param>
            public void Execute(T parameter)
            {
                if (parameter != null)
                {
                    Action(parameter);
                }
            }
        }

        private static AppMessenger _instance = null;
		private static AppMessenger _instanceSender = null;
		private static AppMessenger _instanceListener = null;
        private Dictionary<Type, List<WeakAction>> _recipientsAction = new Dictionary<Type, List<WeakAction>>();

        public static AppMessenger AppInstance
        {
            get
            {
                if (_instance == null)
                    _instance = new AppMessenger();
                return _instance;
            }
        }

		public static AppMessenger Sender
		{
			get
			{
				if (_instanceSender == null)
					_instanceSender = new AppMessenger();
				return _instanceSender;
			}
		}
		public static AppMessenger Listener
		{
			get
			{
				if (_instanceListener == null)
					_instanceListener = new AppMessenger();
				return _instanceListener;
			}
		}

        /// <summary>
        /// Vide tous les abonnements au messenger
        /// </summary>
        public void ResetAll(object recipient = null)
        {
            lock (_recipientLocker)
            {
                if (recipient == null)
                {
                    _recipientsAction.Clear();
                }
                else
                {
                    foreach (KeyValuePair<Type, List<WeakAction>> keyVal in _recipientsAction)
                    {
                        keyVal.Value.RemoveAll(m => m.Recipient == recipient);
                    }
                }
            }
        }

        public void Register<TMessage, TResultMessage>(object recipient, Func<TMessage, TResultMessage> func, int token = 0)
        {
            lock (_recipientLocker)
            {
                if (!InternalRegister<TMessage>(recipient, token))
                {
                    var messageType = typeof(TMessage);
                    WeakAction<TMessage> weakAction = new WeakAction<TMessage>();
                    weakAction.Recipient = recipient;
                    weakAction.Token = token;
                    _recipientsAction[messageType].Add(weakAction);
                }
            }
        }

        public void Register<TMessage>(object recipient, Action<TMessage> action, int token = 0)
        {
            lock (_recipientLocker)
            {
                if (!InternalRegister<TMessage>(recipient, token))
                {
                    var messageType = typeof(TMessage);
                    WeakAction<TMessage> weakAction = new WeakAction<TMessage>();
                    weakAction.Recipient = recipient;
                    weakAction.Token = token;
                    weakAction.Action = action;
                    _recipientsAction[messageType].Add(weakAction);
                }
            }
        }

        /// <summary>
        /// Abonne un écouteur pour recevoir les messages reçus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="recipient"></param>
        /// <param name="action"></param>
        public bool InternalRegister<TMessage>(object recipient, int token = 0)
        {
            bool find = false;
            List<WeakAction> actionList;
            var messageType = typeof(TMessage);

            lock (_recipientLocker)
            {
                if (!_recipientsAction.ContainsKey(messageType))
                {
                    actionList = new List<WeakAction>();
                    _recipientsAction.Add(messageType, actionList);
                }
                else
                    actionList = _recipientsAction[messageType];

                foreach(WeakAction weakActionTmp in actionList)
                {
                    if(weakActionTmp.Recipient == recipient)
                    {
                        find = true;
                        break;
                    }
                }
            }
            return find;
        }

        /// <summary>
        /// Abonne un écouteur pour recevoir les messages reçus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="recipient"></param>
        /// <param name="action"></param>
        public void Unregister<TMessage>(object recipient, Action<TMessage> action = null, int token = 0)
        {
            List<WeakAction> actionList;
            var messageType = typeof(TMessage);
            lock (_recipientLocker)
            {
                if (!_recipientsAction.ContainsKey(messageType))
                    return;
                else
                    actionList = _recipientsAction[messageType];

                WeakAction<TMessage> weakAction = null;
                foreach (WeakAction weakActionTmp in actionList)
                {
                    if (weakActionTmp.Recipient == recipient &&
                        (action != null && ((WeakAction<TMessage>)weakActionTmp).Action is Action<TMessage>) &&
                        (token==0 && weakActionTmp.Token == 0) ||(token!=0 && weakActionTmp.Token == token))
                    {
                        weakAction = (WeakAction<TMessage>)weakActionTmp;
                        break;
                    }
                }

                if (weakAction != null)
                {
                    actionList.Remove(weakAction);
                }
            }
        }

        /// <summary>
        /// Abonne un écouteur pour recevoir les messages reçus
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="recipient"></param>
        /// <param name="action"></param>
        public void Send<TMessage>(TMessage message, int token = 0)
        {
            var messageType = typeof(TMessage);
            var listClone = _recipientsAction.Keys.Take(_recipientsAction.Count()).ToList();
            foreach (var type in listClone)
            {
                List<WeakAction> list = null;
                if (messageType == type)
                {
                    lock (_recipientLocker)
                    {
                        list = _recipientsAction[type].Take(_recipientsAction[type].Count()).ToList();
                    }

                    if(list != null)
                        SendToList(message, list, token);
                }
            }
        }
        private void SendToList<TMessage>(TMessage message, IEnumerable<WeakAction> weakActions, int token)
        {
            if (weakActions != null)
            {
                var list = weakActions.ToList();
                var listClone = list.Take(list.Count()).ToList();

                foreach (var itemTmp in listClone)
                {
                    var item = itemTmp as WeakAction<TMessage>;

                    if (item.Action != null
                        && item.Action.Target != null
                        && ((item.Token == 0 && token == 0) || item.Token != 0 && item.Token==token))
                    {
                        item.Execute(message);
                    }
                }
            }
        }
    }
}
