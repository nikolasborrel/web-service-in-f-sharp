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

    let handleGetRadioLocation' (radio : Radio) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            match radio.location with
            | Some location ->  
                let locationDto = { location = location }
                json locationDto next ctx
            | None -> locationNotFoundRequest next ctx

    let handleGetRadioLocation (id : RadioID) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                match (getRadio id) with
                   | Some radio -> return! handleGetRadioLocation' radio next ctx
                   | None -> return! radioIdNotFound next ctx
            }
    
    // ---------------------------------
    // handlePostRadio
    // ---------------------------------

    let handlePostRadio' (id : RadioID) (radioDto : RadioDto) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            addRadio id radioDto
            radioProfileCreatedSucces next ctx

    let handlePostRadio (id : RadioID) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {

                // To ease the understanding of bindJson, note that:
                //
                // validateModel : RadioDto -> HttpHandler -> 'T -> HttpHandler
                //
                // where
                //
                // HttpHandler : HttpFunc -> HttpFunc
                // HttpFunc : HttpContext -> HttpFuncResult

                return! bindJson<RadioDto> (validateModel (handlePostRadio' id)) next ctx
            }

    // ---------------------------------
    // handlePostRadioLocation
    // ---------------------------------

    let handlePostRadioLocation' (id : RadioID) (locationDto : RadioLocationDto) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            match (addRadioLocation id locationDto.location) with
                | Failure RadioNotFound ->  radioIdNotFound next ctx
                | Failure RadioLocationForbidden ->  locationSetForbiddenRequest next ctx
                | Ok -> locationSetSucces next ctx

    let handlePostRadioLocation (id : RadioID) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                return! bindJson<RadioLocationDto> (validateModel (handlePostRadioLocation' id)) next ctx
            }