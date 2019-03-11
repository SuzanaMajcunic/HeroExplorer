using HeroExplorer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace HeroExplorer
{

    public class MarvelFacade
    {
        private const string PublicKey = "2b60fe4546e1042ac0f79c98af59e022";
        private const string PrivateKey = "3eb58c70ba1cca64d7a2a95d51be98b7eb99ac45";
        private const int MaxCharacters = 1500;
        private const string ImageNotAvailablePath = "http://i.annihil.us/u/prod/marvel/i/mg/b/40/image_not_available";

        public static async Task PopulateMarvelCharactersAsync(ObservableCollection<Character> marvelCharacters)
        {

            try
            {
                var characterDataWrapper = await GetCharacterDataWrapperAsync();
                var characters = characterDataWrapper.data.results;

                foreach (var character in characters)
                {
                    // Filter characters that are missing thumbnail images
                    if (character.thumbnail != null
                        && character.thumbnail.path != ""
                        && character.thumbnail.path != ImageNotAvailablePath)
                    {
                        character.thumbnail.small = String.Format("{0}/standard_small.{1}",
                            character.thumbnail.path, character.thumbnail.extension);
                        character.thumbnail.large = String.Format("{0}/portrait_xlarge.{1}",
                            character.thumbnail.path, character.thumbnail.extension);

                        marvelCharacters.Add(character);
                    }
                }
            }
            catch (Exception)
            {

                return;
            }

        }

        public static async Task PopulateMarvelComicsAsync(int characterId, ObservableCollection<ComicBook> marvelComics)
        {

            try
            {
                var comicDataWrapper = await GetComicDataWrapperAsync(characterId);
                var comics = comicDataWrapper.data.results;

                foreach (var comic in comics)
                {
                    // Filter characters that are missing thumbnail images
                    if (comic.thumbnail != null
                        && comic.thumbnail.path != ""
                        && comic.thumbnail.path != ImageNotAvailablePath)
                    {
                        comic.thumbnail.small = String.Format("{0}/portrait_medium.{1}",
                            comic.thumbnail.path, comic.thumbnail.extension);
                        comic.thumbnail.large = String.Format("{0}/portrait_xlarge.{1}",
                            comic.thumbnail.path, comic.thumbnail.extension);

                        marvelComics.Add(comic);
                    }
                }
            }
            catch (Exception)
            {

                return;
            }

        }

        private async static Task<CharacterDataWrapper> GetCharacterDataWrapperAsync()
        {
            Random random = new Random();
            var offset = random.Next(MaxCharacters);
            string url = String.Format("https://gateway.marvel.com:443/v1/public/characters?limit=10&offset={0}", offset);

            // Call out to Marvel
            var jsonMessage = await CallMarvelAsync(url);

            // 2.way - just one line - Newtonsoft.Json
            var result = JsonConvert.DeserializeObject<CharacterDataWrapper>(jsonMessage);
            return result;
        }

        private async static Task<ComicDataWrapper> GetComicDataWrapperAsync(int characterId)
        {
            string url = String.Format("https://gateway.marvel.com:443/v1/public/comics?characters={0}&limit=10", characterId);

            // Call out to Marvel
            var jsonMessage = await CallMarvelAsync(url);

            // 2.way - just one line - Newtonsoft.Json
            var result = JsonConvert.DeserializeObject<ComicDataWrapper>(jsonMessage);
            return result;
        }

        private async static Task<string> CallMarvelAsync(string url)
        {
            // Assemble the URL
            var timeStamp = DateTime.Now.Ticks.ToString();

            // Get the MD5 Hash
            var hash = CreateHash(timeStamp);
            string completeUrl = String.Format("{0}&apikey={1}&ts={2}&hash={3}", url, PublicKey, timeStamp, hash);

            // Call out to Marvel
            HttpClient http = new HttpClient();
            var response = await http.GetAsync(completeUrl); //it is type of HttpResponseMessage
            return await response.Content.ReadAsStringAsync(); // json string

        }

        private static string CreateHash(string timeStamp)
        {
            var toBeHashed = timeStamp + PrivateKey + PublicKey;
            var hashedMessage = ComputeMD5(toBeHashed);
            return hashedMessage;
        }

        private static string ComputeMD5(string str)
        {
            // Create a HashAlgorithmProvider object
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            // Convert the message string to binary data
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            // Hash the message
            var hashed = alg.HashData(buff);
            // Convert the hash to a hex string (for display)
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;

            // OR - ANOTHER SOLUTION BY MD5 CLASS
            //// Step 1 - calculate the md5 hash from input
            //MD5 md5 = MD5.Create();
            //byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(str);
            //byte[] hashBytes = md5.ComputeHash(inputBytes);
            //// Step 2 - convert byte array to hex string
            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < hashBytes.Length; i++)
            //{
            //    sb.Append(hashBytes[i].ToString("X2"));
            //}
            //return sb.ToString();

        }
    }
}
