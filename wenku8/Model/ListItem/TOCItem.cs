namespace wenku8.Model.ListItem
{
    using Book;
    class TOCItem : TreeItem 
    {
        private Volume Vol;
        private Chapter Ch;

        public TOCItem( Volume V )
            :base( V.VolumeTitle, 0 )
        {
            Vol = V;
        }

        public TOCItem( Chapter C )
            :base( C.ChapterTitle, 1 )
        {
            Ch = C;
        }

        public bool IsItem( Chapter C )
        {
            if ( Ch == null ) return false;
            return Ch.Equals( C );
        }

        public bool IsItem( Volume V )
        {
            if ( Vol == null ) return false;
            return Vol.Equals( V );
        }

        public Chapter GetChapter()
        {
            if ( Ch == null )
            {
                if ( Vol.ChapterList.Length == 0 ) return null;
                return Vol.ChapterList[ 0 ];
            }
            return Ch;
        }
    }
}
