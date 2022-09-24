namespace Ct.Interview.Web.Api.Middlewares
{
    public class AsxExceptionDto
    {
        public AsxExceptionDto()
        {
        }

        public AsxExceptionDto(Exception e)
        {
            Message = e.Message;
        }

        public string Message { get; set; } = string.Empty;
    }
}
