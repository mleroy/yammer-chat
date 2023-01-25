using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.Parsers
{
    public class ReferenceKey : IEquatable<ReferenceKey>
    {
        public static ReferenceKey ForUser(long id) { return new ReferenceKey("user", id); }
        public static ReferenceKey ForConversation(long id) { return new ReferenceKey("conversation", id); }
        public static ReferenceKey ForThread(long id) { return new ReferenceKey("thread", id); }

        public string Type { get; private set; }
        public long Id { get; private set; }

        private readonly int hashCode;

        public ReferenceKey(string type, long id)
        {
            this.Type = type;
            this.Id = id;

            this.hashCode = this.Type.GetHashCode() ^ this.Id.GetHashCode();
        }

        public bool Equals(ReferenceKey other)
        {
            return object.Equals(this.Type, other.Type) && object.Equals(this.Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(ReferenceKey)) return false;
            return Equals((ReferenceKey)obj);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}
