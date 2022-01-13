namespace GamerMapsSquadViewClient.Helpers
{
	using System;
	using System.IO;
	using System.Net;
	using System.Net.Http;
	using System.Text;

	internal class HttpManager
	{
		private static readonly HttpClient _httpClient = new HttpClient();
		private static readonly Uri _loginUri = new Uri("https://gamermaps.net/squadview/uploader/login");
		private static readonly Uri _uploadUri = new Uri("https://gamermaps.net/squadview/uploader/upload");

		static HttpManager()
		{
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		}

		internal static HttpResult ClientLogin(String username, String userpass)
		{
			String serverData = "u=" + username + "&p=" + userpass;

			try
			{
				using (StringContent postData = new StringContent(serverData, Encoding.UTF8, "application/x-www-form-urlencoded"))
				using (HttpResponseMessage httpResponseMessage = _httpClient.PostAsync(_loginUri, postData).Result)
				{
					httpResponseMessage.EnsureSuccessStatusCode();
					String result = httpResponseMessage.Content.ReadAsStringAsync().Result;

					Boolean success = !String.Equals(result, "invalid_login");
					return new HttpResult(success, result);
				}
			}
			catch (Exception ex)
			{
				return new HttpResult(false, ex.Message);
			}
		}

		internal static HttpResult UploadFile(String accessKey, String uploadFile)
		{
			try
			{
				using (FileStream fileStream = new FileStream(uploadFile, FileMode.Open, FileAccess.Read))
				using (MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent())
				using (StringContent stringContent = new StringContent(accessKey))
				using (StreamContent imageContent = new StreamContent(fileStream))
				{
					multipartFormDataContent.Add(stringContent, "access_key");
					multipartFormDataContent.Add(imageContent, Path.GetFileName(uploadFile), "image/jpeg");
					using (HttpResponseMessage httpResponseMessage = _httpClient.PostAsync(_uploadUri, multipartFormDataContent).Result)
					{
						httpResponseMessage.EnsureSuccessStatusCode();

						String result = httpResponseMessage.Content.ReadAsStringAsync().Result;

						switch (result)
						{
							case "success":
								return new HttpResult(true, result);

							case "time":
								return new HttpResult(false, "You're uploading to fast...");

							default:
								return new HttpResult(false, result);
						}
					}
				}
			}
			catch (Exception ex)
			{
				return new HttpResult(false, ex.Message);
			}
		}
	}
}
