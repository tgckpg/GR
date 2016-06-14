using System;
using System.Linq;
using System.Collections.Generic;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Linq;
using Net.Astropenguin.Loaders;
using Net.Astropenguin.Logging;

using libeburc;

namespace wenku8.Model
{
    using Interfaces;
    using ListItem;

    internal class EBDictionary : ActiveData, ISearchableSection<ActiveItem>
    {
        public static readonly string ID = typeof( EBDictionary ).Name;

        private IEnumerable<EBSubbook> subbooks;

        private bool _searching = false;
        public bool Searching
        {
            get { return _searching; }
            set { _searching = value; NotifyChanged( "Searching" ); }
        }

        private int _numresults = 0;
        public int NumResults
        {
            get { return _numresults; }
            set { _numresults = value; NotifyChanged( "NumResults" ); }
        }

        public EBDictionary( IEnumerable<EBSubbook> subbooks )
        {
            this.subbooks = subbooks;
        }

        public IEnumerable<ActiveItem> SearchSet { get; set; }

        private string term = "";
        public string SearchTerm
        {
            get { return term; }
            set { AutoSearch( term = value ); }
        }

        private async void AutoSearch( string Word )
        {
            if ( Searching ) return;

            List<ActiveItem> Results = new List<ActiveItem>();
            Searching = true;

            string[] Term = new string[] { Word };
            try
            {
                foreach ( EBSubbook subbook in subbooks )
                {
                    EBSearchCode[] Codes = subbook.SearchFlags;
                    if ( Codes.Contains( EBSearchCode.EB_SEARCH_WORD ) )
                    {
                        // Search with exact word first
                        IEnumerable<EBHit> Hits = await subbook.SearchAysnc( Term, EBSearchCode.EB_SEARCH_EXACTWORD );

                        // If no results, try the word search
                        if( Hits.Count() == 0 )
                        {
                            Hits = await subbook.SearchAysnc( Term, EBSearchCode.EB_SEARCH_WORD );
                        }

                        IEnumerable<string> Pages = await subbook.GetTextAsync( Hits );

                        Results.AddRange( Pages.Remap( x =>
                        {
                            string[] a = x.Split( new char[] { '\n' }, 2 );
                            return new ActiveItem( a[ 0 ], subbook.Title, a[ 1 ], null );
                        } ) );
                    }
                }
            }
            catch( Exception ex )
            {
                EBErrorCode Code = EBException.LastError;
                if ( Code != EBErrorCode.EB_SUCCESS )
                {
                    Results.Add( new ActiveItem( "An Error Occured", ex.Message, Code.ToString()  + ": " + EBException.LastMessage, null ) );
                }
                else
                {
                    Results.Add( new ActiveItem( "An Error Occured", ex.Message, null ) );
                }
            }

            if ( ( NumResults = Results.Count() ) == 0 )
            {
                StringResources stx = new StringResources( "AppResources" );
                Results.Add( new ActiveItem( stx.Text( "Search_Example" ), null, null ) );
            }

            SearchSet = Results;
            Searching = false;
            NotifyChanged( "SearchSet" );

            // Resume the queue
            if( Word != SearchTerm )
            {
                AutoSearch( SearchTerm );
            }
        }
    }
}