using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace MeisterTask
{
    class Person
    {
        public long Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string FullName { get { return FirstName + " " + LastName; } }
        public string Email { get; private set; }
        public string AvatarUrl { get; private set; }
        public string AvatarThumbnailUrl { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime Updated { get; private set; }

        public Person(dynamic json)
        {
            Id = json.id;
            FirstName = json.firstname;
            LastName = json.lastname;
            Email = json.email;
            AvatarUrl = json.avatar;
            AvatarThumbnailUrl = json.avatar_thumb;
            Created = DateTime.Parse(json.created_at);
            Updated = DateTime.Parse(json.updated_at);
        }

        public override string ToString()
        {
            return "PERSON: " + Json.Encode(this);
        }
    }
}
