using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Ext
{
    static class XProto 
    {
        public const string Init = "wenku8_protocol.Init, wenku8-protocol"; 
        public const string Verification = "wenku8.System.Verification, wenku8-protocol";

        public const string BookItemEx = "wenku8.Model.Book.BookItemEx, wenku8-protocol";

        public const string WRuntimeCache = "wenku8.AdvDM.WRuntimeCache, wenku8-protocol";
        public const string WRequest = "wenku8.Settings.WRequest, wenku8-protocol";
        public const string WProtocols = "wenku8.Settings.WProtocols, wenku8-protocol";
        public const string WException = "wenku8.System.WException, wenku8-protocol";
        public const string WCode = "wenku8.System.WCode, wenku8-protocol";
        public const string ServerSelector = "wenku8.System.ServerSelector, wenku8-protocol";

        public const string FavSection = "wenku8.Model.Section.FavSection, wenku8-protocol";
        public const string NavListSection = "wenku8.Model.Section.NavListSection, wenku8-protocol";
        public const string RectileSection = "wenku8.Model.Section.RectileSection, wenku8-protocol";
        public const string NavSelections = "wenku8.Model.Section.NavSelections, wenku8-protocol";

        public const string Member = "wenku8.Member, wenku8-protocol";
        public const string MemberInfo = "wenku8.Settings.MemberInfo, wenku8-protocol";

        public const string AutoCache = "wenku8.Model.Loaders.AutoCache, wenku10";
        public const string BookLoader = "wenku8.Model.Loaders.BookLoader, wenku10";
        public const string ListLoader = "wenku8.Model.Loaders.ListLoader, wenku8-protocol";

        public const string NavListSettings = "wenku8.Settings.Layout.NavList, wenku10";
        public const string MainPageSettings = "wenku8.Settings.Layout.MainPage, wenku10";
        public const string MainStage = "wenku10.MainStage, wenku10";

        public const string HistoryPage = "wenku10.Pages.History, wenku10";
        public const string CatListPage = "wenku10.Pages.CategorizedList, wenku10";
        public const string NavListPage = "wenku10.Pages.NavList, wenku10";
    }
}
