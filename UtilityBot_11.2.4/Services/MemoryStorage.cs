using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using UtilityBot_11._2._4.Models;

namespace UtilityBot_11._2._4.Services
{
    public class MemoryStorage: IStorage
    {
        /// <summary>
        /// Хранилище сессий
        /// </summary>
        private readonly ConcurrentDictionary<long, Session> _sessions;

        public MemoryStorage()
        {
            _sessions = new ConcurrentDictionary<long, Session>();
        }

        public Session GetSession(long chatId)
        {
            // Возвращаем сессию по ключу, если она существует
            if (_sessions.ContainsKey(chatId))
                return _sessions[chatId];

            // Создаем и возвращаем новую, если такой не было
            var newSession = new Session() { TaskCode = "1" };
            _sessions.TryAdd(chatId, newSession);
            return newSession;
        }
    }
}
