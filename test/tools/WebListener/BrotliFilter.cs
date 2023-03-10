// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace mvc.Controllers
{
    internal sealed class BrotliFilter : ResultFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            using (var memoryStream = new MemoryStream())
            {
                var responseStream = httpContext.Response.Body;
                httpContext.Response.Body = memoryStream;

                await next();

                using (var compressedStream = new BrotliStream(responseStream, CompressionLevel.Fastest))
                {
                    httpContext.Response.Headers.Add("Content-Encoding", new[] { "br" });
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(compressedStream);
                }
            }
        }
    }
}
