﻿using Abot.Poco;
using log4net;
using System;
using System.Collections.Generic;

namespace Abot.Core
{
    public interface IScheduler
    {
        /// <summary>
        /// Count of remaining items that are currently scheduled
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Schedule the param
        /// </summary>
        void Add(PageToCrawl page);

        /// <summary>
        /// Retrieve the next scheduled type
        /// </summary>
        PageToCrawl GetNext();
    }

    public class FifoScheduler : IScheduler
    {
        static ILog _logger = LogManager.GetLogger(typeof(FifoScheduler).FullName);
        Queue<PageToCrawl> _pagesToCrawl = new Queue<PageToCrawl>();
        Object locker = new Object();

        public int Count
        {
            get
            {
                lock (locker)
                {
                    return _pagesToCrawl.Count;
                }
            }
        }

        public void Add(PageToCrawl page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            _logger.DebugFormat("Scheduling for crawl [{0}]", page.Uri.AbsoluteUri);

            lock (locker)
            {
                _pagesToCrawl.Enqueue(page);
            }
        }

        public PageToCrawl GetNext()
        {
            PageToCrawl nextItem = null;
            lock (locker)
            {
                nextItem = _pagesToCrawl.Dequeue();
            }

            return nextItem;
        }
    }
}