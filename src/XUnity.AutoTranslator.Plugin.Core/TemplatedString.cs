using System.Collections.Generic;

namespace XUnity.AutoTranslator.Plugin.Core
{
   internal class TemplatedString
   {
      public TemplatedString( string template, Dictionary<string, string> arguments )
      {
         Template = template;
         Arguments = arguments;
      }

      public string Template { get; private set; }

      public Dictionary<string, string> Arguments { get; private set; }

      public string Untemplate( string text )
      {
         foreach( var kvp in Arguments )
         {
            text = text.Replace( kvp.Key, kvp.Value );
         }
         return text;
      }

      public string PrepareUntranslatedText( string untranslatedText )
      {
         foreach( var kvp in Arguments )
         {
            var key = kvp.Key;
            //Common.Logging.XuaLogger.AutoTranslator.Info( "key:"+ key );
            var translatorFriendlyKey = CreateTranslatorFriendlyKey( key );
            //Common.Logging.XuaLogger.AutoTranslator.Info( "translatorFriendlyKey:" + translatorFriendlyKey );

            untranslatedText = untranslatedText.Replace( key, translatorFriendlyKey );
         }

         return untranslatedText;
      }

      public string FixTranslatedText( string translatedText, bool useTranslatorFriendlyArgs )
      {
         foreach( var kvp in Arguments )
         {
            var key = kvp.Key;
            //Common.Logging.XuaLogger.AutoTranslator.Info( "key:"+ key );
            var translatorFriendlyKey = useTranslatorFriendlyArgs ? CreateTranslatorFriendlyKey( key ) : key;
            //Common.Logging.XuaLogger.AutoTranslator.Info( "translatorFriendlyKey:" + translatorFriendlyKey );
            translatedText = ReplaceApproximateMatches( translatedText, translatorFriendlyKey, key );
         }
         //Common.Logging.XuaLogger.AutoTranslator.Info( "translatedText:" + translatedText );
         return translatedText;
      }

      public static string CreateTranslatorFriendlyKey( string key )
      {
         var c = key[ 2 ];
         var translatorFriendlyKey = "ZM" + (char)( c + 2 ) + "Z";
         return translatorFriendlyKey;
      }

      public static string ReplaceApproximateMatches( string translatedText, string translatorFriendlyKey, string key )
      {
         var cidx = 0; // translatorFriendlyKey position
         var startIdx = 0; // translatorFriendlyKey start position

         for( int i = 0; i < translatedText.Length; i++ )
         {
            var c = translatedText[ i ]; // translated text char on position i
            if( c == ' ' || c == '　' ) continue; // translator service can break keys and split them with this chars

            //Common.Logging.XuaLogger.AutoTranslator.Info( 
            //     "\r\nc:"+ c.ToString()
            //   + "\r\ntranslatorFriendlyKey[ cidx ]"+ translatorFriendlyKey[ cidx ] 
            //   );

            var firstloopcheck = true; // prevents infinity move and check with goto CheckChars below

            CheckChars:
            if( char.ToUpperInvariant( c ) == char.ToUpperInvariant( translatorFriendlyKey[ cidx ] ) ) // translatorFriendlyKey char equal with text char
            {
               if( cidx == 0 )
               {
                  startIdx = i; // when translatorFriendlyKey position is first - set start index to text index
               }

               cidx++; // move translatorFriendlyKey current char position
            }
            else if( firstloopcheck )
            {
               bool goandrecheck = cidx > 0; // recheck only when

               // reset translatorFriendlyKey current char position and start index if one of chars was not equal
               cidx = 0;
               startIdx = 0;

               if( goandrecheck )
               {
                  firstloopcheck = false; // do not go here after recheck current char
                  goto CheckChars; // make sure first key's char not equal current text char. to not skip keypairs like ZMDZZMCZ
               }
            }

            //Common.Logging.XuaLogger.AutoTranslator.Info( 
            //     "\r\ni:"+ i
            //   + "\r\ncidx" + cidx
            //   + "\r\nstartIdx" + startIdx
            //   );

            if( cidx == translatorFriendlyKey.Length ) // replace translatorFriendlyKey to key only when translatorFriendlyKey position equal with length
            {
               int endIdx = i + 1;

               var lengthOfKey = endIdx - startIdx;
               var diff = lengthOfKey - key.Length;

               translatedText = translatedText.Remove( startIdx, lengthOfKey ).Insert( startIdx, ( startIdx > 0 && !char.IsWhiteSpace( translatedText[ startIdx - 1 ] ) ? " " : "" ) + key );

               i -= diff;

               cidx = 0;
               startIdx = 0;
            }
         }

         return translatedText;
      }
   }
}
