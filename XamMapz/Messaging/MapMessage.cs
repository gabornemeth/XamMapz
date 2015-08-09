using System;
using System.Collections.Generic;
using System.Text;

namespace XamMapz.Messaging
{
    /// <summary>
    /// Base class for messages used between MapEx and custom renderer
    /// </summary>
    public class MapMessage
    {
        /// <summary>
        /// Identifier for messages sent by MapEx to MapExRenderer
        /// </summary>
        public const string Message = "MapExMessage";

        /// <summary>
        /// Identifier for messages sent by MapExRenderer to MapEx
        /// </summary>
        public const string RendererMessage = "MapExRendererMessage";
    }
}
