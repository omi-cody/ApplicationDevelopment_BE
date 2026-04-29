using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Bike360.Application.Common
{
    public class ApiResponse
    {
        public object? Data { get; set; }
        public List<string> Errors { get; set; } = [];
        public int StatusCode { get; set; }
        public bool Success => StatusCode >= 200 && StatusCode < 300;

        public ApiResponse(object? data, List<string>? errors, HttpStatusCode statusCode)
        {
            Data = data;
            Errors = errors ?? [];
            StatusCode = (int)statusCode;
        }

        public static ApiResponse Ok(object? data = null) =>
            new(data, null, HttpStatusCode.OK);
    }
}
