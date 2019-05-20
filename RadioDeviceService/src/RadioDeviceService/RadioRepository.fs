namespace RadioDeviceService

module RadioRepository =    
    type RadioRepository = Map<string, Radio>
    type RadioID = int
    type RadioLocation = string

    type FailureReason =
        | RadioNotFound
        | RadioLocationForbidden

    let mutable _radioRepository = Map.empty<RadioID, Radio>

    type RepositoryStatus =
        | Ok
        | Failure of FailureReason

    // ---------------------------------
    // getRadio
    // ---------------------------------

    let getRadio radioId = 
        if _radioRepository.ContainsKey(radioId) then
            Some _radioRepository.[radioId]
        else
            None

    // ---------------------------------
    // addRadio
    // ---------------------------------

    let addRadio' radioRepository radioID radioDto = 
        let radio = { 
            alias = radioDto.alias; 
            allowed_locations = radioDto.allowed_locations; 
            location = None; }

        _radioRepository <- _radioRepository.Add(radioID, radio)

    let addRadio = addRadio' _radioRepository

    // ---------------------------------
    // addRadioLocation
    // ---------------------------------

    let addRadioLocation' rep radioID location  = 
        if _radioRepository.ContainsKey(radioID) then
            let radio = _radioRepository.[radioID]
            if List.exists (fun elem -> elem.Equals(location)) radio.allowed_locations then
                let radio' = { radio with location = Some(location) }
                _radioRepository <- _radioRepository.Add( radioID, radio')
                Ok
            else
                Failure RadioLocationForbidden
        else 
            Failure RadioNotFound
    
    let addRadioLocation radioID (location : string) = 
        addRadioLocation' _radioRepository radioID location