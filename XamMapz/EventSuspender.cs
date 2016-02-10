//
// EventSuspender.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;
using System.Collections.Generic;
using System.Text;

namespace XamMapz
{
    /// <summary>
    /// Helper class to suspend event handling
    /// </summary>
    public class EventSuspender
    {
        protected int _count;

        /// <summary>
        /// Whether event handling is suspended
        /// </summary>
        public bool IsSuspended
        {
            get
            {
                return _count != 0;
            }
        }

        /// <summary>
        /// Suspend event handling
        /// </summary>
        public void Suspend()
        {
            this._count++;;
        }

        /// <summary>
        /// Allow Event handling
        /// </summary>
        public void Allow()
        {
            this._count--;
        }
    }
}
