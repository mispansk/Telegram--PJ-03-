using System;
using System.Collections.Generic;
using System.Text;
using UtilityBot_11._2._4.Models;

namespace UtilityBot_11._2._4.Services
{
    public interface IStorage
    {
        /// <summary>
        /// Получение сессии пользователя по идентификатору
        /// </summary>
        Session GetSession(long chatId);
    }
}
