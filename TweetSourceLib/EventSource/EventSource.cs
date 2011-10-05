﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TweetSource.EventSource
{
    public abstract class EventSource<T> where T : EventArgs
    {
        public event EventHandler<T> DataArrived;

        public event EventHandler<T> SourceDown;

        public event EventHandler<T> SourceUp;

        protected void FireDataArrived(T data)
        {
            if (DataArrived != null)
                DataArrived(this, data);
        }

        protected void FireSourceDown(T data)
        {
            if (SourceDown != null)
                SourceDown(this, data);
        }

        protected void FireSourceUp(T data)
        {
            if (SourceUp != null)
                SourceUp(this, data);
        }
    }
}