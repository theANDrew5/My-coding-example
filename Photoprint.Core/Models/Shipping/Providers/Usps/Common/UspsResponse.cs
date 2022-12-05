namespace Photoprint.Core.Models
{
    public class UspsResponse<TResponse>
    {
        public UspsError Error { get; }
        public TResponse Response { get; }
        public bool IsNotSuccess => Error != null;

        public UspsResponse(UspsError error)
        {
            Error = error;
        }
        public UspsResponse(TResponse response)
        {
            Response = response;
        }
    }
}
