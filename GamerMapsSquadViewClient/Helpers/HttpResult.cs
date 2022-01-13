namespace GamerMapsSquadViewClient.Helpers
{
	using System;

	internal class HttpResult
	{
		private readonly Boolean _success;
		private readonly String _message;

		public HttpResult(Boolean success, String message)
		{
			_success = success;
			_message = message;
		}

		public Boolean Success => _success;

		public String Message => _message;
	}
}
