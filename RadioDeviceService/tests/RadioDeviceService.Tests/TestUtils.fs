module TestUtils

open System
open System.IO
open System.Net
open System.Net.Http
open Xunit
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Newtonsoft.Json
open System.Text

// ---------------------------------
// Helper functions (extend as you need)
// ---------------------------------

let createHost() =
    WebHostBuilder()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .Configure(Action<IApplicationBuilder> RadioDeviceService.App.configureApp)
        .ConfigureServices(Action<IServiceCollection> RadioDeviceService.App.configureServices)

let runTask task =
    task
    |> Async.AwaitTask
    |> Async.RunSynchronously

let httpGet (path : string) (client : HttpClient) =
    path
    |> client.GetAsync
    |> runTask

let httpPost<'T> (path : string) (payload: 'T) (client : HttpClient) =
    let json = JsonConvert.SerializeObject(payload);
    let content = new StringContent(json, Encoding.UTF8, "application/json")

    (path, content) 
    |> client.PostAsync 
    |> runTask

let isStatus (code : HttpStatusCode) (response : HttpResponseMessage) =
    Assert.Equal(code, response.StatusCode)
    response

let ensureSuccess (response : HttpResponseMessage) =
    if not response.IsSuccessStatusCode
    then response.Content.ReadAsStringAsync() |> runTask |> failwithf "%A"
    else response

let readText (response : HttpResponseMessage) =
    response.Content.ReadAsStringAsync()
    |> runTask

let readJson<'T> (response : HttpResponseMessage) =
    response.Content.ReadAsStringAsync()
    |> runTask
    |> JsonConvert.DeserializeObject<'T>

let shouldEqual expected actual =
    Assert.StrictEqual(expected, actual)

let shouldContain (expected : string) (actual : string) =
    Assert.True(actual.Contains expected)