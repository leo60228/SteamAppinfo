using System;

namespace SteamAppinfo {
    public class WebException : Exception {
        public WebException(string message) : base(message) { }
        public WebException(string message, Exception inner) : base(message, inner) { }
        public WebException(Exception inner) : base(inner.Message, inner) { }
    }
}
