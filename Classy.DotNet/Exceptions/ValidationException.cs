using System;
using System.Collections.Generic;
using ServiceStack.Text;
using System.Net;

namespace Classy.DotNet
{
    public class ValidationError
    {
        public string FieldName { get; set; }
        public string ErrorCode { get; set; }
    }
        
    public class ClassyException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public IList<ValidationError> Errors { get; private set; }

        public ClassyException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public ClassyException(string errorCode) : base()
        {
            StatusCode = HttpStatusCode.BadRequest;
            Errors = new List<ValidationError> {
                new ValidationError {
                    ErrorCode = errorCode
                }
            };
        }

        public ClassyException(IList<ValidationError> errors) : base() 
        {
            StatusCode = HttpStatusCode.BadRequest;
            Errors = errors;
        }

        public bool IsValidationError()
        {
            return StatusCode == HttpStatusCode.BadRequest;
        }
    }

    public static class ValidationErrorExtensions
    {
        public class ResponseStatus
        {
            public string ErrorCode { get; set; }
            public string Message { get; set; }
            public IList<ValidationError> Errors { get; set; }
        }

        public class BadRequest
        {
            public ResponseStatus ResponseStatus { get; set; }
        }

        public static ClassyException ToClassyException(this WebException wex)
        {
            var responseBody = wex.GetResponseBody();
            if (wex.IsBadRequest())
            {
                var badRequest = responseBody.FromJson<BadRequest>();
                return badRequest.ResponseStatus.Errors.Count == 0 ? new ClassyException(badRequest.ResponseStatus.Message) : new ClassyException(badRequest.ResponseStatus.Errors);
            }
            else return new ClassyException((wex.Response as HttpWebResponse).StatusCode, wex.Message);
        }
    }
}
