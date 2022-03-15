namespace WalletConnectSharp.Core.Models
{
    public class ErrorResponse
    {
        //keep for json deserialization
        private string message;

        public ErrorResponse(string message) => this.message = message;

        public string Message => message;
    }
}