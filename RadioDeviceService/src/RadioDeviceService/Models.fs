namespace RadioDeviceService

open Giraffe

[<CLIMutable>]
type Radio = 
    {
        alias: string
        allowed_locations: list<string>
        location: string Option
    }

[<CLIMutable>]
type RadioDto = 
    {
        alias: string
        allowed_locations: list<string>
    }

    member this.HasErrors() =
            if      isNull this.alias || this.alias.Length = 0 then Some "alias is empty."
            else if isNull (box this.allowed_locations) || this.allowed_locations.Length = 0 then 
                Some "allowed_locations is empty."
            else None

    interface IModelValidation<RadioDto> with
            member this.Validate() =
                match this.HasErrors() with
                | Some msg -> Error (RequestErrors.badRequest (text msg))
                | None     -> Ok this

[<CLIMutable>]
type RadioLocationDto =  
    { 
        location: string 
    }

    member this.HasErrors() =
            if      isNull this.location || this.location.Length = 0 then Some "location is not set (null or empty)."
            else None

    interface IModelValidation<RadioLocationDto> with
            member this.Validate() =
                match this.HasErrors() with
                | Some msg -> Error (RequestErrors.badRequest (text msg))
                | None     -> Ok this