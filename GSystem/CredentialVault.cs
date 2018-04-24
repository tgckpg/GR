using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage.Streams;

using Net.Astropenguin.IO;
using Net.Astropenguin.Logging;

namespace GR.GSystem
{
	using Ext;
	using Settings;

	sealed class LoginInfo : XParameter
	{
		public string Account { get; private set; }
		public string Password { get; private set; }

		public LoginInfo( IMember Member ) : base( Member.GetType().FullName ) { }

		public async Task SetInfo( string Name, string Passwd )
		{
			DataProtectionProvider DPP = new DataProtectionProvider( "LOCAL=user" );

			IBuffer BuffMsg = CryptographicBuffer.ConvertStringToBinary(
				Name.Replace( "\n", "\\n" ) + "\n" + Passwd.Replace( "\n", "\\n" )
				, BinaryStringEncoding.Utf8 );
			IBuffer EncInfo = await DPP.ProtectAsync( BuffMsg );

			SetValue( new XKey( "v", CryptographicBuffer.EncodeToBase64String( EncInfo ) ) );
		}

		public async Task SetInfo( XParameter Param )
		{
			if ( Param == null ) return;

			try
			{
				DataProtectionProvider DPP = new DataProtectionProvider();
				IBuffer DecBuff = await DPP.UnprotectAsync( CryptographicBuffer.DecodeFromBase64String( Param.GetValue( "v" ) ) );

				string[] Info = CryptographicBuffer.ConvertBinaryToString( BinaryStringEncoding.Utf8, DecBuff ).Split( '\n' );
				Account = Info[ 0 ].Replace( "\\n", "\n" );
				Password = Info[ 1 ].Replace( "\\n", "\n" );
			}
			catch ( Exception ex )
			{
				Logger.Log( "LoginInfo", ex.Message, LogType.WARNING );
			}
		}
	}

	sealed class CredentialVault
	{
		private readonly string ID = typeof( CredentialVault ).Name;

		private XRegistry AuthReg;

		public CredentialVault()
		{
			string SettingsFile = FileLinks.ROOT_AUTHMGR + "vault.xml";
			AuthReg = new XRegistry( "<keys />", SettingsFile );
		}

		public static async Task<LoginInfo> Protect( IMember Member, string Name, string Passwd )
		{
			LoginInfo Info = new LoginInfo( Member );
			await Info.SetInfo( Name, Passwd );
			return Info;
		}

		public void Store( LoginInfo Info )
		{
			if ( AuthReg.Parameter( Info.Id ) == null )
			{
				AuthReg.SetParameter( Info );
				AuthReg.Save();
			}
		}

		public async Task<LoginInfo> Retrieve( IMember Member )
		{
			LoginInfo Info = new LoginInfo( Member );
			await Info.SetInfo( AuthReg.Parameter( Info.Id ) );
			return Info;
		}

		public void Remove( IMember Member )
		{
			string Scope = Member.GetType().FullName;
			AuthReg.RemoveParameter( Scope );
			AuthReg.Save();
		}

	}
}