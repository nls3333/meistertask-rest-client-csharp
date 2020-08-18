using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace MeisterTask
{
    class Comment
    {
        public long Id { get; private set; }
        public long TaskId { get; private set; }
        public long PersonId { get; private set; }
        public string Text { get; private set; }
        public string TextHtml { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime Updated { get; private set; }

        public Comment(dynamic json)
        {
            Id = json.id;
            TaskId = json.task_id;
            PersonId = json.person_id;
            Text = json.text;
            TextHtml = json.text_html;
            Created = DateTime.Parse(json.created_at);
            Updated = DateTime.Parse(json.updated_at);
        }

        public override string ToString()
        {
            return "COMMENT: " + Json.Encode(this);
        }
    }
}
