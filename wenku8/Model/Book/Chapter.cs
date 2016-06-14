using System;
using Net.Astropenguin.DataModel;

namespace wenku8.Model.Book
{
    using Resources;
    using Settings;
    using Text;

	class Chapter : ActiveData
	{
		public string vid { get; set; }
		public string cid { get; set; }
		public string aid { get; set; }

        virtual protected string VolRoot { get; set; }

		virtual public string ChapterPath
        {
            get { return VolRoot + vid + "/" + cid + ".txt"; }
        }

		virtual public string IllustrationPath
		{
			get { return VolRoot + vid + "/" + cid + ".ils"; }
		}

        public bool HasIllustrations
        {
            get { return Shared.Storage.FileExists( IllustrationPath ); }
        }

		public bool IsCached
        {
            get { return Shared.Storage.FileExists( ChapterPath ); }
        }

		public string ChapterTitle { get; set; }

        public Chapter( string Name, string BookId, string VolumeId, string ChapterId )
        {
            ChapterTitle = Name;
            vid = VolumeId;
            cid = ChapterId;
            aid = BookId;

            VolRoot = FileLinks.GetVolumeRoot( aid );
        }

        public Paragraph[] GetParagraphs()
        {
            string[] Lines = Shared.Storage.GetString( ChapterPath )
                .Split( new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries );

            int l = Lines.Length;
            Paragraph[] Paragraphs = new Paragraph[ Lines.Length ];

            for ( int i = 0; i < l; i++ )
            {
                Paragraph p = new Paragraph( Lines[ i ] );
                Paragraphs[ i ] = p;
            }

            return Paragraphs;
        }

        public void UpdateStatus()
        {
            NotifyChanged( "IsCached", "HasIllustrations" );
        }

        public override bool Equals( object obj )
        {
            Chapter C = obj as Chapter;
            if ( C != null )
            {
                return C.cid == this.cid && C.aid == this.aid;
            }
            return base.Equals( obj );
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    class LocalChapter : Chapter
    {
        protected override string VolRoot
        {
            get { return FileLinks.ROOT_LOCAL_VOL + aid + "/"; }
        }

        public LocalChapter( string Name, string BookId, string VolumeId, string ChapterId )
            : base( Name, BookId, VolumeId, ChapterId )
        {
        }
    }
}
