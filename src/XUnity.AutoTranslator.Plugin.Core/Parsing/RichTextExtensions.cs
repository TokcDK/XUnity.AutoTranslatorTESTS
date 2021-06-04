using System.Text.RegularExpressions;
using XUnity.AutoTranslator.Plugin.Core.Configuration;

namespace XUnity.AutoTranslator.Plugin.Core
{
   static class RichTextExtensions
   {
      /// <summary>
      /// removes all rich text markup when HandleRichText is false
      /// </summary>
      /// <param name="input"></param>
      /// <returns></returns>
      public static string RemoveRichTextMarkup( this string input )
      {
         if( Settings.HandleRichText || !Settings.RemoveRichText ) return input;

         return Regex.Replace( input, @"(<[a-z]+[^>]+>)|(<\/[a-z]+>)", string.Empty );
      }

      /// <summary>
      /// restoring broken rich text tags content
      /// </summary>
      /// <param name="translatedText"></param>
      /// <returns></returns>
      public static string RestoreTags( this string translatedText )
      {
         if( Settings.HandleRichText || Settings.RemoveRichText ) return translatedText;

         var tags = Regex.Matches( translatedText, @"<[^<>]+>" );

         for( int i = tags.Count - 1; i >= 0; i-- )
         {
            var tag = tags[ i ];
            var newvalue = tag.Value.ToLowerInvariant().Replace( " ", string.Empty );
            translatedText = translatedText.Remove( tag.Index, tag.Length ).Insert( tag.Index, newvalue );
         }

         translatedText = translatedText.Replace( "> <", "><" );

         return translatedText;
      }
   }
}
