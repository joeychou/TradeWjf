using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace wjf
{
    public class WebAsyncResult : IAsyncResult
    {
        private AsyncCallback _callback;

        public WebAsyncResult(AsyncCallback cb, HttpContext context)
        {
            Context = context;
            _callback = cb;
        }

        //当异步完成时调用该方法
        public void SetComplete()
        {
            IsCompleted = true;
            if (_callback != null)
            {
                _callback(this);
            }
        }

        public HttpContext Context
        {
            get;
            private set;
        }

        public object AsyncState
        {
            get { return null; }
        }

        //由于ASP.NET不会等待WEB异步方法，所以不使用此对象
        public WaitHandle AsyncWaitHandle
        {
            get { throw new NotImplementedException(); }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get;
            private set;
        }
    }
}