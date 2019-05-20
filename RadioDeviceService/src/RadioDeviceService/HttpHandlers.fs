namespace RadioDeviceService

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open Giraffe
    open RadioRepository

    let radioProfileCreatedSucces = 
        (setStatusCode StatusCodes.Status200OK >=> text "200 OK (radio profile created)\n")

    let locationSetSucces = 
        (setStatusCode StatusCodes.Status200OK >=> text "200 OK (location was set)\n")

    let locationSetForbiddenRequest = 
        (setStatusCode StatusCodes.Status403Forbidden >=> text "403 FORBIDDEN (location not allowed)\n")

    let locationNotFoundRequest = 
        (setStatusCode StatusCodes.Status404NotFound >=> text "404 NOT FOUND (location undefined/unknown)\n")

    let radioIdNotFound = 
        (setStatusCode StatusCodes.Status404NotFound >=> text "404 NOT FOUND (No radio with id)\n")



    // ---------------------------------
    // handleGetRadioLocation
    // ---------------------------------

    let handleGetRadioLocation' (radio : Radio) next ctx =
        match radio.location with
        | Some location ->  
            let locationDto = { location = location }
            json locationDto next ctx
         | None -> locationNotFoundRequest next ctx

    let handleGetRadioLocation (id) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                match (getRadio id) with
                   | Some radio -> return! handleGetRadioLocation' radio next ctx
                   | None -> return! radioIdNotFound next ctx
            }
    
    // ---------------------------------
    // handlePostRadio
    // ---------------------------------

    let handlePostRadio' id (radioDto : RadioDto) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                addRadio id radioDto
                return! radioProfileCreatedSucces next ctx
            }

    let handlePostRadio id =   
            bindJson<RadioDto> (validateModel (handlePostRadio' id))

    // ---------------------------------
    // handlePostRadioLocation
    // ---------------------------------

    let handlePostRadioLocation' id (locationDto : RadioLocationDto) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                match (addRadioLocation id locationDto.location) with
                    | Failure RadioNotFound ->  return! radioIdNotFound next ctx
                    | Failure RadioLocationForbidden ->  return! locationSetForbiddenRequest next ctx
                    | Ok -> return! locationSetSucces next ctx
            }

    let handlePostRadioLocation id =   
            bindJson<RadioLocationDto> (validateModel (handlePostRadioLocation' id))