using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace wenku8.Model.Comments
{
    sealed class HSComment : Comment 
    {
        public static HSComment ActiveInstance;

        private bool _selected = false;

        public IEnumerable<HSComment> Replies { get; set; }
        public bool Folded { get; set; }
        public bool Encrypted { get; set; }
        public bool DecFailed { get; set; }
        public string Id { get; set; }
        public int Level { get; set; }
        public bool Selected
        {
            get { return _selected; }
            private set
            {
                _selected = value;
                NotifyChanged( "Selected" );
            }
        }

        // Required for Activator
        public HSComment( JsonObject Def ) :this( Def, 0 ) { } 

        public HSComment( JsonObject Def, int Level )
        {
            Folded = false;
            this.Level = Level;

            JsonObject JAuthor = Def.GetNamedObject( "author" );
            Username = JAuthor.GetNamedString( "name" );
            UserId = JAuthor.GetNamedString( "_id" );

            Id = Def.GetNamedString( "_id" );
            Title = Def.GetNamedString( "content" );
            TimeStamp = DateTime.Parse( Def.GetNamedString( "date_created" ) );

            Encrypted = Def.GetNamedBoolean( "enc" );

            JsonArray JReplies = Def.GetNamedArray( "replies" );

            List<HSComment> Stacks = new List<HSComment>();
            foreach( JsonValue JValue in JReplies )
            {
                if( JValue.ValueType == JsonValueType.Object )
                {
                    Stacks.Add( new HSComment( JValue.GetObject(), Level + 1 ) );
                }
                else
                {
                    Stacks.Add( new HSComment( JValue.GetString(), Level + 1 ) );
                }
            }

            Replies = Stacks.OrderByDescending( k => k.TimeStamp );
        }

        public HSComment( string Id, int Level = 0 )
        {
            Folded = true;

            this.Id = Id;
            this.Level = Level;
        }

        public void MarkSelect()
        {
            if( ActiveInstance != null )
                ActiveInstance.Selected = false;

            ActiveInstance = this;
            ActiveInstance.Selected = true;
        }
    }
}
