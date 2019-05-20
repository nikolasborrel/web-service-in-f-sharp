namespace RadioDeviceService

module RadioRepository =
    type RadioID = int
    type RadioLocation = string

    type FailureReason =
        | RadioNotFound
        | RadioLocationForbidden

    type RepositoryStatus =
        | Ok
        | Failure of FailureReason

    val getRadio : RadioID -> Radio option
    val addRadio : (RadioID -> RadioDto -> unit)
    val addRadioLocation : RadioID -> string -> RepositoryStatus