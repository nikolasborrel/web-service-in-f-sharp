module Tests

open System.Net
open Xunit
open Microsoft.AspNetCore.TestHost
open TestUtils
open RadioDeviceService

// ---------------------------------
// Radio profile
// ---------------------------------

let radioProfileCreatedSucces = "200 OK (radio profile created)\n"
let locationSetSucces = "200 OK (location was set)\n"
let locationSetForbiddenRequest = "403 FORBIDDEN (location not allowed)\n"
let radioIdNotFound = "404 NOT FOUND (No radio with id)\n"
let locationNotFound = "404 NOT FOUND (location undefined/unknown)\n"
let notFound = "404 NOT FOUND\n"

type RadioDto_malformed = { alias_: string; allowed_locations_: list<string> }
type RadioDto_malformed_no_locations = { alias: string }
type RadioLocationDto_malformed = { location_: string }

// ---------------------------------
// Create radio profile
// ---------------------------------

[<Fact>]
let ``Create radio profile with POST route /radios/{id} returns 200 OK"`` () =
    // arrange
    use server = new TestServer(createHost())
    use client = server.CreateClient()   

    // act
    client
    |> httpPost<RadioDto> "/radios/1" { alias = "Radio CPH"; allowed_locations = ["CPH-1"; "CPH-2"] }

    // assert
    |> ensureSuccess
    |> readText
    |> shouldContain radioProfileCreatedSucces

[<Fact>]
let ``Set radio profile missing locations with POST route /radios/{id} returns 400 BAD REQUEST"`` () =
    // arrange
    use server = new TestServer(createHost())
    use client = server.CreateClient()

    // act
    client
    |> httpPost "/radios/1" { alias = "Radio CPH"; allowed_locations = [] } 
    // assert
    |> isStatus HttpStatusCode.BadRequest
    |> ignore

[<Fact>]
let ``Set radio profile missing locations field with POST route /radios/{id} returns 400 BAD REQUEST"`` () =
    // arrange
    use server = new TestServer(createHost())
    use client = server.CreateClient()

    // act
    client
    |> httpPost "/radios/1" { alias = "Radio CPH" }
    // assert
    |> isStatus HttpStatusCode.BadRequest
    |> ignore

[<Fact>]
let ``Set mal-formed radio profile with POST route /radios/{id} returns 400 BAD REQUEST"`` () =
    // arrange
    use server = new TestServer(createHost())
    use client = server.CreateClient()

    // act
    client
    |> httpPost "/radios/1" { alias_ = "Radio CPH"; allowed_locations_ = ["CPH-1"; "CPH-2"] } 
    // assert
    |> isStatus HttpStatusCode.BadRequest
    |> ignore

// ---------------------------------
// Radio location
// ---------------------------------

[<Fact>]
let ``Set radio location with POST route /radios/{id}/location returns 200 OK"`` () =
    // arrange
    use server = new TestServer(createHost())
    use client = server.CreateClient()

    client
    |> httpPost "/radios/1" { alias = "Radio CPH"; allowed_locations = ["CPH-1"; "CPH-2"] } 
    |> ignore

    // act
    client
    |> httpPost "/radios/1/location" { location = "CPH-1" }

    // assert
    |> ensureSuccess
    |> readText
    |> shouldEqual locationSetSucces

[<Fact>]
let ``Set radio location for non-existing profile with POST route /radios/{id}/location returns 404 NOT FOUND"`` () =
    // arrange
    use server = new TestServer(createHost())
    use client = server.CreateClient()

    client
    |> httpPost "/radios/1" { alias = "Radio CPH"; allowed_locations = ["CPH-1"; "CPH-2"] } 
    |> ignore

    // act
    client
    |> httpPost "/radios/2/location" { location = "CPH-1" }

    // assert
    |> isStatus HttpStatusCode.NotFound
    |> readText
    |> shouldEqual radioIdNotFound

[<Fact>]
let ``Set non-existing radio location with POST route /radios/{id}/location returns 403 FORBIDDEN"`` () =
    // arrange
    use server = new TestServer(createHost())
    use client = server.CreateClient()

    client
    |> httpPost "/radios/1" { alias = "Radio CPH"; allowed_locations = ["CPH-1"; "CPH-2"] } 
    |> ignore   

    // act
    client
    |> httpPost "/radios/1/location" { location = "NOT ALLOWED" }

    // assert
    |> isStatus HttpStatusCode.Forbidden
    |> readText
    |> shouldEqual locationSetForbiddenRequest

[<Fact>]
let ``Set mal-formed radio location with POST route /radios/{id}/location returns 400 BAD REQUEST"`` () =
    // arrange
    use server = new TestServer(createHost())
    use client = server.CreateClient()
    
    client
    |> httpPost "/radios/1" { alias = "Radio CPH"; allowed_locations = ["CPH-1"; "CPH-2"] } 
    |> ignore

    // act
    client
    |> httpPost "/radios/1/location" { location_ = "CPH-1" }

    // assert
    |> isStatus HttpStatusCode.BadRequest
    |> ignore

[<Fact>]
let ``Get radio location with route /radios/{id}/location returns 200 OK and valid JSON"`` () =
    // arrange
    use server = new TestServer(createHost())
    use client = server.CreateClient()
    
    let locationDto = { location = "CPH-1" }

    client
    |> httpPost "/radios/1" { alias = "Radio CPH"; allowed_locations = ["CPH-1"; "CPH-2"] } 
    |> ignore
    
    client
    |> httpPost "/radios/1/location" locationDto 
    |> ignore

    // act
    client
    |> httpGet "/radios/1/location"

    // assert
    |> ensureSuccess
    |> readJson
    |> shouldEqual locationDto

[<Fact>]
let ``Get radio with non-existing location with route /radios/{id}/location returns 404 NOT FOUND"`` () =
    // arrange
    use server = new TestServer(createHost())
    use client = server.CreateClient()

    client
    |> httpPost "/radios/1" { alias = "Radio CPH"; allowed_locations = ["CPH-1"; "CPH-2"] } 
    |> ignore

    // act
    client
    |> httpGet "/radios/1/location"

    // assert
    |> isStatus HttpStatusCode.NotFound
    |> readText
    |> shouldEqual locationNotFound

// ---------------------------------
// Route
// ---------------------------------

[<Fact>]
let ``Route which doesn't exist returns 404 NOT FOUND`` () =
    use server = new TestServer(TestUtils.createHost())
    use client = server.CreateClient()

    client
    |> httpGet "/route/which/does/not/exist"
    |> isStatus HttpStatusCode.NotFound
    |> readText
    |> shouldEqual notFound

// ---------------------------------
// SCENARIO 1
// ---------------------------------

[<Fact>]
let ``SCENARIO 1`` () =
    // arrange
    use server = new TestServer(createHost())
    use client = server.CreateClient()   

    // act
    client
    |> httpPost "/radios/100" { alias = "Radio100"; allowed_locations = ["CPH-1"; "CPH-2"] }
    |> ensureSuccess
    |> ignore

    client
    |> httpPost "/radios/101" { alias = "Radio101"; allowed_locations = ["CPH-1"; "CPH-2"; "CPH-3"] }
    |> ensureSuccess
    |> ignore

    client
    |> httpPost "/radios/100/location" { location = "CPH-1" }
    |> ensureSuccess
    |> ignore

    client
    |> httpPost "/radios/101/location" { location = "CPH-3" }
    |> ensureSuccess
    |> ignore

    client
    |> httpPost "/radios/100/location" { location = "CPH-3" }
    |> isStatus HttpStatusCode.Forbidden
    |> ignore

    client
    |> httpGet "/radios/100/location"
    |> ensureSuccess
    |> readJson
    |> shouldEqual { location = "CPH-1" }

    client
    |> httpGet "/radios/101/location"
    |> ensureSuccess
    |> readJson
    |> shouldEqual { location = "CPH-3" }

// ---------------------------------
// SCENARIO 2
// ---------------------------------

[<Fact>]
let ``SCENARIO 2`` () =
    // arrange
    use server = new TestServer(createHost())
    use client = server.CreateClient()   

    // act
    client
    |> httpPost "/radios/102" { alias = "Radio102"; allowed_locations = ["CPH-1"; "CPH-3"] }
    |> ensureSuccess
    |> ignore

    client
    |> httpGet "/radios/102/location"
    |> isStatus HttpStatusCode.NotFound
    |> ignore