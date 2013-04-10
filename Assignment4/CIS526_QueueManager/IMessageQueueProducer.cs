﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CIS526_QueueManager
{
    /// <summary>
    /// An interface for the Producer to push messages onto a queue.
    /// </summary>
    /// <typeparam name="T">Model type.</typeparam>
    public interface IMessageQueueProducer
        : IDisposable
    {
        /// <summary>
        /// Retives all models from the data base.
        /// </summary>
        /// <returns></returns>
        IList<object> GetAll();

        /// <summary>
        /// Updates all models contained in the list.
        /// </summary>
        /// <param name="data"></param>
        void Update(IList<object> data);

        /// <summary>
        /// Creates all models in the database.
        /// </summary>
        /// <param name="data"></param>
        void Create(IList<object> data);

        /// <summary>
        /// Removes all models contains in the list from the database.
        /// </summary>
        /// <param name="data"></param>
        void Remove(IList<object> data);
    }
}