using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using XIVApiLib.Models.Responses;
using Newtonsoft.Json;
using XIVApiLib.Models;

namespace XIVApiLib
{
    internal struct HttpClientResponse
    {
        public bool HasError;
        public string Message;
    }

    public class XIVApi
    {

        //  The API token used to perform queries to the api
        private readonly string _token;
        private readonly string _scheme = "https";
        private readonly string _host = "xivapi.com";


        public XIVApi(string token)
        {
            _token = token;
        }

        private async Task<HttpClientResponse> TryGetResponseAsync(string path, IDictionary<string, string> queryDictionary)
        {
            UriBuilder uriBuilder = new UriBuilder(_scheme, _host);
            uriBuilder.Path = path;

            //  Inject the api key into the query
            if (!queryDictionary.ContainsKey("private_key"))
            {
                queryDictionary.Add("private_key", _token);
            }

            //  Build the query string
            string[] queries = new string[queryDictionary.Count];
            int index = 0;
            foreach (KeyValuePair<string, string> kvp in queryDictionary)
            {
                queries[index] = $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}";
                index++;
            }

            uriBuilder.Query = string.Join('&', queries);

            HttpClientResponse response;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    response.Message = await client.GetStringAsync(uriBuilder.ToString());
                    response.HasError = false;
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.HasError = true;
                }
            }

            return response;
        }

        private async Task<HttpClientResponse> TryGetResponseAsync(string path, NameValueCollection query)
        {
            UriBuilder uriBuilder = new UriBuilder(_scheme, _host);
            uriBuilder.Path = path;
            query["private_key"] = _token;
            uriBuilder.Query = query.ToString();

            HttpClientResponse response;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    response.Message = await client.GetStringAsync(uriBuilder.ToString());
                    response.HasError = false;
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.HasError = true;
                }
            }

            return response;


        }

        public async Task<CharacterResponseModel> CharacterByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new CharacterResponseModel
                {
                    Error = true,
                    Message = "Name cannot be empty."
                };
            }

            NameValueCollection query = new NameValueCollection();
            query["name"] = name;

            HttpClientResponse clientResponse = await TryGetResponseAsync("character/search", query);
            if (clientResponse.HasError)
            {
                return new CharacterResponseModel()
                {
                    Error = true,
                    Message = clientResponse.Message
                };
            }
            else
            {
                return JsonConvert.DeserializeObject<CharacterResponseModel>(clientResponse.Message);
            }
        }

        public async Task<CharacterResponseModel> CharacterByName(string server, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                return new CharacterResponseModel
                {
                    Error = true,
                    Message = "First Name cannot be null"
                };
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                return new CharacterResponseModel
                {
                    Error = true,
                    Message = "First Name cannot be null"
                };
            }

            if (string.IsNullOrWhiteSpace(server))
            {
                return new CharacterResponseModel
                {
                    Error = true,
                    Message = "Server cannot be null"
                };
            }

            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("name", $"{firstName}+{lastName}");
            query.Add("server", server);


            HttpClientResponse clientResponse = await TryGetResponseAsync("character/search", query);
            if (clientResponse.HasError)
            {
                return new CharacterResponseModel()
                {
                    Error = true,
                    Message = clientResponse.Message
                };
            }
            else
            {
                return JsonConvert.DeserializeObject<CharacterResponseModel>(clientResponse.Message);
            }
        }

        public async Task<CharacterResponseModel> CharacterByLodestoneID(string lodestoneID)
        {
            if (string.IsNullOrWhiteSpace(lodestoneID))
            {
                return new CharacterResponseModel
                {
                    Error = true,
                    Message = "Lodeston ID cannot be empty."
                };
            }

            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("data", "fc");

            HttpClientResponse clientResponse = await TryGetResponseAsync($"character/{lodestoneID}", query);
            if (clientResponse.HasError)
            {
                return new CharacterResponseModel()
                {
                    Error = true,
                    Message = clientResponse.Message
                };
            }
            else
            {
                return JsonConvert.DeserializeObject<CharacterResponseModel>(clientResponse.Message);
            }
        }

        public async Task<TitleListResponseModel> GetTitlesList()
        {
            //NameValueCollection query = new NameValueCollection();
            //query["indexes"] = "Title";
            //query["page"] = "1";

            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add("indexes", "title");
            query.Add("page", "1");

            TitleListResponseModel result = new TitleListResponseModel();

            HttpClientResponse clientResponse = await TryGetResponseAsync("search", query);
            if (clientResponse.HasError)
            {
                return new TitleListResponseModel()
                {
                    Error = true,
                    Message = clientResponse.Message
                };
            }
            else
            {
                TitleListResponseModel temp = JsonConvert.DeserializeObject<TitleListResponseModel>(clientResponse.Message);
                result.Results.AddRange(temp.Results);

                while(temp.Pagination.PageNext.HasValue)
                // for (int p = 2; p <= temp.Pagination.PageNext; p++)
                {
                    query["page"] = $"{temp.Pagination.PageNext}";
                    clientResponse = await TryGetResponseAsync("search", query);

                    if(clientResponse.HasError)
                    {
                        return new TitleListResponseModel()
                        {
                            Error = true,
                            Message = clientResponse.Message
                        };
                    }
                    else
                    {
                        temp = JsonConvert.DeserializeObject<TitleListResponseModel>(clientResponse.Message);
                        result.Results.AddRange(temp.Results);
                    }
                }

                return result;
            }

        }
    }
}